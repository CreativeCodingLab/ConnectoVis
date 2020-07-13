using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TimeLineCanvas : MonoBehaviour
{
    [SerializeField]private Sprite rectangleSprite;
    public int timelineNumber;
    private RectTransform graphContainer;
    private DataLoader dataLoader;
    private Builder builder;
    private bool isFirstUpdate = true;
    private int timestep;
    private string networkName;
    private List<string> NodeSelected = new List<string>();
    private List<string> previousNodeSelected = new List<string>();
    private List<Dictionary<string, string[][]>> NetworkList = new List<Dictionary<string, string[][]>>();
    private Dictionary<string, int> NetworkNameIndex = new Dictionary<string, int>();
    private Dictionary<int, string> NodeNumberRegionDictionary = new Dictionary<int, string>();
    private Dictionary<string, Color> ColorCoding = new Dictionary<string, Color>();
    private GameObject SingleNetwork;
    private int moduleCount = 0;
    private int totalNodeNumber;
    private Dictionary<string, float> ModulePattern = new Dictionary<string, float>();

    private float posThresholdMin = 0f;
    private float posThresholdMax = 1f;
    private float posPreThresholdMin = 0;
    private float posPreThresholdMax = 1;

    private float negThresholdMin = 0f;
    private float negThresholdMax = 1f;
    private float negPreThresholdMin = 0;
    private float negPreThresholdMax = 1;
    private List<GameObject> moduleList = new List<GameObject>();
    //private List<GameObject> statList = new List<GameObject>();
    //private string[] moduleOnTimeLine = new string[16];
    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        
    }
    private void Start()
    {
        builder = GameObject.Find("Builder").GetComponent<Builder>();
        dataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
        posThresholdMax = posPreThresholdMax;
        posThresholdMin = posPreThresholdMin;
        negThresholdMin = negPreThresholdMin;
        negThresholdMax = negPreThresholdMax;
    }

    private void CreateRectangle(Vector2 anchoredPosition, Color color, Vector2 size,string Name)
    {
        GameObject rectangleGameObject = new GameObject(Name, typeof(Image));
        rectangleGameObject.GetComponent<Image>().color = color;
        rectangleGameObject.transform.SetParent(graphContainer, false);
        rectangleGameObject.GetComponent<Image>().sprite = rectangleSprite;
        RectTransform recTransform = rectangleGameObject.GetComponent<RectTransform>();
        recTransform.anchoredPosition = anchoredPosition;
        recTransform.sizeDelta = size;
        recTransform.anchorMin = new Vector2(0, 0);
        recTransform.anchorMax = new Vector2(0, 0);
        if (Name.StartsWith("Module"))
            moduleList.Add(rectangleGameObject);
        //else if (Name.StartsWith("Stat"))
            //statList.Add(rectangleGameObject);
    }

    private void Text(string title, Vector2 anchoredPosition, Vector2 size, string objectName, int fontSize)
    {
        GameObject timelineName = new GameObject();
        timelineName.transform.parent = this.transform;
        timelineName.name = objectName;
        TextMeshProUGUI text;
        text = timelineName.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.fontSize = fontSize;
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
    }

    public void SelectedNetworksChange()
    {
        if (this.transform.tag == "TimeLine")
        {
            this.transform.name = "T" + builder.SelectedNetworks[timelineNumber];
            networkName = builder.SelectedNetworks[timelineNumber];
        }
        SingleNetwork = GameObject.Find(networkName);
        //NodeNumberRegionDictionary = SingleNetwork.GetComponent<SingleNetwork>().NodeNumberRegionDictionary;
        this.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = networkName;
        if (moduleList.Count != 0)
        {
            //ClearStat();
            ClearMod();
            Recaculating();
        }  
    }

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            timestep = dataLoader.timeStep;
            //moduleOnTimeLine = new string[timestep];
            if (timelineNumber <= builder.SelectedNetworks.Count - 1)
            {
                this.transform.name = "T" + builder.SelectedNetworks[timelineNumber];
                networkName = builder.SelectedNetworks[timelineNumber];
                //print(networkName);
                Text(networkName, new Vector2(-32f, 65), new Vector2(200, 1), "Title", 15);
            }
            SingleNetwork = GameObject.Find(networkName);
            //print(SingleNetwork);
            NetworkList = dataLoader.NetworkGlobalList;
            NetworkNameIndex = dataLoader.NetworkNameIndex;
            NodeNumberRegionDictionary = SingleNetwork.GetComponent<SingleNetwork>().NodeNumberRegionDictionary;
            ColorCoding = dataLoader.colorCodingGlobal;
            moduleCount = ColorCoding.Count;
            for (int i = 0; i < moduleCount; i++)
            {
                ModulePattern.Add((i + 1).ToString(), 0);
            }
            totalNodeNumber = dataLoader.totalNodeNumber;
            for (int i = 1; i < timestep+1; i++)
            {
                float width = 20;
                float height = 0.7f;
                //if(i ==1 || i % 50 == 0)
                {
                    Text((i).ToString(), new Vector2(-58f + width/2+ 98f / timestep *(i-1) , -38), new Vector2(width, height), "TimeStep",10);
                }
            }
            for (int i = 0; i <5; i++)
            {
                Text("  ", new Vector2(-53, 37-13*i), new Vector2(43, 0.7f), "RegionName"+i, 6);
            }

            //Text("t  ", new Vector2(-52.5f, -38), new Vector2(43, 0.7f), "CurrentTimeStep", 10);
        }
        //print(SingleNetwork.name);
        NodeSelected = SingleNetwork.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule;
        if (NodeSelected.Count != 0 && !previousNodeSelected.SequenceEqual(NodeSelected))
        {
            previousNodeSelected = new List<string>(NodeSelected);
            //ClearStat();
            //SelectedNetworksChange();
            ClearMod();
            Recaculating();
        }
        if (NodeSelected.Count == 0)
        {
            //ClearStat();
            ClearMod();
        }

        posThresholdMax = SingleNetwork.GetComponent<SingleNetwork>().posThresholdMax;
        posThresholdMin = SingleNetwork.GetComponent<SingleNetwork>().posThresholdMin;

        negThresholdMax = SingleNetwork.GetComponent<SingleNetwork>().negThresholdMax;
        negThresholdMin = SingleNetwork.GetComponent<SingleNetwork>().negThresholdMin;

        if (posPreThresholdMin != posThresholdMin || posPreThresholdMax != posThresholdMax)
        {
            posPreThresholdMax = posThresholdMax;
            posPreThresholdMin = posThresholdMin;

            //if (statList.Count != 0)
            {
                //ClearStat();
                //PosConnectivityMod();
                //Recaculating();
            }
        }
        //int currentTimeStep = SingleNetwork.GetComponent<SingleNetwork>().DynamicCurrentTime;
        //this.transform.Find("CurrentTimeStep").GetComponent<TextMeshProUGUI>().text = "t = "+ currentTimeStep;

    }

    private void Recaculating()
    {
        string module;
        //hideConnMod();
        if (NodeSelected.Count != 0)
        {
            ///Modularity
            for (int i = 0; i < NodeSelected.Count; i++)
            {
                //RegionModuleName[i].GetComponent<TextMesh>().text = NodeNumberRegionDictionary[int.Parse(NodeSelected[i])];
                //RegionModuleName[i].SetActive(true);
                this.transform.Find("RegionName"+i).GetComponent<TextMeshProUGUI>().text = NodeNumberRegionDictionary[int.Parse(NodeSelected[i])];
                for (int p = 1; p < timestep+1; p++)
                {
                    module = NetworkList[NetworkNameIndex[networkName]]["Mod_" + p][int.Parse(NodeSelected[i])][1];
                    float width = 100f / timestep;
                    CreateRectangle(new Vector2(width/2+100f/timestep*(p-1),80f-10*i -2*i), ColorCoding[module], new Vector2(width,10f), "Module"+p);
                }
                //foreach (Transform child in Children)
                //{
                //    if (child.name.Contains("TimeStep"))
                //    {
                //        int timeStep = int.Parse(child.name.Substring(8));
                //        GameObject Module = child.Find("Module" + i).gameObject;
                //        Module.SetActive(true);
                //        Module.GetComponent<Renderer>().material.color = ColorCoding[moduleOnTimeLine[timeStep - 1]];

                //        //GameObject connectivityTemp = child.Find("Connectivity").gameObject;
                //        //connectivityTemp.SetActive(true);
                //        //connectivityTemp.transform.localPosition = new Vector3(0, ConnectivityArray[timeStep - 1] * 2, 0);
                //    }
                //}
            }
        }
        //PosConnectivityMod();
        //NegConnectivityMod();
    }

    //void PosConnectivityMod()
    //{
    //    float connectivity = 0;
    //    for (int p = 1; p < timestep+1; p++)
    //    {
    //        for (int i = 0; i < moduleCount; i++)
    //        {
    //            ModulePattern[(i + 1).ToString()] = 0;
    //        }
    //        string moduleNumber;
    //        for (int j = 0; j < totalNodeNumber; j++)
    //        {
    //            for (int i = 0; i < NodeSelected.Count; i++)
    //            {
    //                connectivity = float.Parse(NetworkList[NetworkNameIndex[networkName]]["Time_" + p][int.Parse(NodeSelected[i])][j]);
    //                if (connectivity > posThresholdMin && connectivity < posThresholdMax)
    //                {
    //                    moduleNumber = NetworkList[NetworkNameIndex[networkName]]["Mod_" + p][j + 1][1];
    //                    ModulePattern[moduleNumber] = ModulePattern[moduleNumber] + connectivity;
    //                }

    //            }
    //        }
    //        var sortedModule = from pair in ModulePattern orderby pair.Value descending select pair;
    //        float connectivityOffset = 5f;
    //        foreach (KeyValuePair<string, float> EachModulaStrength in sortedModule)
    //        {
    //            string moduleName = EachModulaStrength.Key;
    //            float connectivityStrength = Mathf.Abs(EachModulaStrength.Value);
    //            float width = 100f / timestep;
    //            //CreateRectangle(new Vector2(width / 2 + 100f / timestep * (p - 1), -7.5f), ColorCoding[module], new Vector2(width, 15f));
    //            CreateRectangle(new Vector2(2 + width / 2 + 98f / timestep * (p - 1), connectivityOffset+connectivityStrength * 2), ColorCoding[moduleName], new Vector2(width, connectivityStrength*4),"Stat" +p);
    //            //ModuleStat = Instantiate(ConnectivityPref, new Vector3(0f, connectivityOffset + connectivityStrength / 2, 0), Quaternion.identity);
    //            connectivityOffset = connectivityOffset + connectivityStrength*4;
    //            //ModuleStat.GetComponent<Renderer>().material.color = ColorCoding[moduleName];
    //            //Vector3 temPos = ModuleStat.transform.position;
    //            //ModuleStat.transform.parent = this.gameObject.transform.Find("TimeStep" + p);
    //            //ModuleStat.transform.localPosition = temPos;
    //            //ModuleStat.transform.name = "ModuleStat";
    //            //ModuleStat.transform.localScale = new Vector3(0.3f, connectivityStrength, 1);
    //            //allPosModuleStat.Add(ModuleStat);
    //        }
    //    }
    //}

    void ClearMod()
    {
        foreach (GameObject mod in moduleList)
            if (mod != null)
                Destroy(mod);
    }

    //void ClearStat()
    //{
    //    foreach (GameObject stat in statList)
    //        if (stat != null)
    //            Destroy(stat);
    //}
}
