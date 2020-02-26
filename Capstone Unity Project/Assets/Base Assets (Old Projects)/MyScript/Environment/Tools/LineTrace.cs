using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineTrace : MonoBehaviour
{
    private GameObject parent;
    private LineRenderer line;
    private bool ifStored;
    private List<Vector3> localPos;

    // Start is called before the first frame update
    void Start()
    {
        parent = null;
        line = null;
        ifStored = false;

        localPos = new List<Vector3>();
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

            //Try to store all the point
            if(line.positionCount > localPos.Count)
            {
                //Copy the new element into the List
                for(int i = localPos.Count; i < line.positionCount; i++)
                {
                    localPos.Add(line.GetPosition(i));
                }
            }

            //Modify all the line position
            for(int i = 0; i < line.positionCount; i++)
            {
                if (localPos.Count == line.positionCount)
                    line.SetPosition(i, localPos[i] + parent.transform.position);
                else
                    Debug.Log("Error! Stored position is not equal with line position!");
            }
        }
    }
}
