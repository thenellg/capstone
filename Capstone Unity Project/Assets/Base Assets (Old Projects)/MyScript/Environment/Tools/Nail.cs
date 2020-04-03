using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : Tools
{
    private bool ifNailed;              //If the nail been connected to a structural object
    private bool ifTouching;
    private bool ifFreeze;
    public bool ifNailing;
    private GameObject head, end;

    private GameObject structureGroupPrefab;
    public GameObject structureGroup;       //The current structure group object
    public List<GameObject> connected;      //The list of connected object

    private LineRenderer mLine;
    private Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        head = transform.GetChild(1).gameObject;
        end = transform.GetChild(2).gameObject;

        ifNailed = false;
        ifTouching = false;
        ifFreeze = false;
        ifNailing = false;

        mLine = GetComponent<LineRenderer>();

        connected = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!ifTouching && !ifNailed)
        {
            //Update the forward vector
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
                else
                {
                    mLine.SetPosition(0, head.transform.position);
                    mLine.SetPosition(1, head.transform.position);
                }
            }
            else      //If hit nothing
            {
                mLine.SetPosition(0, head.transform.position);
                mLine.SetPosition(1, head.transform.position);
            }
        }
        else if(!ifTouching)
        {
            mLine.SetPosition(0, head.transform.position);
            mLine.SetPosition(1, head.transform.position);
        }

        //If the user enter X now, freez the nail
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            ifFreeze = !ifFreeze;
        }

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            if (ifFreeze && !ifNailed)
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
        //Set the flag
        ifNailing = true;

        //Receive the prefab
        structureGroupPrefab = managerPrefab;

        //Disable the collision
        GetComponent<Rigidbody>().isKinematic = true;

        //Check if the head was touching something
        if (ifTouching)
        {
            //Check if contains a fixed joint, then remove it if there is one
            FixedJoint current = null;
            Rigidbody connectedManager = null;
            current = GetComponent<FixedJoint>();
            bool needAddBack = false;

            if(current != null)
            {
                //Debug
                Debug.Log("Current joint(current) before nailing: " + current);
                //Debug

                connectedManager = current.connectedBody;

                //Debug
                if (connectedManager == null)
                    Debug.Log("Worning! Connect 0 in function!");
                //Debug

                Destroy(GetComponent<FixedJoint>());

                needAddBack = true;
            }

            float amount = Vector3.Dot(directionHit, forward);

            //Move the nail based on direction
            if (!ifNailed)
            {
                ifNailed = true;
            }
            transform.Translate(forward.normalized * Time.deltaTime, Space.World);

            //Debug
            Debug.Log("Current joint after nailing: " + this.transform.GetComponent<FixedJoint>());
            //Debug

            //Put fixed joint back if there is one, and not added by structure group
            if(GetComponent<FixedJoint>() == null && needAddBack)
            {
                gameObject.AddComponent<FixedJoint>();
                if(structureGroup != null)
                    GetComponent<FixedJoint>().connectedBody = structureGroup.GetComponent<Rigidbody>();
                else if(connectedManager != null)
                    GetComponent<FixedJoint>().connectedBody = connectedManager;
                else
                {
                    //Debug
                    Debug.Log("Nailing process error, final adding step!");
                    //Debug
                }
            }
        }

        //Enable the collision
        GetComponent<Rigidbody>().isKinematic = false;

        //Remove the flag
        ifNailing = false;

        //Debug
        if (gameObject.GetComponent<FixedJoint>().connectedBody == null)
            Debug.Log("Worning! Connect 0!");
        //Debug
    }

    private void OnCollisionEnter(Collision collision)
    {

    }


    /*****
     * There are 4 major situations:
     * 1. Nail no group, target no group
     * 2. Nail have group, target no group
     * 3. Nail no group, target have group
     * 4. Nail have group, target have group, and they aren't same
     * 5. Nail have group, target have group, they are same
     ******/
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

                if (contact.otherCollider.gameObject.tag == "Structure")
                {
                    ifTouching = true;

                    /*****Start to connect structure object*****/

                    //Get the target object
                    GameObject currentTarget = contact.otherCollider.gameObject;
                    Structure currentTargetScript = currentTarget.GetComponent<Structure>();

                    //Check if the target already be nailed
                    bool needNail = true;
                    foreach(GameObject conect in connected)
                    {
                        if (GameObject.ReferenceEquals(conect, currentTarget))
                        {
                            needNail = false;
                            break;
                        }
                        else
                        {
                            //Debug
                            currentTarget.GetComponent<Renderer>().material.color = new Color(0, 255, 255);
                            //Debug
                        }
                    }

                    //Check if start to nail
                    if (ifNailed && needNail)
                    {
                        //If nail doesn't belongs to a structure group, and target object also
                        if (structureGroup == null && 
                            currentTargetScript.trackingManager == null)
                        {
                            //Generate the structure group manager to handle nailed object
                            structureGroup = Instantiate(structureGroupPrefab);
                            structureGroup.transform.position = transform.position;
                            structureGroup.transform.parent = null;

                            //Add the nail and target into the same group
                            transform.parent = structureGroup.transform;
                            currentTarget.transform.parent = structureGroup.transform;
                            structureGroup = null;      //Transfer the task to SG script

                            //Put the first structure object into the connected list
                            if(currentTarget != null)
                                connected.Add(currentTarget);

                            //Debug
                            Debug.Log("Nailing case 1");
                            //Debug
                        }
                        //If nail doesn't belongs to group but target does
                        else if(structureGroup == null && 
                            currentTargetScript.trackingManager != null)
                        {
                            //Set the structure group the same as the current target
                            structureGroup = currentTargetScript.trackingManager;

                            //Transfer as a child to the structure group
                            transform.parent = structureGroup.transform;

                            //Put the structure object into the connected list
                            if (currentTarget != null)
                                connected.Add(currentTarget);

                            //Debug
                            Debug.Log("Nailing case 2");
                            //Debug
                        }
                        //If already exist a connected group, but target doesn't
                        else if (structureGroup != null && 
                            currentTargetScript.trackingManager == null)
                        {
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
                                //Add the target into the group
                                currentTarget.transform.parent = structureGroup.transform;

                                //Put the target into the connect group
                                connected.Add(currentTarget);
                            }

                            //Debug
                            Debug.Log("Nailing case 3");
                            //Debug
                        }
                        //If connecting two structure group, not same
                        else if (structureGroup != null && 
                            currentTargetScript.trackingManager != null &&
                            !GameObject.ReferenceEquals(currentTargetScript.trackingManager, structureGroup))
                        {
                            //Connect with target object first
                            if (currentTarget != null)
                                connected.Add(currentTarget);

                            //Move all the obejct for other manager group to this one
                            GameObject targetManager = currentTargetScript.trackingManager;
                            foreach(Transform targetChild in targetManager.transform)
                            {
                                //If target contains rigidbody, then that means it's the root, for double check
                                if(targetChild.GetComponent<Rigidbody>() != null)
                                {
                                    //Move the target into current manager
                                    targetChild.parent = structureGroup.transform;
                                }
                            }

                            //After finish, check if target structure group contains no more object
                            if(targetManager.transform.childCount == 0)
                            {
                                //Destroy(targetManager);
                            }

                            //Debug
                            Debug.Log("Nailing case 4");
                            //Debug
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {

    }
}

