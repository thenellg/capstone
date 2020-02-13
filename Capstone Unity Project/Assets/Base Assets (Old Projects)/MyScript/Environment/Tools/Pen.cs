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
    }

    // Update is called once per frame
    void Update()
    {
        //Update the direction vector
        direction = tip.transform.position - eraser.transform.position;
        direction.Normalize();

        //Debug
        //Debug.Log("Active!");
        //Debug
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(currentLine == null)
        {
            //Check if hold by player, set activated
            if (true)
            {
                //Tracking all the possible collision
                foreach (ContactPoint contact in collision.contacts)
                {
                    //Find the tip's collision
                    if (ReferenceEquals(contact.thisCollider.gameObject, tip))
                    {
                        //Create a line mark and start to tracking it
                        currentLine = Instantiate(linePrefab, tip.transform.position, tip.transform.rotation);
                        currentLineRenderer = currentLine.GetComponent<LineRenderer>();

                        //Set the first point
                        currentLineRenderer.SetPosition(0, tip.transform.position);
                        currentLineRenderer.SetPosition(1, tip.transform.position);
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {

        //Check if hold by player, set activated
        if (true)
        {
            //Tracking all the possible collision
            foreach(ContactPoint contact in collision.contacts)
            {
                //Find the tip's collision
                if(ReferenceEquals(contact.thisCollider.gameObject, writer))
                {
                    //Get the target object
                    GameObject target = contact.otherCollider.gameObject;

                    //Keep drawing the line if still touching
                    if(currentLine != null)
                    {
                        //Raycast to find the intersection
                        RaycastHit hit;
                        Vector3 direction = writer.transform.position - tip.transform.position;
                        float distance = Vector3.Distance(tip.transform.position, writer.transform.position);
                        if(Physics.Raycast(tip.transform.position, direction, out hit, distance))
                        {
                            //Set the point
                            currentLineRenderer.positionCount = currentLineRenderer.positionCount + 1;
                            currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, hit.point);
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Leave!");

        //Check if hold by player, set activated
        if (true)
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
}
