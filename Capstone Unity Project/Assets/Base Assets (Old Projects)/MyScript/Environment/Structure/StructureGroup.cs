using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGroup : MonoBehaviour
{
    public bool ifActive;

    public List<Transform> childList;
    public List<GameObject> nailList;

    //Debug
    public List<GameObject> targetList;
    public bool ifNeedUpdate;
    //Debug

    public GameObject structureGroupPrefab;

    public OVRGrabbable grabScript;
    public Collider[] grabPoints;

    // Start is called before the first frame update
    void Start()
    {
        grabScript = gameObject.GetComponent<OVRGrabbable>();
        grabScript.enabled = false;

        childList = new List<Transform>();

        nailList = new List<GameObject>();

        //Debug
        ifNeedUpdate = false;
        targetList = new List<GameObject>();
        //Debug

        ifActive = true;

        //Give new name
        gameObject.name = "Structure Group " + Random.Range(0, 9999).ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!grabScript.enabled && grabPoints.Length != 0)
        {
            grabScript.enabled = true;
        }
    }

    void Update()
    {
        if (ifActive)
        {
            //Debug
            Debug.Log("Structure Group " + gameObject.name + " Child Count: " +  transform.childCount);
            //Debug

            //Put all the child into the tracking list
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                //Check if child still exist
                if(child == null)
                {
                    //Debug
                    Debug.Log("Structure Group: Delete null child");
                    //Debug

                    Destroy(transform.GetChild(i).gameObject);
                }

                //Set the iterator for each child
                if(child.name == "Nail")
                {
                    child.gameObject.GetComponent<Nail>().iterated = false;
                }
                else if(child.tag == "Structure")
                {
                    child.gameObject.GetComponent<Structure>().iterated = false;
                }

                if (child != null)
                {
                    //Check if already in the child list
                    bool needAdd = true;
                    for(int j = 0; j < childList.Count; j++)
                    {
                        if (childList[j] != null)
                        {
                            if (ReferenceEquals(childList[j].gameObject, child.gameObject))
                                needAdd = false;
                        }
                        else
                        {
                            //Remove from the child list if the target is not exist anymore
                            childList.RemoveAt(j);
                        }
                    }

                    if (needAdd)
                        AddChild(child);
                }
            }

            //Set the grabble point to all the child collider
            grabPoints = gameObject.GetComponentsInChildren<Collider>();

            //Enable grab pointer for OVR Grababl
            grabScript.NewGrabPoints(grabPoints);

            //Update the structure if needed
            if (ifNeedUpdate)
            {
                UpdateStructure();
                ifNeedUpdate = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ifActive)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider.name == "Head")
                {
                    //Debug
                    Debug.Log("Structure Group Target Collider: " + contact.otherCollider.name);
                    Debug.Log("Structure Group This Collider: " + contact.thisCollider.name);
                    //Debug

                    if (contact.otherCollider.tag == "Structure")
                    {
                        GameObject thisNail = contact.thisCollider.transform.parent.gameObject;
                        Nail thisScript = thisNail.GetComponent<Nail>();
                        GameObject otherStructure = contact.otherCollider.gameObject;
                        Structure otherScript = otherStructure.GetComponent<Structure>();

                        //Check if target structure is single or not
                        if (!otherScript.trackingManager)
                        {
                            //If target structure is single, add into structure group
                            //Debug
                            Debug.Log("Structure Group add new structure: " + otherStructure);
                            //Debug

                            //Try to remove rigid body
                            if (otherStructure.GetComponent<Rigidbody>())
                            {
                                Destroy(otherStructure.GetComponent<Rigidbody>());
                            }

                            //Try to remove the grabbale
                            if (otherStructure.GetComponent<OVRGrabbable>())
                            {
                                Destroy(otherStructure.GetComponent<OVRGrabbable>());
                            }

                            //Add as child and start tracking
                            otherStructure.transform.parent = transform;
                            //AddChild(otherStructure.transform);

                            //Update the target structure
                            otherScript.trackingManager = gameObject;

                            //Connect nail and structure
                            thisNail.GetComponent<Nail>().addToConnect(otherStructure);
                            otherScript.AddConnect(thisNail);
                        }
                        else
                        {
                            //Get the target manager
                            GameObject targetManager = otherScript.trackingManager;

                            //Disable the target tracking manager
                            targetManager.GetComponent<StructureGroup>().ifActive = false;

                            //Debug
                            Debug.Log("SG Trans: from " + targetManager.name + " to " + gameObject.name);
                            Debug.Log("SG Trans: target child num is: " + targetManager.transform.childCount);
                            //Debug

                            //Transfer all the child to this manager
                            //Use the while loop here in order to correct tracking the child number
                            //The old version with for loop will make the counter change as child moved
                            while(targetManager.transform.childCount > 0)
                            {
                                //Get child
                                GameObject currentObject = targetManager.transform.GetChild(0).gameObject;

                                //Debug
                                //targetList.Add(currentObject);
                                //continue;
                                //Debug.Log("SG Trans: Total child: " + targetManager.transform.childCount);
                                //Debug.Log("SG Trans: target " + i + " child is");
                                //Debug.Log(currentObject.name);
                                //Debug

                                //Start to tracking the child
                                //AddChild(currentObject.transform);

                                //Debug
                                //currentObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                                //Debug

                                //Check if stud or nail
                                if (currentObject.tag == "Tools")
                                {
                                    //Debug
                                    Debug.Log("SG Trans: Find Tool '" + currentObject.name + "' in " + targetManager.name);
                                    //Debug

                                    //Set manager
                                    Nail currentScript = currentObject.GetComponent<Nail>();
                                    currentScript.structureGroup = this.gameObject;
                                }
                                else if (currentObject.tag == "Structure")
                                {
                                    //Set manager
                                    Structure currentScript = currentObject.GetComponent<Structure>();
                                    currentScript.trackingManager = this.gameObject;
                                }

                                //Transfer child
                                currentObject.transform.parent = transform;
                            }

                            //Debug
                            //thisNail.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                            //otherStructure.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                            //Debug

                            //Connect nail and structure
                            thisScript.addToConnect(otherStructure);
                            otherScript.AddConnect(thisNail);

                            //Debug.log
                            Debug.Log("SG Trans finish.");
                            //Debug

                            //Destroy target manager
                            Destroy(targetManager);
                        }

                        //Debug
                        //targetList = IterateChild();
                        //Debug
                    }
                }
            }
        }
    }

    //A debug function used to test if the iteration actually work
    public void UpdateStructure()
    {
        //Check if the position is valid
        if(transform.childCount > 0)
        {
            //Start to remove it from its connected object
            List<GameObject> isolateObjects = IterateChild();
            do
            {
                //Debug
                Debug.Log("SG Iterator: The isolateObject list count: " + isolateObjects.Count);
                //Debug

                //Create a new structure group
                if (isolateObjects.Count > 0)
                {
                    GameObject newSG = Instantiate(Resources.Load("StructureGroupPrefab") as GameObject, null);

                    for(int i = 0; i < isolateObjects.Count; i++)
                    {
                        GameObject currentTarget = isolateObjects[i];
                        if (currentTarget.name == "Nail")
                            currentTarget.GetComponent<Nail>().structureGroup = newSG;
                        else
                            currentTarget.GetComponent<Structure>().trackingManager = newSG;

                        isolateObjects[i].transform.parent = newSG.transform;
                    }
                }

                //Get an isolated group
                isolateObjects = IterateChild();

            } while (isolateObjects.Count > 0);
        }
    }

    public List<GameObject> IterateChild()
    {
        //Create a stack for tracking object
        List<GameObject> trackingStack = new List<GameObject>();
        List<GameObject> iteratedStack = new List<GameObject>();

        //Pick up child 0 and start to iterate
        if(transform.childCount > 0)
        {
            //Put the very first object in
            trackingStack.Add(transform.GetChild(0).gameObject);

            //Start the iterate
            int debugCounter = 0;
            while(trackingStack.Count > 0)
            {
                //Get the very first object
                GameObject currentObj = trackingStack[0];

                //Find it's connection
                List<GameObject> currectConnected;
                if(currentObj.name == "Nail")
                {
                    currectConnected = currentObj.GetComponent<Nail>().connected;
                }
                else
                {
                    currectConnected = currentObj.GetComponent<Structure>().connected;
                }

                //Access each connected object, and put into stack if needed
                for(int i = 0; i < currectConnected.Count; i++)
                {
                    GameObject currentTarget = currectConnected[i];

                    bool ifContain = false;
                    for(int j = 0; j < iteratedStack.Count; j++)
                    {
                        if (ReferenceEquals(iteratedStack[j], currentTarget))
                            ifContain = true;
                    }

                    //Check if already iterated
                    if(!ifContain)
                    {
                        //If no, put into the tracking stack
                        trackingStack.Add(currentTarget);
                    }
                }

                //Remove current object from tracking stack
                trackingStack.RemoveAt(0);
                iteratedStack.Add(currentObj);

                //Debug
                debugCounter++;
                if (debugCounter > 2)
                    break;
                //Debug
            }
        }

        return iteratedStack;
    }

    public void AddChild(Transform target)
    {
        childList.Add(target);
    }
}
