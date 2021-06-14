using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FSM
{
    public delegate void Action();
    public delegate bool Condition();

    public class StateTransition
    {
        public Condition condition;
        public int stateIndex;
        public Action transitionAction;

        public StateTransition(Condition condition, int stateIndex, Action transitionAction)
        {
            this.condition = condition;
            this.stateIndex = stateIndex;
            this.transitionAction = transitionAction;
        }

        public StateTransition(Condition condition, Enum state, Action transitionAction)
        {
            this.condition = condition;
            this.stateIndex = Convert.ToInt32(state);
            this.transitionAction = transitionAction;
        }
    }

    public class SubStateMachineTransition
    {
        public Condition condition;
        public string subStateMachineName;
        public int stateIndex;
        public Action transitionAction;

        public SubStateMachineTransition(Condition condition, string subStateMachineName, int stateIndex, Action transitionAction)
        {
            this.condition = condition;
            this.subStateMachineName = subStateMachineName;
            this.stateIndex = stateIndex;
            this.transitionAction = transitionAction;
        }

        public SubStateMachineTransition(Condition condition, string subStateMachineName, Enum state, Action transitionAction)
        {
            this.condition = condition;
            this.subStateMachineName = subStateMachineName;
            this.stateIndex = Convert.ToInt32(state);
            this.transitionAction = transitionAction;
        }
    }

    public class State
    {
        public Action preTransitionActions;
        public List<StateTransition> stateTransitions;
        public Action postTransitionActions;
        public List<SubStateMachineTransition> subStateMachineTransitions;

        public State()
        {
            preTransitionActions = new Action(() => { return; });
            stateTransitions = new List<StateTransition>();
            postTransitionActions = new Action(() => { return; });
            subStateMachineTransitions = new List<SubStateMachineTransition>();
        }
    }

    [System.Serializable]
    public class SubStateMachine
    {
        public string name = "SubStateMachine";
        public State[] states;
        public int activeStateIndex = 0;
        public Action anyStateActions;
        public Enum displayEnum = null;

        public SubStateMachine(string name, int numStates, int initialState, Enum displayEnum = null)
        {
            this.name = name;
            states = new State[numStates];
            for (int i = 0; i < numStates; i++)
                states[i] = new State();
            activeStateIndex = initialState;
            anyStateActions = new Action(() => { return; });
            this.displayEnum = displayEnum;
        }

        public SubStateMachine(string name, int numStates, Enum initialState, Enum displayEnum = null)
        {
            this.name = name;
            states = new State[numStates];
            for (int i = 0; i < numStates; i++)
                states[i] = new State();
            activeStateIndex = Convert.ToInt32(initialState);
            anyStateActions = new Action(() => { return; });
            this.displayEnum = displayEnum;
        }

        public void AddPreTransitionAction(int stateIndex, Action preTransitionAction)
        {
            states[stateIndex].preTransitionActions += preTransitionAction;
        }
        public void AddPreTransitionAction(Enum state, Action preTransitionAction)
        {
            states[Convert.ToInt32(state)].preTransitionActions += preTransitionAction;
        }
        public void AddPreTransitionActions(int state, params Action[] preTransitionActions)
        {
            foreach(Action a in preTransitionActions)
                states[state].preTransitionActions += a;
        }
        public void AddPreTransitionActions(Enum state, params Action[] preTransitionActions)
        {
            foreach(Action a in preTransitionActions)
                states[Convert.ToInt32(state)].preTransitionActions += a;
        }

        public void AddStateTransition(int stateIndex, Condition condition, int targetStateIndex, Action transitionAction)
        {
            states[stateIndex].stateTransitions.Add(new StateTransition(condition, targetStateIndex, transitionAction));
        }
        public void AddStateTransition(int stateIndex, Condition condition, Action transitionAction)
        {
            states[stateIndex].stateTransitions.Add(new StateTransition(condition, -1, transitionAction));
        }
        public void AddStateTransition(Enum state, Condition condition, Enum targetState, Action transitionAction)
        {
            states[Convert.ToInt32(state)].stateTransitions.Add(new StateTransition(condition, Convert.ToInt32(targetState), transitionAction));
        }
        public void AddStateTransition(Enum state, Condition condition, Action transitionAction)
        {
            states[Convert.ToInt32(state)].stateTransitions.Add(new StateTransition(condition, -1, transitionAction));
        }

        public void AddPostTransitionAction(int stateIndex, Action postTransitionAction)
        {
            states[stateIndex].postTransitionActions += postTransitionAction;
        }
        public void AddPostTransitionAction(Enum state, Action postTransitionAction)
        {
            states[Convert.ToInt32(state)].postTransitionActions += postTransitionAction;
        }
        public void AddPostTransitionActions(int state, params Action[] postTransitionActions)
        {
            foreach(Action a in postTransitionActions)
                states[state].postTransitionActions += a;
        }
        public void AddPostTransitionActions(Enum state, params Action[] postTransitionActions)
        {
            foreach(Action a in postTransitionActions)
                states[Convert.ToInt32(state)].postTransitionActions += a;
        }

        public void AddSubStateMachineTransition(int stateIndex, Condition condition, string targetSubStateMachineName, int targetStateIndex, Action transitionAction)
        {
            states[stateIndex].subStateMachineTransitions.Add(new SubStateMachineTransition(condition, targetSubStateMachineName, targetStateIndex, transitionAction));
        }
        public void AddSubStateMachineTransition(Enum state, Condition condition, string targetSubStateMachineName, Enum targetState, Action transitionAction)
        {
            states[Convert.ToInt32(state)].subStateMachineTransitions.Add(new SubStateMachineTransition(condition, targetSubStateMachineName, Convert.ToInt32(targetState), transitionAction));
        }

        public void AddAnyStateAction(Action action)
        {
            anyStateActions += action;
        }

        public int GetActiveStateIndex()
        {
            return activeStateIndex;
        }
    }

    [System.Serializable]
    public class FiniteStateMachine
    {
        private Dictionary<string, SubStateMachine> subStateMachines;
        public SubStateMachine activeSubStateMachine;
        public float stateTimer = 0.0f;

        public FiniteStateMachine()
        {
            subStateMachines = new Dictionary<string, SubStateMachine>();
            activeSubStateMachine = null;
        }

        public FiniteStateMachine(SubStateMachine[] subStateMachines, SubStateMachine activeSSM)
        {
            this.subStateMachines = new Dictionary<string, SubStateMachine>();
            foreach(SubStateMachine s in subStateMachines)
                this.subStateMachines.Add(s.name, s);
            activeSubStateMachine = activeSSM;
        }

        public void AddSubStateMachine(SubStateMachine subStateMachine)
        {
            subStateMachines.Add(subStateMachine.name, subStateMachine);
        }

        public void SetSubStateMachine(string subStateMachineName)
        {
            activeSubStateMachine = subStateMachines[subStateMachineName];
        }

        public SubStateMachine GetActiveSubStateMachine()
        {
            return activeSubStateMachine;
        }

        public int GetState(){
            return GetActiveSubStateMachine().GetActiveStateIndex();
        }

        public void SetState(int stateIndex)
        {
            activeSubStateMachine.activeStateIndex = stateIndex;
            stateTimer = 0.0f;
        }
        public void SetState(Enum state)
        {
            activeSubStateMachine.activeStateIndex = Convert.ToInt32(state);
            stateTimer = 0.0f;
        }

        public bool IsInState(int stateIndex){
            return GetState() == stateIndex;
        }
        public bool IsInState(Enum state){
            return GetState() == Convert.ToInt32(state);
        }

        public void Run()
        {
            if (activeSubStateMachine != null)
            {
                stateTimer += Time.deltaTime;
                //Any State Actions
                activeSubStateMachine.anyStateActions.Invoke();
                //Pre Transition Actions
                activeSubStateMachine.states[activeSubStateMachine.activeStateIndex].preTransitionActions.Invoke();
                //Transitions
                foreach (StateTransition s in activeSubStateMachine.states[activeSubStateMachine.activeStateIndex].stateTransitions)
                {
                    if (s.condition.Invoke())
                    {
                        if(s.stateIndex >= 0)
                            activeSubStateMachine.activeStateIndex = s.stateIndex;
                        s.transitionAction.Invoke();
                        stateTimer = 0.0f;
                        break;
                    }
                }
                //Post Transition Actions
                activeSubStateMachine.states[activeSubStateMachine.activeStateIndex].postTransitionActions.Invoke();
                //Sub State Machine Transitions
                foreach (SubStateMachineTransition s in activeSubStateMachine.states[activeSubStateMachine.activeStateIndex].subStateMachineTransitions)
                {
                    if (s.condition.Invoke())
                    {
                        s.transitionAction.Invoke();
                        activeSubStateMachine = subStateMachines[s.subStateMachineName];
                        activeSubStateMachine.activeStateIndex = s.stateIndex;
                        stateTimer = 0.0f;
                        break;
                    }
                }
            }
        }

        public float GetActiveStateTime()
        {
            return stateTimer;
        }
    }
}