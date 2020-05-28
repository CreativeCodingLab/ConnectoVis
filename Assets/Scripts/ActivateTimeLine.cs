using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActivateTimeLine : MonoBehaviour
{
    private Builder builder;
    Transform[] Children;
    // Start is called before the first frame update
    void Start()
    {
        builder = GameObject.Find("Builder").GetComponent<Builder>();
        Children = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void activateTimeLine()
    {
        foreach (Transform child in Children)
        {
            if (child.tag == "TimeLine")
            {
                child.gameObject.SetActive(true);
                child.transform.Find("TimeLine").gameObject.SetActive(true);
            }
        }

    }

    public void deActivateTimeLine()
    {
        foreach (Transform child in Children)
        {
            if (child.tag == "TimeLine")
                child.gameObject.SetActive(false);
        }
    }
}
