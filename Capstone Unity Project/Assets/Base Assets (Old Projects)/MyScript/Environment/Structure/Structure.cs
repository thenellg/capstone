using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public GameObject trackingManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setManager(GameObject manager)
    {
        trackingManager = manager;

        //Debug
        gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        //Debug
    }
}
