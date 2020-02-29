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

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.gameObject.tag == "Structure")
            {
                //Debug
                Debug.Log("Nail Head Touching");
                //Debug

                //Told the parent the current status
                //parent.Touching();

                /*****Start to connect structure object*****/

                //Get the target object
                GameObject currentTarget = contact.otherCollider.gameObject;

                //Debug
                //currentTarget.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                //Debug

                //Told the parent to nail the target
                //parent.Nailing(currentTarget);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.gameObject.tag == "Structure")
            {
                //Debug
                Debug.Log("Head Leave!");
                //Debug
            }
        }
    }
}
