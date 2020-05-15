using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Tools
{
    protected GameObject handle, head, headhead, headend;
    public GameObject managerPrefab;                            //The structure manager prefab, used to hold the object when nailing the nail into an object

    // Start is called before the first frame update
    void Start()
    {
        handle = transform.GetChild(0).gameObject;
        head = transform.GetChild(1).gameObject;
        headhead = transform.GetChild(2).gameObject;
        headend = transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Update the physics effect
    private void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check where hit what
        foreach (ContactPoint contact in collision.contacts)
        {
            //If the hammer hit something with its head
            if(contact.thisCollider.gameObject.name == "HeadHead")
            {
                Debug.Log("Other hammer collider: " + contact.otherCollider.name);
                //Check if hit the nail
                if (contact.otherCollider.gameObject.transform.parent.gameObject.name == "Nail")
                {
                    //Debug
                    Debug.Log("Hammer touch nail!");
                    //Debug

                    //Get the game object
                    GameObject nail = contact.otherCollider.transform.parent.gameObject;

                    //Call the hit function and pass it the direction of hit
                    Vector3 directionHit = transform.GetChild(3).transform.position -
                        transform.GetChild(2).transform.position;

                    nail.GetComponent<Nail>().HitNail(1.0f, managerPrefab);
                }
                //Otherwise check if hit a structure group
                if(contact.otherCollider.tag == "SG Manager")
                {
                    //Debug
                    Debug.Log("Hit Manager");
                    //Debug

                    //Try to ask if hit the nail
                }
            }
        }
    }

    public override void Use()
    {
        base.Use();

        //Set tools to be active
        ifHold = true;
    }
}
