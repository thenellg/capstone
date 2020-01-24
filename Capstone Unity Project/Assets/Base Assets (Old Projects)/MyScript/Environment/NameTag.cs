using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : MonoBehaviour
{
    public GameObject tagPreFab;
    public Text tag;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("yes");
        tag = Instantiate(tagPreFab, GameObject.Find("Canvas").transform).GetComponent<Text>();
        tag.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        tag.transform.position = transform.position + new Vector3(0, 0.1f, 0);
    }
}
