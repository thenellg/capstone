using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meter : Tools
{
    public GameObject hangPoint;
    private Vector3 hangPosition;
    private Vector3 resetPosition;

    private LineRenderer line;

    private GameObject currentTouching;

    private bool ifConnect;

    // Start is called before the first frame update
    void Start()
    {
        ifConnect = false;
        currentTouching = null;
        hangPosition = new Vector3(0, 0, 0);
        resetPosition = transform.position;

        line = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Show the line during the prepare phase
        if (ifConnect)
        {
            line.SetPosition(0, hangPoint.transform.position);
            line.SetPosition(1, transform.position + resetPosition);

            if (OVRInput.Get(OVRInput.RawButton.Y))
            {
                //Return the hangPoint
                hangPoint.transform.parent = transform;
                hangPoint.transform.position = resetPosition;

                //Reset the flag
                ifConnect = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Try to find the structure object currently touching
        foreach(ContactPoint contact in collision.contacts)
        {
            if(contact.otherCollider.gameObject.tag == "Structure")
            {
                currentTouching = contact.otherCollider.gameObject;
                hangPosition = contact.point;
            }
        }

        //Check if being held by player
        if(ifHold)
        {
            //Check if the player trigger the button
            if(OVRInput.Get(OVRInput.RawButton.X) || OVRInput.Get(OVRInput.RawButton.A))
            {
                //If not connect to any of the object, then attach the hang point on the object
                if(!ifConnect && currentTouching != null)
                {
                    //Release the hang point, set parent to the touching object
                    hangPoint.transform.parent = currentTouching.transform;
                    //Lock the point
                    hangPoint.transform.position = hangPosition;
                    //Update the status
                    ifConnect = true;
                }
            }
        }
    }
}
