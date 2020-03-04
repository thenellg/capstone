using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : Tools
{
    private GameObject tip;
    private GameObject writer;
    private GameObject eraser;
    private Vector3 direction;
    private GameObject currentLine;
    private LineRenderer currentLineRenderer;

    private Texture2D currentTex;

    public GameObject linePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Start track the tip
        tip = transform.Find("Tip").gameObject;

        //Start track the writer
        writer = transform.Find("Writer").gameObject;

        //Start track the eraser
        eraser = transform.Find("Eraser").gameObject;

        currentLine = null;

        currentLineRenderer = null;

        currentTex = null;

        ifHold = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Update the direction vector
        direction = tip.transform.position - eraser.transform.position;
        direction.Normalize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentLine == null)
        {
            //Check if hold by player, set activated
            if (ifHold)
            {
                //Create a line mark and start to tracking it
                currentLine = Instantiate(linePrefab, tip.transform.position, tip.transform.rotation);
                currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                currentLine.transform.parent = other.transform;

                //Raycast to find the intersection
                RaycastHit hit;
                Vector3 direction = writer.transform.position - tip.transform.position;
                direction.Normalize();
                float distance = Vector3.Distance(tip.transform.position, writer.transform.position);
                if (Physics.Raycast(tip.transform.position, direction, out hit))
                {
                    //Set the first point
                    currentLineRenderer.SetPosition(0, hit.point);
                    currentLineRenderer.SetPosition(1, hit.point);
                }             
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Check if hold by player, set activated
        if (ifHold)
        {
            //Get the target object
            GameObject target = other.gameObject;

            //Raycast to find the intersection
            RaycastHit hit;
            Vector3 direction = writer.transform.position - tip.transform.position;
            direction.Normalize();
            float distance = Vector3.Distance(tip.transform.position, writer.transform.position);
            if (Physics.Raycast(tip.transform.position, direction, out hit, distance))
            {
                //Set the point
                currentLineRenderer.positionCount = currentLineRenderer.positionCount + 1;
                currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, hit.point);

                /*
                //Try to use texture draw
                if (currentTex == null)
                    currentTex = target.GetComponent<Renderer>().material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= currentTex.width;
                pixelUV.y *= currentTex.height;

                //Debug
                Debug.Log(pixelUV);
                //Debug

                currentTex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
                currentTex.Apply();

                //Reset the collider to be non-convex
                target.GetComponent<MeshCollider>().convex = true;
                */
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if hold by player, set activated
        if (ifHold)
        {
            //Keep drawing the line if still touching
            if (currentLine != null)
            {
                //Remove the line tracking
                currentLine = null;

                currentLineRenderer = null;
            }
        }
    }

    public override void Use()
    {
        base.Use();
        ifHold = true;
    }
}
