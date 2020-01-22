using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public float m_DefaultLength = 5.0f;
    public GameObject m_Dot;
    public GameObject current;

    private LineRenderer m_LineRenderer = null;

    private void Start()
    {
        current = transform.gameObject;
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Update the current location and forward direction
        Vector3 curPos = current.transform.position;
        Vector3 forDir = current.transform.forward;

        //Ray cast to find if any hit point
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, m_DefaultLength);

        if(hit.collider)    //If hit something
        {
            //Cast the ray based on the hit point
            m_LineRenderer.SetPosition(0, curPos);
            m_LineRenderer.SetPosition(1, hit.point);
        }
        else    //If hit nothing
        {
            m_LineRenderer.SetPosition(0, curPos);
            m_LineRenderer.SetPosition(1, curPos + forDir * m_DefaultLength);
        }
    }






}
