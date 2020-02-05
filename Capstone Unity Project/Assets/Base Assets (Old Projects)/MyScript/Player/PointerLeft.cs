using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerLeft : MonoBehaviour
{
    public float m_DefaultLength = 5.0f;
    public GameObject m_Dot;
    public GameObject current;
    private GameObject pointObject;

    private LineRenderer m_LineRenderer = null;

    void Awake()
    {
        current = transform.GetChild(1).gameObject;
        m_LineRenderer = current.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
        {
            //Update the current location and forward direction
            Vector3 curPos = current.transform.position;
            Vector3 forDir = current.transform.forward;

            //Ray cast to find if any hit point
            RaycastHit hit;
            Ray ray = new Ray(curPos, forDir);
            Physics.Raycast(ray, out hit, m_DefaultLength);

            if (hit.collider)    //If hit something
            {
                //Cast the ray based on the hit point
                m_LineRenderer.SetPosition(0, curPos);
                m_LineRenderer.SetPosition(1, hit.point);

                //Check if hit an interactable object
                if (hit.collider.gameObject.layer == 9)
                {
                    FindEnvironmentalParent(hit.collider.gameObject);
                }
                else     //If not hitting a interactable object
                {
                    //If the pointer pointed at something before
                    if (pointObject != null)
                    {
                        //Reset that color
                        pointObject.GetComponent<HighLight>().ifHighLight = false;

                        //Reset line color
                        m_LineRenderer.startColor = new Color(255, 255, 255);

                        pointObject = null;
                    }
                }
            }
            else    //If hit nothing
            {
                m_LineRenderer.SetPosition(0, curPos);
                m_LineRenderer.SetPosition(1, curPos + forDir * m_DefaultLength);

                //If the pointer pointed at something before
                if (pointObject != null)
                {
                    //Reset that color
                    pointObject.GetComponent<HighLight>().ifHighLight = false;

                    //Reset line color
                    m_LineRenderer.startColor = new Color(255, 255, 255);

                    pointObject = null;
                }
            }
        }
        else
        {
            m_LineRenderer.SetPosition(0, current.transform.position);
            m_LineRenderer.SetPosition(1, current.transform.position);
        }
    }

    private GameObject FindEnvironmentalParent(GameObject child)
    {
        if(child.layer == 9 && child.tag != "Tools")
        {
            return FindEnvironmentalParent(child.transform.parent.gameObject);
        }
        else if(child.layer == 9 && child.tag == "Tools")
        {
            pointObject = child;

            //Highlight the object
            pointObject.GetComponent<HighLight>().ifHighLight = true;
            
            //Chang the line color
            m_LineRenderer.startColor = new Color(255, 0, 0);
            return child;
        }
        return null;
    }




}
