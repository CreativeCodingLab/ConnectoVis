using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNodeBehaviour : MonoBehaviour
{

    public bool isEnlarged = false;
    public List<string> FirstSelectedRegions = new List<string>();
    public List<string> SecondSelectedRegions = new List<string>();

    private bool isFirstUpdate = true;
    private string NodeName;
    private string RegionName;
    private GameObject[] NetworkParents;

    void Start()
    {
       
    }

    void Update()
    {
        if (isFirstUpdate)
        {
            NetworkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
            isFirstUpdate = false;
        }  
    }

    public void OnMouseOver()
    {
        NodeName = transform.name;
        RegionName = transform.parent.gameObject.GetComponent<SingleNetwork>().NodeNumberRegionDictionary[int.Parse(NodeName)];

        if (Input.GetMouseButtonDown(0))
        {
            FirstSelectedRegions = transform.parent.gameObject.GetComponent<SingleNetwork>().SelectedRegions;

            if (!FirstSelectedRegions.Contains(RegionName))
            {
                FirstSelectedRegions.Add(RegionName);
                GameObject.Find(RegionName).transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 0, 0);
            }
            else
            {
                GameObject.Find(RegionName).transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 1, 1);
                FirstSelectedRegions.Remove(RegionName);
                foreach (GameObject networkParent in NetworkParents)
                {

                    List<string> selectedNodes = networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule;
                    if (selectedNodes.Contains(transform.name))
                        networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule.Remove(transform.name);
                    selectedNodes = networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule;
                }
            }
            
            foreach (GameObject networkParent in NetworkParents)
            {
                networkParent.GetComponent<SingleNetwork>().SelectedRegions = FirstSelectedRegions;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            FirstSelectedRegions = NetworkParents[0].GetComponent<SingleNetwork>().SelectedRegions;
            //GameObject.Find("Sorting").GetComponent<Sorting>().nodeSelected = transform.name;
            foreach (GameObject networkParent in NetworkParents)
            {
                RegionName = transform.parent.gameObject.GetComponent<SingleNetwork>().NodeNumberRegionDictionary[int.Parse(NodeName)];
                if (FirstSelectedRegions.Contains(RegionName))
                {
                    List<string> selectedNodes = networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule;
                    if(!selectedNodes.Contains(transform.name))
                        networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule.Add(transform.name);
                    else networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule.Remove(transform.name);
                } 
                //else networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule = string.Empty;
            }
        }
    }

    public void OnMouseEnter()
    {
        GameObject node = transform.Find("Text").gameObject;
        if (!node.activeSelf)
            node.SetActive(true);
        else node.SetActive(false);
    }

    public void OnMouseExit()
    {
        GameObject node = transform.Find("Text").gameObject;
        if (!node.activeSelf)
            node.SetActive(true);
        else node.SetActive(false);
    }
}
