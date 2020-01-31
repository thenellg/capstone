using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHandS : OVRGrabber
{
    private GameObject lastObject;
    private GameObject currentObject;
    private Tools currentScript;

    protected override void Start()
    {
        base.Start();

        lastObject = null;
        currentObject = null;
        currentScript = null;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //Check if the hand is holding something
        if(m_grabbedObj != null)
        {
            //Check if grabing a tool object       
            if(checkTag("Tools", LayerMask.NameToLayer("Environmental"), m_grabbedObj.gameObject))
            {
                //Get the tools 
                currentObject = m_grabbedObj.gameObject;
                lastObject = currentObject;

                //Get the access to the script
                currentScript = currentObject.GetComponent<Tools>();
                if(currentScript != null)
                {
                    //Set the tool to be use
                    currentScript.Use();
                }
            }
        }
        //If hand is not holding anything
        else
        {
            //Check if just droped
            if(lastObject != null)
            {
                //Stop using the tool
                currentScript.StopUse();

                //Release the object
                lastObject = null;
                currentObject = null;
                currentScript = null;
            }
        }
    }

    private bool checkTag(string tag, LayerMask layer, GameObject current)
    {
        //Check if still in the layer
        if(current.layer.Equals(layer))
        {
            if(current.tag != tag)
            {
                return checkTag(tag, layer, current.transform.parent.gameObject);
            }
            return true;
        }
        return false;
    }
}
