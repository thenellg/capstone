using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public GameObject trackingManager;

    public List<GameObject> connected;

    public bool iterated;

    // Start is called before the first frame update
    void Start()
    {
        trackingManager = null;

        connected = new List<GameObject>();

        iterated = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Update the status to stick on tracking manager
        if (trackingManager)
            transform.parent = trackingManager.transform;

        //Update the connected list
        for (int i = 0; i < connected.Count; i++)
        {
            //Delete the invalid(null) connected object
            if (connected[i] == null)
            {
                //Debug
                Debug.Log("Delete invalid connected object!");
                //Debug

                connected.RemoveAt(i);
            }
        }
    }

    public void setManager(GameObject manager)
    {
        trackingManager = manager;
    }

    public void AddConnect(GameObject targetNail)
    {
        connected.Add(targetNail);
    }
}
