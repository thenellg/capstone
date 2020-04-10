using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkLine : Tools
{
    public GameObject hangPoint;
    private GameObject resetObj;
    private GameObject resetPos;
    private Vector3 hangPosition;

    public GameObject lineTracePrefab;
    private GameObject lineTrace;
    private LineRenderer line;

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

        line = gameObject.GetComponent<LineRenderer>();

        resetObj = transform.Find("Reset").gameObject;
        resetPos = transform.Find("ResetPos").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Show the line during the prepare phase
        if(ifConnect && ifHold && !ifFlick)
        {
            //Update the line
            line.SetPosition(0, resetObj.transform.position);
            line.SetPosition(1, hangPoint.transform.position);
        }
        else if(ifConnect && ifFlick && ifHold)
        {
            if(OVRInput.Get(OVRInput.RawButton.Y))
            {
                //Release the line
                lineTrace = null;

                //Return the hangPoint
                hangPoint.transform.position = resetPos.transform.position;
                hangPoint.transform.rotation = resetPos.transform.rotation;
                hangPoint.transform.parent = transform;
                hangPoint.transform.localScale = new Vector3(1, 1, 1);

                //Reset the flag
                ifConnect = false;
                ifFlick = false;

                //Reset the line
                line.SetPosition(0, new Vector3(0, 0, 0));
                line.SetPosition(1, new Vector3(0, 0, 0));
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Try to find the structure object currently touching
        foreach(ContactPoint contact in collision.contacts)
        {
            if(contact.otherCollider.gameObject.tag == "Structure" &&
                contact.thisCollider.gameObject.name == "Oggetto_7")
            {
                currentTouching = contact.otherCollider.gameObject;
                hangPosition = contact.point;
            }
        }

        //Check if being held by player
        if (ifHold)
        {
            //Check if the player trigger the button
            if (OVRInput.Get(OVRInput.RawButton.X) || OVRInput.Get(OVRInput.RawButton.A))
            {
                //If not connect to any of the object, then attach the hang point on the object
                if (!ifConnect && currentTouching != null)
                {
                    //Release the hang point, set parent to the touching object
                    hangPoint.transform.parent = currentTouching.transform;

                    //Lock the point
                    hangPoint.transform.position = hangPosition;
                    hangPoint.transform.localScale = new Vector3(1, 1, 1);

                    //Update the status
                    ifConnect = true;

                    //Update the line
                    line.SetPosition(0, resetObj.transform.position);
                    line.SetPosition(1, hangPoint.transform.position);
                }
            }
            else if (OVRInput.Get(OVRInput.RawButton.Y))
            {
                //If connected but not flick yet, then try to flick
                if (ifConnect && currentTouching != null && !ifFlick)
                {
                    //Instantiate the line prefab
                    lineTrace = Instantiate(lineTracePrefab);

                    //Set the start and end point
                    lineTrace.GetComponent<LineRenderer>().SetPosition(0, hangPoint.transform.position);
                    lineTrace.GetComponent<LineRenderer>().SetPosition(1, hangPosition);
                    //Transfer to the target object
                    lineTrace.transform.parent = currentTouching.transform;

                    //Update the status
                    ifFlick = true;
                }
            }
        }
    }
}
