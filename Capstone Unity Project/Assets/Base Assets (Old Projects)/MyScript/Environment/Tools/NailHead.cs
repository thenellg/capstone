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

    }
}
