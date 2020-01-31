using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class CirSaw : Tools
{
    private GameObject targetObject;
    public GameObject Saw;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if being used by user
        if(ifHold)
        {
            //If the player hit A on the right controller
            if(OVRInput.Get(OVRInput.Button.One))
            {
                CutObject();
            }
        }
    }

    private void CutObject()
    {
        GameObject[] newChild = targetObject.SliceInstantiate(Saw.transform.position, Saw.transform.up);

        //Check if success
        if(newChild != null)
        {
            foreach(GameObject child in newChild)
            {
                child.transform.parent = null;
            }
        }
    }
}
