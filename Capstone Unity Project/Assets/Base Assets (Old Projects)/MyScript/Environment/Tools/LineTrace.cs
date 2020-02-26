using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineTrace : MonoBehaviour
{
    private GameObject parent;
    private LineRenderer line;
    private bool ifStored;
    private Vector3 localPos;

    // Start is called before the first frame update
    void Start()
    {
        parent = null;
        line = null;
        ifStored = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(parent == null)
        {
            //Try to get the parent
            parent = transform.parent.gameObject;
        }

        if(parent != null)
        {
            //Try to get the line
            if(line == null)
            {
                line = gameObject.GetComponent<LineRenderer>();
            }
        }
    }
}
