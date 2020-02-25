﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : Tools
{
    private bool ifNailed;              //If the nail been connected to a structural object
    private bool ifTouching;
    private bool ifFreeze;
    private GameObject head, middle, end, header, ender;

    private GameObject structureGroupPrefab;
    private GameObject structureGroup;      //The list of connected object
    public List<GameObject> connected;

    private LineRenderer mLine;
    private Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        head = transform.GetChild(1).gameObject;
        middle = transform.GetChild(2).gameObject;
        end = transform.GetChild(3).gameObject;
        header = transform.GetChild(4).gameObject;
        ender = transform.GetChild(5).gameObject;

        ifNailed = false;
        ifTouching = false;
        ifFreeze = false;

        mLine = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ifTouching && !ifNailed)
        {//Update the forward vector
            forward = head.transform.position - end.transform.position;
            Vector3.Normalize(forward);

            //Cast a ray to check if pointing at structure object
            RaycastHit hit;
            Ray ray = new Ray(head.transform.position, forward);
            if (Physics.Raycast(ray, out hit, 2.0f))
            {
                if (hit.collider.tag == "Structure")
                {
                    mLine.SetPosition(0, head.transform.position);
                    mLine.SetPosition(1, hit.point);
                }
            }
            else      //If hit nothing
            {
                mLine.SetPosition(0, head.transform.position);
                mLine.SetPosition(1, head.transform.position);
            }
        }

        //If the user enter X now, freez the nail
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            ifFreeze = !ifFreeze;
        }

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            if (ifFreeze)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
    }

    //Called when a hammer hit nail (call by hammer)
    public void HitByHammer(Vector3 directionHit, GameObject managerPrefab)
    {
        //Debug
        //transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255, 0, 255);
        //Debug

        //Receive the prefab
        structureGroupPrefab = managerPrefab;

        //Check if the head was touching something
        if (ifTouching == true)
        {
            //Debug
            //transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255, 0, 255);
            //Debug

            float amount = Vector3.Dot(directionHit, forward);

            //Move the nail based on direction
            if(!ifNailed)
                ifNailed = true;
            transform.position += forward * amount;

            //Connect the nail with other object


        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug
        Debug.Log("Nail Collision Enter");
        //Debug
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug
        Debug.Log(collision.contactCount);
        //Debug

        foreach(ContactPoint contact in collision.contacts)
        {
            //Check if the head is colliding with an structural object
            if(contact.thisCollider.gameObject.name == "Head")
            {
                //Debug
                //transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255, 255, 0);
                //Debug.Log(contact.otherCollider.gameObject.tag);
                //Debug

                if (contact.otherCollider.gameObject.tag == "Structure")
                {
                    ifTouching = true;

                    //Debug
                    //transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                    //Debug

                    /*****Start to connect structure object*****/

                    //Get the target object
                    GameObject currentTarget = contact.otherCollider.gameObject;

                    //Debug
                    currentTarget.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                    //Debug

                    //Check if start to nail
                    if (ifNailed)
                    {
                        //Debug
                        transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                        //Debug

                        //Create a new group if don't have one
                        if (structureGroup == null)
                        {
                            //Generate the structure group manager to handle nailed object
                            structureGroup = GameObject.Instantiate(structureGroupPrefab);
                            structureGroup.transform.position = transform.position;
                            structureGroup.transform.parent = null;

                            //Debug
                            Debug.Log("SG Created");
                            //Debug

                            //Debug
                            currentTarget.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                            //Debug

                            //Add the nail and target into the same group
                            transform.parent = structureGroup.transform;
                            currentTarget.transform.parent = structureGroup.transform;

                            //Put the first structure object into the connected list

                            if(currentTarget != null)
                                connected.Add(currentTarget);
                        }
                        else if(structureGroup)      //If already exist a connected group
                        {
                            //Debug
                            currentTarget.GetComponent<Renderer>().material.color = new Color(255, 255, 255);
                            //Debug

                            //Check if the strcuture object already in the connect list
                            bool ifInside = false;
                            foreach(GameObject connect in connected)
                            {
                                if(GameObject.ReferenceEquals(currentTarget, connect))
                                {
                                    ifInside = true;
                                    break;
                                }
                            }
                            if(!ifInside)
                            {
                                //Debug
                                Debug.Log("Nail Adding Object");
                                //Debug

                                //Add the target into the group
                                currentTarget.transform.parent = structureGroup.transform;

                                //Put the target into the connect group
                                connected.Add(currentTarget);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug
        //Debug.Log("Head Leave!");
        //Debug
    }
}
