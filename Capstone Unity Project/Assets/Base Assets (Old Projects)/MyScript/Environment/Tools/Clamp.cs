using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clamp : Tools
{
    public GameObject baseRubber;
    public GameObject basePart;

    public GameObject moveRubber;
    public GameObject movePart;

    private GameObject  firstTarget;
    private GameObject  secondTarget;
    public GameObject   structureGroupPrefab;
    private GameObject  groupManager;

    private GameObject resetPosition;       //The reset position of the moving part
    private GameObject activePosition;      //The position of the moving part when initialize the clamp action
    private bool ifActive;

    // Start is called before the first frame update
    void Start()
    {
        //Get two rubber position

        //baseRubber = transform.Find("Head_Fixed").gameObject;
        //basePart = transform.Find("Clamp_Fixed").gameObject;

        //moveRubber = transform.Find("Head_Mobile").gameObject;
        //movePart = transform.Find("Clamp_Mobile").gameObject;

        resetPosition = transform.Find("Reset").gameObject;
        activePosition = transform.Find("Active").gameObject;

        ifActive = false;

        groupManager = null;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the clamp being used
        if(ifHold)
        {
            //Check if the button was pressed
            if (OVRInput.Get(OVRInput.RawButton.A))
            {
                //If the clamp was not opened yet
                if (!ifActive)
                {
                    //Open the clamp
                    movePart.transform.position = activePosition.transform.position;

                    //Set the flag
                    ifActive = true;
                }
                //If the clamp already opened
                else if(ifActive)
                {
                    //Ray cast to find the position of the head
                    RaycastHit hit;
                    Vector3 direction = moveRubber.transform.position - baseRubber.transform.position;
                    if (Physics.Raycast(baseRubber.transform.position, direction, out hit))
                    {
                        //Check if targeting a structure object
                        if (hit.collider.gameObject.tag == "Structure")
                        {
                            //Store the target structure object
                            firstTarget = hit.collider.gameObject;

                            //Calculate the target position
                            float moveDistance = Vector3.Distance(baseRubber.transform.position, hit.point);
                            Vector3 moveDirection = hit.point - baseRubber.transform.position;
                            moveDirection = Vector3.Normalize(moveDirection);

                            //Disable the grabber
                            gameObject.GetComponent<OVRGrabbable>().enabled = false;

                            //Move the object to the target location
                            transform.position += moveDirection * moveDistance;

                            //Ray cast to find the position of the mobile part
                            direction = baseRubber.transform.position - moveRubber.transform.position;
                            if (Physics.Raycast(moveRubber.transform.position, direction, out hit))
                            {
                                //Check if the second target is a structure object
                                if (hit.collider.gameObject.tag == "Structure")
                                {
                                    //Store the second object
                                    secondTarget = hit.collider.gameObject;
                                    
                                    //Calculate the target position
                                    moveDistance = Vector3.Distance(moveRubber.transform.position, hit.point);
                                    moveDirection = hit.point - moveRubber.transform.position;
                                    moveDirection = Vector3.Normalize(moveDirection);

                                    //Move the move part to the targe position
                                    movePart.transform.position += moveDirection * moveDistance;

                                    //Instantiate a structure group
                                    groupManager = Instantiate(structureGroupPrefab);
                                    groupManager.transform.parent = null;

                                    //Transfer everything into the structure group
                                    transform.parent = groupManager.transform;
                                    firstTarget.transform.parent = groupManager.transform;
                                    secondTarget.transform.parent = groupManager.transform;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
    }
}
