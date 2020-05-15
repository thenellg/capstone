using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : Tools
{
    public bool ifNailed;              //If the nail been connected to a structural object
    public bool ifTouching;
    public bool ifFreeze;
    public bool ifNailing;
    public GameObject head, end;       //The game object for head and end child
    private NailHead headScript;
    private NailEnd endScript;

    private GameObject structureGroupPrefab;
    public GameObject structureGroup;       //The current structure group object
    public List<GameObject> connected;      //The list of connected object

    private LineRenderer mLine;
    private Vector3 forward;

    public bool iterated;

    // Start is called before the first frame update
    void Start()
    {
        //Debug
        Debug.Log("Nail script initializing!");
        //Debug

        //Get the access to head and end and get their script
        setComponent();

        if(!ifNailed)
            ifNailed = false;
        if(!ifTouching)
            ifTouching = false;
        ifFreeze = false;
        ifNailing = false;

        mLine = GetComponent<LineRenderer>();

        if(connected.Count == 0)
            connected = new List<GameObject>();

        iterated = false;
    }

    public void setComponent()
    {
        head = transform.GetChild(1).gameObject;
        if (head != null)
        {
            headScript = head.GetComponent<NailHead>();
            if (headScript != null)
            {
                headScript.AssignParent(this);
            }
        }

        end = transform.GetChild(2).gameObject;
        if (end != null)
        {
            endScript = end.GetComponent<NailEnd>();
            if (endScript != null)
            {
                endScript.AssignParent(this);
            }
        }
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
        else if (!ifTouching)
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

        //Update the parent
        if(structureGroup)
            transform.parent = structureGroup.transform;
    }

    private void Update()
    {
        if(structureGroup != null)
        {
            headScript.childStructureGroup = structureGroup;
            endScript.childStructureGroup = structureGroup;
        }

        //Update the connected list
        for(int i = 0; i < connected.Count; i++)
        {
            //Delete the invalid(null) connected object
            if(connected[i] == null)
            {
                //Debug
                Debug.Log("Delete invalid connected object!");
                //Debug

                connected.RemoveAt(i);
            }
        }
    }


    public void hammerHitFunc(float amount)
    {
        //Debug
        Debug.Log("hammer hit func called!" + (ifTouching||ifNailed));
        //Debug

        //Check if the head was touching something
        if (ifTouching || ifNailed)
        {
            //Make the nail to be flagged as nailed
            if (!ifNailed && ifHold)
            {
                ifNailed = true;
            }

            //Update the forward vector
            setComponent();
            forward = head.transform.position - end.transform.position;
            Vector3.Normalize(forward);


            //Debug
            Debug.Log("Move the nail forward! " + forward.normalized);
            //Debug

            //Move the nail based on direction
            transform.Translate(forward.normalized * Time.deltaTime * amount, Space.World);

            //Debug
            //Debug.Log("Current joint after nailing: " + this.transform.GetComponent<FixedJoint>());
            //Debug
        }
    }

    //Called when a hammer hit nail (call by hammer)
    public void HitNail(float amount, GameObject managerPrefab)
    {
        //Debug
        Debug.Log("Nail hit function called");
        //Debug

        //Receive the prefab
        structureGroupPrefab = managerPrefab;

        hammerHitFunc(amount);
    }

    public void NailFunction(ContactPoint contact)
    {
        /*****Start to connect structure object*****/

        //Get the target object
        GameObject currentTarget = contact.otherCollider.gameObject;
        Structure currentTargetScript = currentTarget.GetComponent<Structure>();

        //Debug
        Debug.Log("===== Nailing Info =====");
        Debug.Log("Triggered nailed!");
        Debug.Log("Contact object: " + currentTarget);
        Debug.Log("Connection check: " + ifConnected(currentTarget));
        Debug.Log("If allowed to nail: " + ifNailed);
        Debug.Log("===== Nailing Info =====");
        //Debug

        //Check if start to nail
        if (ifNailed && !ifConnected(currentTarget))
        {
            /*****Start 2 cases*****/

            if (currentTarget.tag == "Structure")
            {
                //Debug
                Debug.Log("===== Nail target Info =====");
                Debug.Log("Structure stataus: " + currentTarget.GetComponent<Structure>().trackingManager);
                Debug.Log("===== Nail target Info =====");
                //Debug

                //Try to remove the nail's OVRGrabbale
                if (gameObject.GetComponent<OVRGrabbable>())
                {
                    //gameObject.GetComponent<OVRGrabbable>().enabled = false;
                    Destroy(gameObject.GetComponent<OVRGrabbable>());
                }

                //Try to remove the nail's rigidbody
                if (gameObject.GetComponent<Rigidbody>())
                {
                    Destroy(gameObject.GetComponent<Rigidbody>());
                }

                //If nail doesn't belongs to a structure group, and target object also not
                if (currentTargetScript.trackingManager == null)
                {
                    //Create a new structure group
                    if (structureGroup == null)
                    {
                        //Generate the structure group manager to handle nailed object
                        structureGroup = Instantiate(structureGroupPrefab);
                        structureGroup.transform.position = transform.position;
                        structureGroup.transform.parent = null;

                        //Debug
                        //Debug.Log("=====");
                        //Debug.Log("Parent Set!");
                        //Debug
                    }


                    /*****Editing Target object*****/

                    //Connect the target object into the group, and add to child list
                    currentTarget.transform.parent = structureGroup.transform;
                    //structureGroup.GetComponent<StructureGroup>().AddChild(currentTarget.transform);
                    if (currentTarget.GetComponent<OVRGrabbable>())
                    {
                        //currentTarget.GetComponent<OVRGrabbable>().enabled = false;
                        Destroy(currentTarget.GetComponent<OVRGrabbable>());
                    }

                    //Try to remove the target's rigidbody
                    if (currentTarget.GetComponent<Rigidbody>())
                    {
                        Destroy(currentTarget.GetComponent<Rigidbody>());
                    }

                    //Set the tracking manager
                    currentTarget.GetComponent<Structure>().trackingManager = structureGroup.gameObject;


                    /*****Editing This object*****/

                    //Connect this nail into the group and add into child list
                    transform.parent = structureGroup.transform;
                    //structureGroup.GetComponent<StructureGroup>().AddChild(transform);

                    //Debug
                    Debug.Log("Nailing case 1");
                    //Debug
                }
                //If target is in a structure group
                else
                {
                    //Set the structure group the same as the current target
                    structureGroup = currentTargetScript.trackingManager;

                    //Transfer as a child to the structure group and add to child list
                    transform.parent = structureGroup.transform;
                    //structureGroup.GetComponent<StructureGroup>().AddChild(transform);

                    //Debug
                    Debug.Log("Nailing case 2");
                    //Debug
                }

                //Put the structure object into the connected list and inverse too
                if (currentTarget != null)
                {
                    //Debug
                    Debug.Log("Connecting nail!");
                    //Debug

                    addToConnect(currentTarget);
                    currentTarget.GetComponent<Structure>().AddConnect(gameObject);

                    //Debug
                    Debug.Log("Connecting nail status: " + connected.Count);
                    //Debug
                }
            }
        }
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
        Debug.Log("Nail Collide: " + collision.contactCount);
        //Debug

        foreach (ContactPoint contact in collision.contacts)
        {
            //Check if the head is colliding with an structural object
            if (contact.thisCollider.gameObject.name == "Head")
            {

                //Debug
                //Debug.Log("Nail Collide Target: " + contact.otherCollider.name);
                //Debug

                if (contact.otherCollider.gameObject.tag == "Structure" ||
                    contact.otherCollider.gameObject.tag == "SG Manager")
                {
                    ifTouching = true;

                    //If nailed, set all child to be trigger and start monitoring
                    if (ifNailed && structureGroupPrefab)
                    {
                        NailFunction(contact);
                    }
                }
            }
        }
    }

    public bool ifConnected(GameObject currentTarget)
    {
        foreach (GameObject conect in connected)
        {
            if (GameObject.ReferenceEquals(conect, currentTarget))
            {
                return true;
            }
        }
        return false;
    }

    public void receiveGroup(GameObject childSGroup)
    {
        transform.parent = childSGroup.transform;

        //Debug
        Debug.Log("=====");
        Debug.Log("Nail's parent: " + transform.parent);
        //Debug
    }

    public void addToConnect(GameObject targetObject)
    {
        if (connected.Count == 0)
            connected = new List<GameObject>();

        connected.Add(targetObject);
    }

    public Vector3 getForward()
    {
        return forward;
    }

    public void setTouch()
    {
        ifTouching = true;
    }

    public void setNailed()
    {
        ifNailed = true;
    }
}

