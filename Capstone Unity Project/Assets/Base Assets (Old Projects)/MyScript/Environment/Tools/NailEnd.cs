using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailEnd : NailElement
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
        //Check if activated
        if(ifNailed)
        {
            Debug.Log("End touch!" + other.gameObject.name);

            //Check if other collider is the hammer
            if(other.gameObject.name == "HeadHead")
            {
                //parent.hammerHitFunc();
            }
        }
    }
}
