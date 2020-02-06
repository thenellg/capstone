using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTest : MonoBehaviour
{
    public GameObject Saw;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Saw.transform.position;
        transform.rotation = Saw.transform.rotation;
        transform.Rotate(new Vector3(90, 0, 0));
    }
}
