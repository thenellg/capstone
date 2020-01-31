using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGroup : MonoBehaviour
{
    private Transform[] childList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //Debug.Log("LateUpdate executed!");

        //Put all the child into the tracking list
        childList = GetComponentsInChildren<Transform>();
        

        //Modify the child
        foreach(Transform child in childList)
        {
            //If the child
            if (!GameObject.ReferenceEquals(child.gameObject, gameObject))
            {
                //Disable all the child's rigidbody
                if(child.gameObject.GetComponent<Rigidbody>() != null)
                    Destroy(child.gameObject.GetComponent<Rigidbody>());

                //Tell them I'm the MASTER OF SCRIPT!
                Structure childScript = child.gameObject.GetComponent<Structure>();
                childScript.setManager(gameObject);
            }
        }
    }
}
