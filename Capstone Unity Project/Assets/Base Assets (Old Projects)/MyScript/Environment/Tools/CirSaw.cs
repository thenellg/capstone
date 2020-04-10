using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class CirSaw : Tools
{
    private GameObject currentObject;
    private GameObject targetObject;
    public GameObject cutPlane;
    public GameObject Saw;

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
            GameObject temp = FindStructureParent(contact.otherCollider.gameObject);

            if (temp != null)
                currentObject = temp;
        }
    }

    private void CutObject()
    {
        GameObject[] newChild = targetObject.SliceInstantiate(cutPlane.transform.position, cutPlane.transform.up);

        //Check if success
        if(newChild != null)
        {
            //Remove old object
            Destroy(targetObject);

            foreach(GameObject child in newChild)
            {
                //Free the new generated object
                child.transform.parent = null;

                //Change the layer and tag of the child
                child.layer = 9;
                child.tag = "Structure";

                //Give them rigid body and colliderbox
                child.AddComponent<Rigidbody>();
                MeshCollider childCollider = child.AddComponent<MeshCollider>();
                childCollider.convex = true;        //Set to be convex

                //Add grabber to the child object
                OVRGrabbable childGrabble = child.AddComponent<OVRGrabbable>();
                childGrabble.NewGrabPoints(child.GetComponents<Collider>());
            }
        }
    }
}
