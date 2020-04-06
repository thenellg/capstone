﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailElement : MonoBehaviour
{
    protected Nail parent;
    protected bool ifNailed;
    protected GameObject childStructureGroup;       //The current structure group object
    protected GameObject structureGroupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        parent = null;
        childStructureGroup = null;
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

    public void StartNail(GameObject SGPrefab)
    {
        ifNailed = true;
        structureGroupPrefab = SGPrefab;
    }
}
