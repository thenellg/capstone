using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGroup : MonoBehaviour
{
    private Transform[] childList;
    public OVRGrabbable grabScript;
    public Collider[] grabPoints;

    // Start is called before the first frame update
    void Start()
    {
        grabScript = gameObject.GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!grabScript.isActiveAndEnabled && grabPoints.Length != 0)
        {
            grabScript.enabled = true;
        }
    }

    void FixedUpdate()
    {
        //Debug.Log("LateUpdate executed!");

        //Put all the child into the tracking list
        childList = GetComponentsInChildren<Transform>();

        //Modify the child
        foreach (Transform child in childList)
        {
            //Exclude self
            if (!GameObject.ReferenceEquals(child.gameObject, gameObject))
            {
                //Debug
                child.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                //Debug

                //Disable all the child's rigidbody
                if (child.gameObject.GetComponent<Rigidbody>() != null)
                {
                    //child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    //child.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    Destroy(child.gameObject.GetComponent<Rigidbody>());
                }

                    //Diable all the child's OVR grabble if exist
                    //if (child.gameObject.GetComponent<OVRGrabbable>() != null)
                    //Destroy(child.gameObject.GetComponent<OVRGrabbable>());

                    //Tell them I'm the MASTER OF SCRIPT!
                if (child.gameObject.tag == "Structure")
                {
                    Structure childScript = child.gameObject.GetComponent<Structure>();
                    childScript.setManager(gameObject);
                }
            }
        }

        //Set the grabble point to all the child collider
        grabPoints = gameObject.GetComponentsInChildren<Collider>();
        grabScript.NewGrabPoints(grabPoints);
    }
}
