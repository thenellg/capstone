using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : Tools
{
    private bool ifNailed;              //If the nail been connected to a structural object
    private bool ifTouching;
    private bool ifFreeze;
    public bool ifNailing;
    private GameObject head, end;       //The game object for head and end child
    private NailHead headScript;
    private NailEnd endScript;

    private GameObject structureGroupPrefab;
    public GameObject structureGroup;       //The current structure group object
    public List<GameObject> connected;      //The list of connected object

    private LineRenderer mLine;
    private Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        //Get the access to head and end and get their script
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


    public void hammerHitFunc()
    {
        //Set the flag
        ifNailing = true;

        //Disable the collision
        GetComponent<Rigidbody>().isKinematic = true;

        //Check if the head was touching something
        if (ifTouching || ifNailed)
        {
            //Check if contains a fixed joint, then remove it if there is one
            FixedJoint current = null;
            Rigidbody connectedManager = null;
            current = GetComponent<FixedJoint>();
            bool needAddBack = false;

            if (current != null)
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

            //Make the nail to be flagged as nailed
            if (!ifNailed)
            {
                ifNailed = true;
            }

            //Move the nail based on direction
            transform.Translate(forward.normalized * Time.deltaTime, Space.World);

            //Debug
            //Debug.Log("Current joint after nailing: " + this.transform.GetComponent<FixedJoint>());
            //Debug

            //Put fixed joint back if there is one, and not added by structure group
            if (GetComponent<FixedJoint>() == null && needAddBack)
            {
                gameObject.AddComponent<FixedJoint>();
                if (structureGroup != null)
                    GetComponent<FixedJoint>().connectedBody = structureGroup.GetComponent<Rigidbody>();
                else if (connectedManager != null)
                    GetComponent<FixedJoint>().connectedBody = connectedManager;
                else
                {
                    //Debug
                    //Debug.Log("Nailing process error, final adding step!");
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

    //Called when a hammer hit nail (call by hammer)
    public void HitByHammer(Vector3 directionHit, GameObject managerPrefab)
    {
        //Receive the prefab
        structureGroupPrefab = managerPrefab;

        hammerHitFunc();
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

                    //If nailed, set all child to be trigger and start monitoring
                    if(ifNailed && structureGroupPrefab)
                    {
                        //Update the child's status
                        headScript.StartNail(structureGroupPrefab);
                        endScript.StartNail();

                        //Set to be trigger
                        foreach (Collider childCollider in GetComponentsInChildren<Collider>())
                        {
                            childCollider.isTrigger = true;
                        }
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
        connected.Add(targetObject);
    }
}

