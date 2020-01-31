﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Tools : MonoBehaviour
{
    protected bool ifHold;

    // Start is called before the first frame update
    void Start()
    {
        ifHold = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual public void Use()
    {
        ifHold = true;
    }

    virtual public void StopUse()
    {
        ifHold = false;
    }

}
