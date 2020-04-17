using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailHead : NailElement
{ 

    // Start is called before the first frame update
    void Start()
    {
        parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug
        //Debug.Log("Triggered nail head!");
        //Debug

        if(ifNailed)
        {
            /*****Start to connect structure object*****/

            //Get the target object
            GameObject currentTarget = other.gameObject;
            Structure currentTargetScript = currentTarget.GetComponent<Structure>();

            //Debug
            Debug.Log("=====");
            Debug.Log("Triggered nail head and nailed!");
            Debug.Log("Contact object: " + other.gameObject);
            Debug.Log("Parent connection check: " + parent.ifConnected(currentTarget));
            //Debug

            //Check if start to nail
            if (currentTarget.tag == "Structure")
            {
                if (ifNailed && !parent.ifConnected(currentTarget))
                {
                    //Generate a structure group, and give to parent for first time
                    bool ifParentNeedGroup = false;
                    if (childStructureGroup == null && structureGroupPrefab)
                    {
                        ifParentNeedGroup = true;
                    }

                    //If nail doesn't belongs to a structure group, and target object also
                    if (currentTargetScript.trackingManager == null)
                    {
                        if (childStructureGroup == null)
                        {   //Generate the structure group manager to handle nailed object
                            childStructureGroup = Instantiate(structureGroupPrefab);
                            childStructureGroup.transform.position = parent.transform.position;
                            childStructureGroup.transform.parent = null;

                            //Debug
                            Debug.Log("=====");
                            Debug.Log("Parent Set!");
                            //Debug
                        }

                        //Connect the target object into the group
                        currentTarget.transform.parent = childStructureGroup.transform;

                        //Put the first structure object into the connected list
                        if (currentTarget != null)
                            parent.addToConnect(currentTarget);

                        //Debug
                        Debug.Log("Nailing case 1");
                        //Debug
                    }
                    //If nail doesn't belongs to group but target does
                    else if (childStructureGroup == null &&
                        currentTargetScript.trackingManager != null)
                    {
                        //Set the structure group the same as the current target
                        childStructureGroup = currentTargetScript.trackingManager;

                        //Transfer as a child to the structure group
                        parent.transform.parent = childStructureGroup.transform;

                        //Put the structure object into the connected list
                        if (currentTarget != null)
                            parent.addToConnect(currentTarget);

                        //Debug
                        Debug.Log("Nailing case 2");
                        //Debug
                    }
                    //If connecting two structure group, not same
                    else if (childStructureGroup != null &&
                        currentTargetScript.trackingManager != null &&
                        !GameObject.ReferenceEquals(currentTargetScript.trackingManager, childStructureGroup))
                    {
                        //Connect with target object first
                        if (currentTarget != null)
                            parent.addToConnect(currentTarget);

                        //Move all the obejct for other manager group to this one
                        GameObject targetManager = currentTargetScript.trackingManager;

                        //Count how many child need to be transfered
                        int targetChildNum = 0;
                        targetChildNum = targetManager.transform.childCount;

                        for(int i = 0; i < targetChildNum; i++)
                        {
                            Transform targetChild = targetManager.transform.GetChild(i).transform;

                            //If target contains rigidbody, then that means it's the root.
                            //This step is in case the script accidentially get the child's child
                            if (targetChild.GetComponent<Rigidbody>() != null)
                            {
                                //Move the fixed joint if there is one
                                if(targetChild.GetComponent<FixedJoint>())
                                {
                                    targetChild.GetComponent<FixedJoint>().connectedBody = childStructureGroup.GetComponent<Rigidbody>();
                                }

                                //Move the target into current manager
                                targetChild.parent = childStructureGroup.transform;

                                //Set the new child's structure group pointer to the current manager
                                if(targetChild.tag == "Strcuture")
                                {
                                    targetChild.GetComponent<Structure>().trackingManager = childStructureGroup;
                                }
                                else if(targetChild.name == "Nail")
                                {
                                    targetChild.GetComponent<Nail>().structureGroup = childStructureGroup;
                                    targetChild.GetChild(1).GetComponent<NailHead>().childStructureGroup = this.childStructureGroup;
                                    targetChild.GetChild(2).GetComponent<NailEnd>().childStructureGroup = this.childStructureGroup;
                                }
                            }
                        }

                        //After finish, check if target structure group contains no more object
                        if (targetManager.transform.childCount == 0)
                        {
                            Destroy(targetManager);
                        }

                        //Debug
                        Debug.Log("Nailing case 3");
                        //Debug
                    }

                    if (ifParentNeedGroup)
                        parent.receiveGroup(childStructureGroup);
                }
            }
        }
    }
}
