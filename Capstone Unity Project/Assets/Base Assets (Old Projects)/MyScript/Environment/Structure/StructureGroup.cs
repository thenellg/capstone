using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGroup : MonoBehaviour
{
    public List<Transform> childList;
    public List<GameObject> nailList;
    public OVRGrabbable grabScript;
    public Collider[] grabPoints;

    // Start is called before the first frame update
    void Start()
    {
        grabScript = gameObject.GetComponent<OVRGrabbable>();

        childList = new List<Transform>();

        nailList = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!grabScript.isActiveAndEnabled && grabPoints.Length != 0)
        {
            grabScript.enabled = true;
        }
    }

    void Update()
    {
        //Debug
        //Debug.Log(transform.childCount);
        //Debug

        //Put all the child into the tracking list
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
            {
                //Debug
                //Debug.Log("Child name: " + child.name);
                //Debug

                //Check if child in list already
                if(child.tag == "Structure")
                {
                    if(child.GetComponent<Structure>().trackingManager != this.gameObject)
                        childList.Add(child);
                }
                else if(child.name == "Nail")
                {
                    //If the new nail or not belong to this manager, set target
                    if (!GameObject.ReferenceEquals(child.GetComponent<Nail>().structureGroup, this.gameObject) &&
                        !child.GetComponent<Nail>().ifNailing)
                    {
                        childList.Add(child);
                        child.GetComponent<Nail>().structureGroup = this.gameObject;

                        //Check if contains fixed joint
                        FixedJoint childJoint = child.GetComponent<FixedJoint>();

                        //Debug
                        Debug.Log("Child nail joint: " + childJoint);
                        //Debug

                        if(childJoint != null)
                        {
                            //Debug
                            Debug.Log("Nail child have joint!");
                            //Debug

                            //Check if connected to this object
                            if(!GameObject.ReferenceEquals(childJoint.connectedBody.gameObject, this.gameObject))
                            {
                                childJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
                            }
                        }
                        else
                        {
                            //Debug
                            Debug.Log("Nail child no joint!");
                            //Debug

                            child.gameObject.AddComponent<FixedJoint>();
                            child.gameObject.GetComponent<FixedJoint>().connectedBody = GetComponent<Rigidbody>();
                        }
                    }
                }
            }
            else
            {
                //Debug
                //Debug.Log("Empty SG!");
                //Debug
            }
        }

        //Modify the child
        foreach (Transform child in childList)
        {
            //Exclude self
            if (!GameObject.ReferenceEquals(child.gameObject, gameObject))
            {
                //Debug
                //child.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 255);
                //Debug

                //Add fixed joint to the child
                if ((child.gameObject.GetComponent<Rigidbody>() != null &&
                    child.gameObject.GetComponent<FixedJoint>() == null))
                {
                    child.gameObject.AddComponent<FixedJoint>();
                    child.gameObject.GetComponent<FixedJoint>().connectedBody = GetComponent<Rigidbody>();
                }

                //Disable all the child's OVR grabble if exist
                if (child.gameObject.GetComponent<OVRGrabbable>() != null)
                {
                    //child.gameObject.GetComponent<OVRGrabbable>().enabled = false;
                    Destroy(child.gameObject.GetComponent<OVRGrabbable>());
                }

                //Tell them I'm the MASTER OF SCRIPT!
                if (child.gameObject.tag == "Structure")
                {
                    Structure childScript = child.gameObject.GetComponent<Structure>();
                    childScript.setManager(gameObject);
                }

                //Change the layer
                //child.gameObject.layer = LayerMask.NameToLayer("SG Object");
            }
        }

        //Set the grabble point to all the child collider
        grabPoints = gameObject.GetComponentsInChildren<Collider>();

        //Change the layer to ignore inner collision
        foreach(Collider currentC in grabPoints)
        {
            if (!GameObject.ReferenceEquals(currentC.gameObject, gameObject))
                currentC.gameObject.layer = LayerMask.NameToLayer("SG Object");
        }

        grabScript.NewGrabPoints(grabPoints);
    }
}
