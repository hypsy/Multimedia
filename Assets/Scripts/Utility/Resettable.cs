using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resettable<T>
{
    public T initialValue;
    public T value;
    public Resettable(T initialValue)
    {
        this.initialValue = initialValue;
        this.value = value;
    }

    public void Reset()
    {
        this.value = this.initialValue;
    }

    public void SetValue(T value)
    {
        this.value = value;
    }

    public T GetValue()
    {
        return this.value;
    }

    public T GetInitialValue()
    {
        return this.initialValue;
    }
}

[System.Serializable]
public class ResettableInt : Resettable<int> {
    public ResettableInt(int value) : base(value){
        this.initialValue = value;
        this.value = value;
    }
    public static ResettableInt operator +(ResettableInt lhs, int rhs)
    {
        lhs.SetValue(lhs.GetValue() + rhs);
        lhs.Clamp();
        return lhs;
    }
    public static ResettableInt operator -(ResettableInt lhs, int rhs)
    {
        lhs.SetValue(lhs.GetValue() - rhs);
        lhs.Clamp();
        return lhs;
    }
    public float GetPercentage()
    {
        return (float)value / (float)initialValue;
    }
    public void Clamp(){
        this.value = Mathf.Clamp(this.value, 0, this.initialValue);   
    }
}
[System.Serializable]
public class ResettableFloat : Resettable<float> {
    public ResettableFloat(float value) : base(value){ }
    public static ResettableFloat operator +(ResettableFloat lhs, float rhs)
    {
        lhs.SetValue(lhs.GetValue() + rhs);
        return lhs;
    }
    public static ResettableFloat operator -(ResettableFloat lhs, float rhs)
    {
        lhs.SetValue(lhs.GetValue() - rhs);
        return lhs;
    }
    public float GetPercentage()
    {
        return value / initialValue;
    }
}
