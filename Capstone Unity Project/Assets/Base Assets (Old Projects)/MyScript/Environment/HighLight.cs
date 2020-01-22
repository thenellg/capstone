using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    public bool ifHighLight;
    Color origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        ifHighLight = false;
    }

    private void LateUpdate()
    {
        if(ifHighLight)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 255);
        }
    }
}
