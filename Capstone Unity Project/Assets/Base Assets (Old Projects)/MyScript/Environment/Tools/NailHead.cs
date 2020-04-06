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
                        foreach (Transform targetChild in targetManager.transform)
                        {
                            //If target contains rigidbody, then that means it's the root, for double check
                            if (targetChild.GetComponent<Rigidbody>() != null)
                            {
                                //Move the target into current manager
                                targetChild.parent = childStructureGroup.transform;
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
