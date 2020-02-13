using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkLine : Tools
{
    public GameObject hangPoint;
    public GameObject lineTracePrefab;
    private GameObject lineTrace;
    private LineRenderer line;
    private Vector3 resetPosition;
    private Vector3 hangPosition;
    private GameObject currentTouching;
    private bool ifConnect;
    private bool ifFlick;

    // Start is called before the first frame update
    void Start()
    {
        ifConnect = false;
        ifFlick = false;
        currentTouching = null;
        lineTrace = null;
        hangPosition = new Vector3(0, 0, 0);
        resetPosition = transform.position;

        line = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Show the line during the prepare phase
        if(ifConnect && !ifFlick)
        {
            line.SetPosition(0, hangPoint.transform.position);
            line.SetPosition(1, transform.position + resetPosition);
        }
        else if(ifConnect && ifFlick)
        {
            if(OVRInput.Get(OVRInput.RawButton.Y))
            {
                //Release the line
                lineTrace = null;
                //Return the hangPoint
                hangPoint.transform.parent = transform;
                hangPoint.transform.position = resetPosition;

                //Reset the flag
                ifConnect = false;
                ifFlick = false;
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
                //If connected but not flick yet, then try to flick
                else if(ifConnect && currentTouching != null && !ifFlick)
                {
                    //Instantiate the line prefab
                    lineTrace = Instantiate(lineTracePrefab);
                    //Transfer to the target object
                    lineTrace.transform.parent = currentTouching.transform;
                    //Set the start and end point
                    lineTrace.GetComponent<LineRenderer>().SetPosition(0, hangPosition);
                    lineTrace.GetComponent<LineRenderer>().SetPosition(1, transform.position + resetPosition);
                    //Update the status
                    ifFlick = true;
                }
            }
        }
    }
}
