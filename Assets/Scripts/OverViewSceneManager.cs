using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverViewSceneManager : MonoBehaviour
{

    public DataLoader DataLoader;
    public Builder Builder;
    private string[] FolderNames;
    public List<Dictionary<string, string[][]>> NetworkList = new List<Dictionary<string, string[][]>>();
    private static int firstLoad = 0;
    //public string mainNetwork = string.Empty;
    //private string PreviousMainNetwork = string.Empty;
    private GameObject[] NetworkParents;
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        
        if (sceneName == "OverView")
        {
            if(firstLoad == 0)
            {
                NetworkList = DataLoader.LoadNetworks(DataLoader.DataFolder);
                firstLoad += 1;
            }     
            //else
            //{
            //    DataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
            //    NetworkList = DataLoader.NetworkList;
            //}
            //FolderNames = DataLoader.GetNetworkFolderName(DataLoader.DataFolder);
            FolderNames = DataLoader.FolderNames;
            Builder.InitialBuild(NetworkList, FolderNames, DataLoader.NetworkTimeStep, DataLoader.colorCodingGlobal);
            DontDestroyOnLoad(DataLoader.gameObject);
        }
    }
    private void Update()
    {
        ////if (mainNetwork!=string.Empty&&PreviousMainNetwork != mainNetwork)
        //{
        //    PreviousMainNetwork = mainNetwork;
        //    NetworkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
        //    Builder.Build(mainNetwork, NetworkList, FolderNames, DataLoader.NetworkTimeStep, DataLoader.colorCodingGlobal);
        //    if (NetworkParents.Length>0)
        //    {
        //        foreach (GameObject network in NetworkParents)
        //            network.SetActive(false);
        //    }
            //GameObject[] TimeLines = GameObject.FindGameObjectsWithTag("TimeLine");
            //TimeLines[0].name = "C" + FolderNames[0];
            //TimeLines[1].name = "C" + FolderNames[1];
            //TimeLines[2].name = "C" + FolderNames[2];
            //TimeLines[3].name = "C" + FolderNames[3];
            //TimeLines[4].name = "C" + FolderNames[4];
        //}
    }
}