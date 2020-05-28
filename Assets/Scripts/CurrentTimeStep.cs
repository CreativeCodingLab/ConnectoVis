using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentTimeStep : MonoBehaviour
{
    private bool isFirstUpdate = true;
    private GameObject[] connectomes;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            connectomes = GameObject.FindGameObjectsWithTag("SingleConnectome");
            //Text("t  ", new Vector2(-52.5f, -38), new Vector2(43, 0.7f), "CurrentTimeStep", 10);
        }
        //pri
        int currentTimeStep = connectomes[0].GetComponent<SingleNetwork>().DynamicCurrentTime;
        this.GetComponent<Text>().text = "t = "+ currentTimeStep;
    }
}
