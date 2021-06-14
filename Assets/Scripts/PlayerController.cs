using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FSM;

public class PlayerController : MonoBehaviour
{
    //Camera
    [HideInInspector]
    public Camera cam;
    [Header("Camera")]
    public float sensitivity = 10.0f;
    public float camSmoothing = 10.0f;
    private float sRotX, sRotY;
    private float camRotX, camRotY;
    public float minCamRot, maxCamRot;
    private float strafeRot = 0.0f;
    public AnimationCurve headbobCurve;
    public float headHeight = 3.0f;
    private float crouchHeadHeightFactor = 1.0f;
    public float headbobFrequency = 1.0f;
    public float headbobAmplitude = 0.1f;
    private float headbobTimer = 0.0f;
    private float headbobIntensity = 0.0f;
    private Transform currentSittingTransform;
    public LayerMask uncrouchMask;
    public bool useHandheldShaking = true;
    public float handheldShakeIntensity = 1.0f;
    public float handheldShakeFrequency = 3.0f;
    private float handheldShakeTimer = 0.0f;

    //Movement
    private CharacterController ct;
    private float uniformSpeed = 0.0f;
    [Header("Movement")]
    public float speed = 10.0f;
    public float acceleration = 5.0f;
    public float deceleration = 5.0f;
    private Vector3 inputVector;
    private Vector3 smoothInputVector;
    public float movementSmoothing = 10.0f;
    private Vector3 lastInputVector;
    public LayerMask groundMask;
    public float gravityMultiplier;
    private float velocity = 0.0f;
    private bool isRunning = false; //for controller only
    public bool canRun = true;
    public bool canCrouch = true;

    //Audio
    [Header("Audio")]
    public AudioClipCollectionObject footsteps;
    private AudioSource audioSource;

    //Interaction
    [Header("Interaction")]
    public LayerMask interactionMask;
    public float interactionRange = 6.0f;

    public enum States{
        Idling,
        Walking,
        Running,
        SlowingDown,

        NumStates,
    }
    [HideInInspector]
    public FiniteStateMachine fsm = new FiniteStateMachine();
    private SubStateMachine ssm = new SubStateMachine("SSM", (int)States.NumStates, States.Idling);

    private void Start() {
        cam = GetComponentInChildren<Camera>();
        ct = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camRotY = this.transform.eulerAngles.y;

        #region FSM
        fsm.AddSubStateMachine(ssm);
        fsm.SetSubStateMachine("SSM");
        fsm.SetState(States.Idling);

        //Any State

        //Idling
        ssm.AddPreTransitionActions(States.Idling, Looking, SimulateGravity, Headbobbing);
        ssm.AddStateTransition(States.Idling, () => {return !IsInputVectorZero();}, States.Walking, () => {headbobTimer = 0.0f;});

        //Walking
        ssm.AddPreTransitionActions(States.Walking, Looking, Accelerate, Headbobbing, SimulateGravity);
        ssm.AddStateTransition(States.Walking, IsInputVectorZero, States.SlowingDown, () => {});        
        ssm.AddStateTransition(States.Walking, GetRunButton, States.Running, () => {});   
        ssm.AddPostTransitionAction(States.Walking, Walking);

        //Running
        ssm.AddPreTransitionActions(States.Running, Looking, AccelerateRunning, Headbobbing, SimulateGravity);
        ssm.AddStateTransition(States.Running, IsInputVectorZero, States.SlowingDown, () => {isRunning = false;});   
        ssm.AddStateTransition(States.Running, () => {return !GetRunButton();}, States.Walking, () => {isRunning = false;});     
        ssm.AddPostTransitionAction(States.Running, Walking);   

        //Slowing Down
        ssm.AddPreTransitionActions(States.SlowingDown, Looking, Decelerate, SlowingDown, Headbobbing, SimulateGravity);
        ssm.AddStateTransition(States.SlowingDown, () => {return !IsInputVectorZero();}, States.Walking, () => {});
        ssm.AddStateTransition(States.SlowingDown, () => {return uniformSpeed <= 0.001f;}, States.Idling, () => {uniformSpeed = 0.0f;});

        #endregion
    }

    private void Update() {
        //Movement
        inputVector = GetInputVector();
        smoothInputVector = Vector3.Lerp(smoothInputVector, inputVector, Time.deltaTime * movementSmoothing);

        fsm.Run();
    }

    #region Input
    float GetRotX(){
        float r = 0.0f;
        if(Mouse.current != null)
            r += Mouse.current.delta.x.ReadValue();
        if(Gamepad.current != null)
            r += Gamepad.current.rightStick.x.ReadValue() * 20.0f;
        return r;
    }
    float GetRotY(){
        float r = 0.0f;
        if(Mouse.current != null)
            r += Mouse.current.delta.y.ReadValue();
        if(Gamepad.current != null)
            r += Gamepad.current.rightStick.y.ReadValue() * 20.0f;
        return r;
    }
    Vector3 GetInputVector(){
        Vector3 inputVector = Vector3.zero;
        if(Keyboard.current != null){
            if(Keyboard.current.dKey.isPressed)
                inputVector.x += 1.0f;
            if(Keyboard.current.aKey.isPressed)
                inputVector.x -= 1.0f;
            if(Keyboard.current.wKey.isPressed)
                inputVector.z += 1.0f;
            if(Keyboard.current.sKey.isPressed)
                inputVector.z -= 1.0f;
        }
        if(Gamepad.current != null){
            inputVector.x += Gamepad.current.leftStick.x.ReadValue();
            inputVector.z += Gamepad.current.leftStick.y.ReadValue();
        }
        return inputVector.normalized;
    }

    bool GetRunButton(){
        if(!canRun)
            return false;
        bool r = false;
        if(Keyboard.current != null)
            if(Keyboard.current.leftShiftKey.isPressed)
                r = true;
        if(Gamepad.current != null)
            if(Gamepad.current.leftStickButton.wasPressedThisFrame){
                isRunning = true;
                r = true;
            }
        return r || isRunning;
    }

    bool GetCrouchButtonDown(){
        if(!canCrouch)
            return false;
        bool r = false;
        if(Keyboard.current != null)
            if(Keyboard.current.leftCtrlKey.wasPressedThisFrame)
                r = true;
        if(Gamepad.current != null)
            if(Gamepad.current.rightStickButton.wasPressedThisFrame)
                r = true;
        return r;
    }

    bool IsInputVectorZero(){
        return inputVector.x == 0.0f && inputVector.z == 0.0f;
    }
    #endregion

    #region Conditions
    bool CanCrouchUp(){
        if(Physics.Raycast(new Ray(cam.transform.position, Vector3.up), out RaycastHit hit, 5.0f, uncrouchMask))
            return false;
        return true;
    }
    #endregion

    #region Actions
    float InversePow(float f, float p){
        return Mathf.Sign(f) * (1.0f - Mathf.Pow(1.0f - Mathf.Abs(f), p));
    }
    Quaternion GetHandheldShaking(){
        handheldShakeTimer += handheldShakeFrequency * Time.deltaTime;
        return Quaternion.Euler(new Vector3(InversePow(Mathf.PerlinNoise(0.0f, handheldShakeTimer) * 2.0f - 1.0f, 2.0f),
                                            InversePow(Mathf.PerlinNoise(0.0f, handheldShakeTimer + 10.0f) * 2.0f - 1.0f, 2.0f),
                                            InversePow(Mathf.PerlinNoise(0.0f, handheldShakeTimer + 30.0f) * 2.0f - 1.0f, 2.0f))
                                             * handheldShakeIntensity * Mathf.Max(0.4f, uniformSpeed));
    }

    void Looking(){
        camRotX -= GetRotY() * sensitivity;
        camRotX = Mathf.Clamp(camRotX, minCamRot, maxCamRot);
        camRotY += GetRotX() * sensitivity;
        sRotX = Mathf.Lerp(sRotX, camRotX, Time.deltaTime * camSmoothing);
        sRotY = Mathf.Lerp(sRotY, camRotY, Time.deltaTime * camSmoothing);
        
        if(inputVector.x > 0.5f)
            strafeRot = Mathf.Lerp(strafeRot, -2.0f, Time.deltaTime * 5.0f);
        else if(inputVector.x < -0.5f)
            strafeRot = Mathf.Lerp(strafeRot, 2.0f, Time.deltaTime * 5.0f);
        else
            strafeRot = Mathf.Lerp(strafeRot, 0.0f, Time.deltaTime * 5.0f);

        cam.transform.localEulerAngles = new Vector3(sRotX, 0.0f, strafeRot);
        cam.transform.rotation *= GetHandheldShaking();
        this.transform.eulerAngles = new Vector3(0.0f, sRotY, 0.0f);
    }

    void LookingSitting(){
        camRotX -= sRotY * sensitivity;
        camRotX = Mathf.Clamp(camRotX, minCamRot, maxCamRot);
        camRotY += sRotX * sensitivity;
        camRotY = Mathf.Clamp(camRotY, currentSittingTransform.eulerAngles.y - 70.0f, currentSittingTransform.eulerAngles.y + 70.0f);
        
        if(inputVector.x > 0.5f)
            strafeRot = Mathf.Lerp(strafeRot, -5.0f, Time.deltaTime * 5.0f);
        else if(inputVector.x < -0.5f)
            strafeRot = Mathf.Lerp(strafeRot, 5.0f, Time.deltaTime * 5.0f);
        else
            strafeRot = Mathf.Lerp(strafeRot, 0.0f, Time.deltaTime * 5.0f);

        cam.transform.localEulerAngles = new Vector3(camRotX, 0.0f, strafeRot);
        this.transform.eulerAngles = new Vector3(0.0f, camRotY, 0.0f);
        cam.transform.position = currentSittingTransform.position;
    }

    void Accelerate(){
        uniformSpeed = Mathf.Lerp(uniformSpeed, 1.0f, Time.deltaTime * acceleration);
        headbobIntensity = Mathf.Lerp(headbobIntensity, 1.0f, Time.deltaTime * acceleration);
    }
    void AccelerateCrouching(){
        uniformSpeed = Mathf.Lerp(uniformSpeed, 0.6f, Time.deltaTime * acceleration);
        headbobIntensity = Mathf.Lerp(headbobIntensity, 1.0f, Time.deltaTime * acceleration);
    }
    void AccelerateRunning(){
        uniformSpeed = Mathf.Lerp(uniformSpeed, 1.5f, Time.deltaTime * acceleration);
        headbobIntensity = Mathf.Lerp(headbobIntensity, 1.0f, Time.deltaTime * acceleration);
    }
    void Decelerate(){
        uniformSpeed = Mathf.Lerp(uniformSpeed, 0.0f, Time.deltaTime * deceleration);
        headbobIntensity = Mathf.Lerp(headbobIntensity, 0.0f, Time.deltaTime * acceleration);
    }
    void JumpingDecelerate(){
        uniformSpeed = Mathf.Lerp(uniformSpeed, 0.0f, Time.deltaTime * deceleration * 0.05f);
        headbobIntensity = Mathf.Lerp(headbobIntensity, 0.0f, Time.deltaTime * acceleration * 0.3f);
    }
    void Walking(){
        Vector3 forward = this.transform.forward;
        Vector3 right = this.transform.right;
        if(Physics.Raycast(new Ray(this.transform.position, Vector3.down), out RaycastHit hit, 6.0f, groundMask)){
            right = Vector3.Cross(hit.normal, this.transform.forward);
            forward = Vector3.Cross(right, hit.normal);
        }
        Vector3 movementVector = Vector3.zero;
        movementVector += forward * smoothInputVector.z;
        movementVector += right * smoothInputVector.x;
        ct.Move(movementVector * uniformSpeed * speed * Time.deltaTime);
        lastInputVector = smoothInputVector;
    }
    void SlowingDown(){
        Vector3 forward = this.transform.forward;
        Vector3 right = this.transform.right;
        if(Physics.Raycast(new Ray(this.transform.position, Vector3.down), out RaycastHit hit, 6.0f, groundMask)){
            right = Vector3.Cross(hit.normal, this.transform.forward);
            forward = Vector3.Cross(right, hit.normal);
        }
        Vector3 movementVector = Vector3.zero;
        movementVector += forward * lastInputVector.z;
        movementVector += right * lastInputVector.x;
        ct.Move(movementVector * uniformSpeed * speed * Time.deltaTime);
    }

    void Headbobbing(){
        headbobTimer += Time.deltaTime * headbobFrequency * uniformSpeed;

        cam.transform.localPosition = new Vector3(0.0f,
            headHeight + headbobCurve.Evaluate(headbobTimer) * headbobIntensity * headbobAmplitude,
            0.0f);

        if(headbobTimer >= 1.0f && !fsm.IsInState(States.SlowingDown)){
            headbobTimer = 0.0f;
            if(fsm.IsInState(States.Walking))
                audioSource.PlayOneShot(footsteps.Get(), 0.5f);
            else if(fsm.IsInState(States.Running))
                audioSource.PlayOneShot(footsteps.Get());
        }
    }

    void SimulateGravity(){
        if(ct.isGrounded)
            velocity = 0.0f;
        else
            velocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        ct.Move(Vector3.up * velocity * Time.deltaTime);
    }

    public void Idle(){
        fsm.SetState(States.Idling);
    }
    #endregion
}
