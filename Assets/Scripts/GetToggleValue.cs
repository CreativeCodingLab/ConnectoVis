using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetToggleValue : MonoBehaviour
{
   
    private bool InNetwork = false;
    private bool ConnectivityHigh = false;
    private bool isFirstUpdate = true;
    private GameObject[] NetworkParents;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate)
        {
            NetworkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
            isFirstUpdate = false;
        }
    }

    public void InNetworkAction()
    {
        InNetwork = !InNetwork;
        if (InNetwork)
        {
            foreach (GameObject networkParent in NetworkParents)
                networkParent.GetComponent<SingleNetwork>().ShowInNetworkConnections = true;       
        }
        else
        {
            foreach (GameObject networkParent in NetworkParents)
                networkParent.GetComponent<SingleNetwork>().ShowInNetworkConnections = false;
        }
    }

    public void ConnectivityHighAction()
    {
        ConnectivityHigh = !ConnectivityHigh;
        if (ConnectivityHigh)
        {

        }
        else
        {
            
        }
    }
}
