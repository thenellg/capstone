using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial : MonoBehaviour
{
    public GameObject removableWall;
    public GameObject removableWallMenu;

    void Start()
    {
        removableWallMenu.SetActive(false);
    }

    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            removableWallMenu.SetActive(true);
            //removeWall();
        }
    }
    public void removeWall()
    {
        Destroy(removableWall);
        Destroy(removableWallMenu);
    }

}
