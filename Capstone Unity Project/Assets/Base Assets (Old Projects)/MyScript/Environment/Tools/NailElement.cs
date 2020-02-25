﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailElement : MonoBehaviour
{
    protected Nail parent;
    protected bool ifNailed;

    // Start is called before the first frame update
    void Start()
    {
        parent = null;
        ifNailed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignParent(Nail newParent)
    {
        if (parent == null)
        {
            parent = newParent;
        }
    }

    public void StartNail()
    {
        ifNailed = true;
    }
}