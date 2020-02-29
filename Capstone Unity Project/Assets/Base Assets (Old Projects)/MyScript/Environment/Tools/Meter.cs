using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meter : Tools
{
    public GameObject hangPoint;
    private GameObject resetObj;
    private GameObject resetPos;
    private Vector3 hangPosition;

    public GameObject measurePrefab;
    private GameObject measureObject;
    private Text measure;

    private LineRenderer line;

    private GameObject currentTouching;

    private bool ifConnect;

    // Start is called before the first frame update
    void Start()
    {
        ifConnect = false;
        currentTouching = null;
        hangPosition = new Vector3(0, 0, 0);

        line = gameObject.GetComponent<LineRenderer>();

        resetObj = transform.Find("Reset").gameObject;
        resetPos = transform.Find("ResetPos").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Show the line during the prepare phase
        if (ifConnect && ifHold)
        {
            //If pressed button, then reset everything
            if (OVRInput.Get(OVRInput.RawButton.Y))
            {
                //Return the hangPoint
                hangPoint.transform.position = resetPos.transform.position;
                hangPoint.transform.rotation = resetObj.transform.rotation;
                hangPoint.transform.parent = transform;

                //Reset the flag
                ifConnect = false;

                //Reset the line
                line.SetPosition(0, new Vector3(0, 0, 0));
                line.SetPosition(1, new Vector3(0, 0, 0));

                //Destroy the measure
                Destroy(measureObject);
            }
            //If not pressed, then update the whole line
            else
            {
                //Update the line
                line.SetPosition(0, resetObj.transform.position);
                line.SetPosition(1, hangPoint.transform.position);

                //Update the measure
                measureObject.transform.position = transform.position + new Vector3(0.2f, 0.1f, 0);
                float distance = Vector3.Distance(hangPoint.transform.position, resetObj.transform.position);
                measure.text = distance.ToString("0.00");
                measureObject.transform.rotation = transform.rotation;
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
        if(ifHold)
        {
            //Check if the player trigger the button
            if(OVRInput.Get(OVRInput.RawButton.X) || OVRInput.Get(OVRInput.RawButton.A))
            {
                //If not connect to any of the object, then attach the hang point on the object
                if (!ifConnect && currentTouching != null)
                {
                    //Lock the point
                    hangPoint.transform.position = hangPosition;

                    //Release the hang point, set parent to the touching object
                    hangPoint.transform.parent = currentTouching.transform;

                    //Update the status
                    ifConnect = true;

                    //Create the line
                    line.SetPosition(0, resetObj.transform.position);
                    line.SetPosition(1, hangPoint.transform.position);

                    //Instantiate the measure
                    measureObject = Instantiate(measurePrefab, GameObject.Find("Canvas").transform);
                    measureObject.transform.position = transform.position + new Vector3(0.5f, 0.1f, 0);
                    measure = measureObject.GetComponent<Text>();
                    float distance = Vector3.Distance(hangPoint.transform.position, resetObj.transform.position);
                    measure.text = distance.ToString();
                }
            }
        }
    }
}
