using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Stat  {
    [SerializeField]
    private BarScript bar;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float currentVal;

    public float CurrentVal
    {
        get
        {
            return currentVal;
        }

        set
        {
           
            this.currentVal =Mathf.Clamp( value, 0 , MaxValue);
            bar.Value = currentVal;
        }
    }

    public float MaxValue
    {
        get
        {
            return maxValue;
        }

        set
        {
            maxValue = value;
            bar.MaxValue = maxValue;
        }
    }
    public void Initialize()
    {
        this.MaxValue = maxValue;
        this.CurrentVal = currentVal;
    }
}
