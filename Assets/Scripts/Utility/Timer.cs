using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    public float time;
    private float internalTime = 0.0f;
    private bool hasRunOut = false;
    
    public Timer(float startValue)
    {
        this.time = startValue;
    }

    public bool HasTimerRunOut()
    {
        if (!hasRunOut)
        {
            internalTime += Time.deltaTime;
            if(internalTime >= time)
            {
                hasRunOut = true;
            }
        }
        return hasRunOut;
    }

    public void Reset()
    {
        hasRunOut = false;
        internalTime = 0.0f;
    }

    public void SetTime(float time){
        internalTime = time;
    }

    public float GetElapsedTime()
    {
        return internalTime;
    }

    public float GetElapsedTimeNormalized()
    {
        return internalTime / time;
    }

    public float GetRemainingTime()
    {
        return time - internalTime;
    }

    public float GetRemainingTimeNormalized()
    {
        return (time - internalTime) / time;
    }

}
