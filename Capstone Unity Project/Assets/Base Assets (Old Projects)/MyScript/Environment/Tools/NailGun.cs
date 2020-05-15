using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailGun : Tools
{
    private GameObject tip;
    public bool ifTriggered;

    public GameObject nailPrefab;

    public GameObject structureGroupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        tip = transform.GetChild(1).gameObject;

        ifTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if being hold by the user
        if(ifHold)
        {
            //If pulled the trigger
            if(OVRInput.GetDown(OVRInput.RawButton.A))
            {
                //Trigger once for to nail
                ifTriggered = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach(ContactPoint currentContact in collision.contacts)
        {
            //Check if contacting with a structure
            if(currentContact.thisCollider.name == "Tip" && currentContact.otherCollider.tag == "Structure")
            {
                //Check if the trigger being pulled
                if(ifTriggered)
                {
                    //Spawn a free nail
                    GameObject newNail = Instantiate(nailPrefab, null);
                    Nail newScript = newNail.GetComponent<Nail>();

                    //Reset the name
                    newNail.name = "Nail";

                    //Set the new nail's transformation
                    newNail.transform.position = tip.transform.position;
                    newNail.transform.rotation = tip.transform.rotation;
                    newNail.transform.Rotate(0.0f, -90.0f, 0.0f);

                    //Hold the nail
                    newScript.Use();

                    //Set status of nail
                    newScript.setTouch();
                    newScript.setNailed();

                    //Act to nail like a hammer
                    newScript.HitNail(3.0f, structureGroupPrefab);

                    //Nail the nail
                    newScript.NailFunction(currentContact);

                    //Act to nail like a hammer
                    //newNail.transform.Translate(newScript.getForward() * 8);

                    //Set status of nail
                    newScript.setTouch();
                    newScript.setNailed();

                    //Debug
                    //Debug.Break();
                    //Debug

                    //Reset the trigger, to make the shut only be once
                    ifTriggered = false;
                }
            }
        }
    }
}
