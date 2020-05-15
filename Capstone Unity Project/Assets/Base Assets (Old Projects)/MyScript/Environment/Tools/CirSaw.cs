using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

//For this circular saw script, a third-party library called "Ezy-slice" was used
public class CirSaw : Tools
{
    private GameObject  currentObject;
    private GameObject  targetObject;
    public GameObject   cutPlane;
    public GameObject   Saw;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        targetObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if being used by user
        if(ifHold)
        {
            //If the player hit A on the right controller
            if (OVRInput.GetDown(OVRInput.RawButton.A))
            {
                //Get the target object when hit button
                targetObject = currentObject;

                CutObject();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Find the only one target structural object
        foreach(ContactPoint contact in collision.contacts)
        {
            //GameObject temp = FindStructureParent(contact.otherCollider.gameObject);

            if (contact.otherCollider.tag == "Structure")
            {
                currentObject = contact.otherCollider.gameObject;
            }
        }
    }

    private void CutObject()
    {
        //Release the target object first, then cut the slice
        targetObject.transform.parent = null;
        GameObject[] newChild = targetObject.SliceInstantiate(cutPlane.transform.position, cutPlane.transform.up);

        //Check if success
        if(newChild != null)
        {
            foreach(GameObject child in newChild)
            {
                //Free the new generated object
                //child.transform.position = this.transform.position;
                //child.transform.parent = null;

                //Change the layer and tag of the child
                child.layer = 9;
                child.tag = "Structure";

                //Give them rigid body and colliderbox
                child.AddComponent<Rigidbody>();
                MeshCollider childCollider = child.AddComponent<MeshCollider>();
                childCollider.convex = true;        //Set to be convex

                //Add grabber to the child object
                OVRGrabbable childGrabble = child.AddComponent<OVRGrabbable>();
                childGrabble.enabled = false;
                childGrabble.NewGrabPoints(child.GetComponents<Collider>());
                childGrabble.enabled = true;

                //Add structure group to the object
                child.AddComponent<Structure>();
            }

            //Check if target is a structure object
            if(targetObject.tag == "Structure")
            {
                //Check if target object was in a tructure group
                if(targetObject.GetComponent<Structure>().trackingManager)
                {
                    //Set target manager to update
                    targetObject.GetComponent<Structure>().trackingManager.GetComponent<StructureGroup>().ifNeedUpdate = true;
                }
            }

            //Remove old object
            Destroy(targetObject);
        }
    }
}
