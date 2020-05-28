using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Builder : MonoBehaviour
{

    private GameObject _nodePrefab;
    private GameObject _edgePrefab;
    private GameObject _NetworkParentEmpty;

    private GameObject[] FourTimeLines;
    //private GameObject MainTimeLine;
    private GameObject ControlPanel;
    private string atlas = "Mod_1";
    List<Vector3> networkPositionList = new List<Vector3>();
    Vector3 NetworkScaleList = new Vector3(3.5f, 3.5f, 3.5f);
    private List<string> PreviousSelectedNetworks = new List<string>();
    private int NetworkTotalNumebr = 0;
    public List<string> SelectedNetworks = new List<string>();
    //public List<string> NotSelectedNetworks = new List<string>();
    //public List<string> NetworksOnScreen = new List<string>();
    //private List<GameObject> _networkParentList = new List<GameObject>();
    //public string[] TopFour = new string[4];
    //public string[] NotinTheRank = new string[4];
    //public string[] PreviousTopFour = new string[4];
    //public bool mainNetworkChange = false;
    //public bool TopFourChange = false;
    void Awake()
    {
        _NetworkParentEmpty = Resources.Load("Prefabs/SingleNetwork") as GameObject;
        _nodePrefab = Resources.Load("Prefabs/Nodes") as GameObject;
        _edgePrefab = Resources.Load("Prefabs/Edges") as GameObject;

        
        //networkPositionList.Add(new Vector3(7.8f, 0f, 15));
        networkPositionList.Add(new Vector3(3.5f, 5.5f, 15));
        networkPositionList.Add(new Vector3(12.7f, 5.5f, 15));
        networkPositionList.Add(new Vector3(3.5f, -4.5f, 15));
        networkPositionList.Add(new Vector3(12.7f, -4.5f, 15));
        networkPositionList.Add(new Vector3(-10.7f, 8f, 15));


        //NetworkScaleList.Add(new Vector3(4.8f, 4.8f, 4.8f));

        //TopFour = new string[4];
        //PreviousTopFour = new string[4];
        FourTimeLines = GameObject.FindGameObjectsWithTag("TimeLine");
        //FourTimeLines = GameObject.FindGameObjectsWithTag("TimeLine");
        //MainTimeLine = GameObject.FindGameObjectWithTag("MainTimeLine");
        ControlPanel = GameObject.Find("ControlPanel");
    }

    private void Update()
    {
       
        if(SelectedNetworks.Count == 0)
        {
            //MainTimeLine.SetActive(false);
            //foreach(GameObject timeline in FourTimeLines)
            //    timeline.SetActive(false);
            PreviousSelectedNetworks = new List<string>();
            placeAllAside();
        }  
        
        if (SelectedNetworks.Count > 0 && !PreviousSelectedNetworks.SequenceEqual(SelectedNetworks))
        {
            PreviousSelectedNetworks = new List<string>(SelectedNetworks);
            //mainNetworkChange = true;
            //SwapPosition(SelectedNetworks);
            GameObject[] Networks = GameObject.FindGameObjectsWithTag("SingleConnectome");

            placeNetworkCenter();
            placeUnselectedAside();
            //if (Networks[0].GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule == string.Empty)
            //    ControlPanel.GetComponent<ActivateTimeLine>().deActivateTimeLine();

            ///update timeline for the selected networks.
            //ControlPanel.GetComponent<ActivateTimeLine>().activateTimeLine();
            foreach (GameObject timeline in FourTimeLines)
            {
                int timelineNumber = timeline.transform.GetComponent<TimeLineCanvas>().timelineNumber;
                if (timelineNumber < SelectedNetworks.Count)
                {
                    timeline.transform.GetComponent<TimeLineCanvas>().SelectedNetworksChange();
            //        //    timeline.SetActive(true);
                    timeline.name = "T" + SelectedNetworks[timelineNumber];
                }
                //else timeline.transform.Find("TimeLine").Find("Title").GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }

        //if (!PreviousTopFour.SequenceEqual(TopFour))
        //{
        //    //TopFourChange = true;
        //    UpdateTopFourPosition();
        //    foreach(GameObject timeline in FourTimeLines)
        //        timeline.transform.Find("TimeLine").GetComponent<TimeLineControl>().TopFourChange();
        //    ControlPanel.GetComponent<ActivateTimeLine>().activateTimeLine();
        //    for(int i = 0; i<4; i++)
        //        PreviousTopFour[i] = TopFour[i];
        //}
    }

    public void InitialBuild(List<Dictionary<string, string[][]>> NetworkList, string[] NetworkNames, Dictionary<string, int> NetworkTimeStep, Dictionary<string, Color> ColorCoding)
    {
        int n = NetworkNames.Length;
        NetworkTotalNumebr = n;
        //NotinTheRank = new string[n - 5];
        GameObject NetworkParent;
        SelectedNetworks = new List<string>();
        //NotSelectedNetworks = new List<string>();
        //TopFour = new string[4];
        if(n < 4)
        {
            for (int i = 0; i < n; i++)
            {
                NetworkParent = Instantiate(_NetworkParentEmpty, networkPositionList[i], Quaternion.identity);
                NetworkParent.name = "Connectome_"+NetworkNames[i] +"_D";
                SelectedNetworks.Add("Connectome_" + NetworkNames[i] + "_D");
                NetworkParent.transform.localScale = NetworkScaleList;
                NetworkParent.transform.tag = "SingleConnectome";
                NetworkParent.AddComponent<SingleNetwork>().attachNetworkData(atlas, NetworkList[i], "Connectome_" + NetworkNames[i] + "_D", _nodePrefab, _edgePrefab, NetworkTimeStep[NetworkNames[i]], ColorCoding);
            }
        }
        //for (int i = 0; i < 1; i++)
        //{
        //    NetworkParent = Instantiate(_NetworkParentEmpty, networkPositionList[i], Quaternion.identity);
        //    NetworkParent.name = NetworkNames[i];
        //    NetworkParent.transform.localScale = NetworkScaleList[i];
        //    NetworkParent.transform.tag = "SingleConnectome";
        //    NetworkParent.AddComponent<SingleNetwork>().attachNetworkData(atlas,NetworkList[i], NetworkNames[i], _nodePrefab, _edgePrefab, NetworkTimeStep[NetworkNames[i]], ColorCoding);
        //    //SelectedNetworks = NetworkNames[i];
        //    PreviousSelectedNetworks = NetworkNames[i];
        //}
        else
        {
            for (int i = 0; i < 4; i++)
            {
                NetworkParent = Instantiate(_NetworkParentEmpty, networkPositionList[i], Quaternion.identity);
                NetworkParent.name = "Connectome_" + NetworkNames[i] + "_D";
                SelectedNetworks.Add("Connectome_" + NetworkNames[i] + "_D");
                NetworkParent.transform.localScale = NetworkScaleList;
                NetworkParent.transform.tag = "SingleConnectome";
                NetworkParent.AddComponent<SingleNetwork>().attachNetworkData(atlas, NetworkList[i], "Connectome_" + NetworkNames[i] + "_D", _nodePrefab, _edgePrefab, NetworkTimeStep[NetworkNames[i]], ColorCoding);
            }
            for ( int i= 4; i < n; i++)
            {
                NetworkParent = Instantiate(_NetworkParentEmpty, networkPositionList[4], Quaternion.identity);
                NetworkParent.name = "Connectome_" + NetworkNames[i] + "_D";
                //NotSelectedNetworks.Add(NetworkNames[i]);
                NetworkParent.transform.localScale = NetworkScaleList;
                NetworkParent.transform.tag = "SingleConnectome";
                NetworkParent.AddComponent<SingleNetwork>().attachNetworkData(atlas, NetworkList[i], "Connectome_" + NetworkNames[i] + "_D", _nodePrefab, _edgePrefab, NetworkTimeStep[NetworkNames[i]], ColorCoding);
            }
        }
        PreviousSelectedNetworks = new List<string>(SelectedNetworks);
    }

    //void SwapPosition(string mainNetwork)
    //{
    //    GameObject[] Networks = GameObject.FindGameObjectsWithTag("SingleConnectome");

    //    foreach(GameObject singleConn in Networks)
    //    {
    //        if(singleConn.name == mainNetwork)
    //        {
    //            singleConn.transform.localScale = NetworkScaleList;
    //            singleConn.transform.position = networkPositionList[0];
    //        }
    //        else
    //        {
    //            singleConn.transform.position = networkPositionList[5];
    //            singleConn.transform.localScale = NetworkScaleList;
    //        } 
    //    }
    //}

    void placeNetworkCenter()
    {
        for (int i = 0; i < SelectedNetworks.Count; i++)
        {
            GameObject Network = GameObject.Find(SelectedNetworks[i]);
            Network.transform.position = networkPositionList[i];
        }
    }

    void placeAllAside()
    {
        GameObject[] Networks = GameObject.FindGameObjectsWithTag("SingleConnectome");
        
        foreach (GameObject singleConn in Networks)
        {
            singleConn.transform.position = networkPositionList[4];
            singleConn.transform.localScale = NetworkScaleList;
        }
    }

    public void placeUnselectedAside()
    {
        GameObject[] Networks = GameObject.FindGameObjectsWithTag("SingleConnectome");

        foreach (GameObject singleConn in Networks)
        {
            if (!SelectedNetworks.Contains(singleConn.name))
            {
                singleConn.transform.position = networkPositionList[4];
                //singleConn.transform.localScale = NetworkScaleList;
            }
        }
    }


    //public void DestroyConnectomes()
    //{
    //    for (int i = 0; i < _networkParentList.Count; i++)
    //    {
    //        if (_networkParentList[i] != null)
    //            Destroy(_networkParentList[i]);
    //    }
    //}
}