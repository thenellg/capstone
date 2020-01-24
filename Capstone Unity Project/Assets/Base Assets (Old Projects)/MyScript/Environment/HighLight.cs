using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    public bool ifHighLight;
    private bool needReset;

    // Start is called before the first frame update
    void Start()
    {
        ifHighLight = false;
        needReset = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if(ifHighLight)
        {
            if(gameObject.GetComponent<Renderer>())
                gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Custom/Outline");

            //Loop through all the child to change their material color
            Renderer[] children = GetComponentsInChildren<Renderer>();

            for(int i = 0; i < children.Length; i++)
            {
                children[i].material.shader = Shader.Find("Custom/Outline");
            }

            //Enable the name tag
            gameObject.GetComponent<NameTag>().tag.gameObject.SetActive(true);

            needReset = true;
        }
        else
        {
            if (gameObject.GetComponent<Renderer>())
                gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Standard");

            //Loop through all the child to change their material color
            Renderer[] children = GetComponentsInChildren<Renderer>();

            for (int i = 0; i < children.Length; i++)
            {
                children[i].material.shader = Shader.Find("Standard");
            }

            //Disable the name tag
            gameObject.GetComponent<NameTag>().tag.gameObject.SetActive(false);
        }
    }
}
