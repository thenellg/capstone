using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineTrace : MonoBehaviour
{
    private GameObject parent;
    private LineRenderer line;
    private int numCheck;
    private List<GameObject> trackList;

    public GameObject checkObjPre;

    // Start is called before the first frame update
    void Start()
    {
        parent = null;
        line = null;

        numCheck = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(parent == null)
        {
            //Try to get the parent
            parent = transform.parent.gameObject;
        }

        if(parent != null)
        {
            //Try to get the line
            if(line == null)
            {
                line = gameObject.GetComponent<LineRenderer>();
            }
            else
            {
                //Get total position
                int totalCheck = line.positionCount;
                for(int i = numCheck; i < totalCheck; i++)
                {
                    //Generate track prefab
                    GameObject curObj = Instantiate(checkObjPre);

                    //Set orientation
                    curObj.transform.position = line.GetPosition(i);
                    curObj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    curObj.transform.parent = transform.parent;
                    trackList.Add(curObj);

                    numCheck++;
                }

                //Update all the position
                for(int i = 0; i < totalCheck; i++)
                {
                    line.SetPosition(i, trackList[i].transform.position);
                }
            }
        }
    }
}
