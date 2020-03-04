using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHandS : OVRGrabber
{
    private GameObject lastObject;
    private GameObject currentObject;
    private Tools currentScript;

    public GameObject nailPre, studPre;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        lastObject = null;
        currentObject = null;
        currentScript = null;
    }

    public override void Update()
    {
        base.Update();

        //Generate new nail when pressed button
        if(OVRInput.GetDown(OVRInput.RawButton.X) && m_grabbedObj == null && gameObject.name == "CustomHandLeft")
        {
            GameObject newNail = Instantiate(nailPre);
            newNail.name = "Nail";
            newNail.transform.parent = null;
            newNail.transform.position = transform.position;
        }

        //Generate new stud when pressed button
        if (OVRInput.GetDown(OVRInput.RawButton.A) && m_grabbedObj == null && gameObject.name == "CustomHandRight")
        {
            GameObject newStud = Instantiate(studPre);
            newStud.name = "Stud";
            newStud.transform.parent = null;
            newStud.transform.position = transform.position;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //Check if the hand is holding something
        if(m_grabbedObj != null)
        {
            //Debug
            //Debug.Log("Holding Something!");
            //Debug

            //Check if grabing a tool object   
            GameObject tempObj = FindToolParent(m_grabbedObj.gameObject);
            if(tempObj != null && tempObj.tag == "Tools")
            {
                //Debug
                //Debug.Log("Holding Tool!");
                //Debug

                //Get the tools 
                currentObject = m_grabbedObj.gameObject;
                lastObject = currentObject;

                //Get the access to the script
                currentScript = currentObject.GetComponent<Tools>();
                if(currentScript != null)
                {
                    //Debug
                    //Debug.Log("Access Tool Script!");
                    //Debug

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

    private GameObject FindToolParent(GameObject child)
    {
        if (child.layer == 9 && child.tag != "Tools")
        {
            return FindToolParent(child.transform.parent.gameObject);
        }
        else if (child.layer == 9 && child.tag == "Tools")
        {
            return child;
        }
        return null;
    }
}
