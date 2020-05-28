using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TimeLineControl : MonoBehaviour
{
    public int timelineNumber;
    private bool isFirstUpdate = true;
    private Transform[] Children;
    private GameObject Rectangle;
    private GameObject HighlightRectangle;
    private GameObject SingleTimeStepPref;
    private GameObject SingleTimeStep;
    //private GameObject ModuleStat;
    //private List<GameObject> allPosModuleStat = new List<GameObject>();
    //private List<GameObject> allNegModuleStat = new List<GameObject>();
    private List<string> NodeSelected = new List<string>();
    private List<string> previousNodeSelected = new List<string>();
    private GameObject ModulePref;
    private GameObject[] Module = new GameObject[4];
    private GameObject ConnectivityGameObject;
    private GameObject ConnectivityPref;
    private string networkName;
    private List<Dictionary<string, string[][]>> NetworkList = new List<Dictionary<string, string[][]>>();
    private Dictionary<string, int> NetworkNameIndex = new Dictionary<string, int>();
    private int timestep;
    private int totalNodeNumber;
    private string[] moduleOnTimeLine = new string[16];
    private Dictionary<string, float> ModulePattern = new Dictionary<string, float> ();
    //private float[] ConnectivityArray = new float[16];
    private GameObject SingleNetwork;
    private Dictionary<int, string> NodeNumberRegionDictionary = new Dictionary<int, string>();
    //private List<string> networksOnScreen = new List<string>();
    //private string mainNetwork = string.Empty;
    private GameObject RegionModuleNamePref;
    private GameObject[] RegionModuleName = new GameObject[4];

    private GameObject TimeMark;
    private GameObject Title;
    private DataLoader dataLoader;
    private int moduleCount = 0;
    private Builder builder;
    private Dictionary<string, Color> ColorCoding = new Dictionary<string, Color>();
    private float posThresholdMin = 0f;
    private float posThresholdMax = 1f;
    private float posPreThresholdMin = 0;
    private float posPreThresholdMax = 1;

    private float negThresholdMin = 0f;
    private float negThresholdMax = 1f;
    private float negPreThresholdMin = 0;
    private float negPreThresholdMax = 1;
    private GameObject[] networkParents;
    // Start is called before the first frame update

    void Start()
    {

        builder = GameObject.Find("Builder").GetComponent<Builder>();
        dataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
        
        
        
        posThresholdMax = posPreThresholdMax;
        posThresholdMin = posPreThresholdMin;
        negThresholdMin = negPreThresholdMin;
        negThresholdMax = negPreThresholdMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            networkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
            timestep = dataLoader.timeStep;
            SingleTimeStepPref = Resources.Load("Prefabs/ConnectivityDot") as GameObject;
            float size = (float)1 / timestep;
            for (int i = 0; i < timestep; i++)
            {
                SingleTimeStep = Instantiate(SingleTimeStepPref, new Vector3(-0.5f + size / 2 + size * i, 0, 0), Quaternion.identity);
                Vector3 temPos = SingleTimeStep.transform.position;
                SingleTimeStep.transform.parent = this.transform;
                SingleTimeStep.transform.localPosition = temPos;
                SingleTimeStep.transform.name = "TimeStep" + (i + 1);
                SingleTimeStep.transform.localScale = new Vector3(size - 0.0025f, 1, 1);
                SingleTimeStep.gameObject.layer = LayerMask.NameToLayer("UI");
                SingleTimeStep.GetComponent<MeshRenderer>().enabled = false;
            }
            moduleCount = dataLoader.colorCodingGlobal.Count;
            for (int i = 0; i < moduleCount; i++)
            {
                ModulePattern.Add((i + 1).ToString(), 0);
            }
            moduleOnTimeLine = new string[timestep];
            //ConnectivityArray = new float[timestep];
            totalNodeNumber = dataLoader.totalNodeNumber;
            Children = GetComponentsInChildren<Transform>();
            Rectangle = Resources.Load("Prefabs/RectangleNew") as GameObject;
            ModulePref = Resources.Load("Prefabs/Module") as GameObject;
            ConnectivityPref = Resources.Load("Prefabs/ConnectivityDot") as GameObject;
            RegionModuleNamePref = Resources.Load("Prefabs/Text") as GameObject;
            ColorCoding = dataLoader.colorCodingGlobal;
            //if (this.transform.parent.tag == "MainTimeLine")
            //{
            //    this.transform.parent.name = "T" + builder.SelectedNetworks;
            //    networkName = builder.SelectedNetworks;
            //}
            //else
            if (timelineNumber <= builder.SelectedNetworks.Count - 1)
            {
                this.transform.parent.name = "T" + builder.SelectedNetworks[timelineNumber];
                networkName = builder.SelectedNetworks[timelineNumber];
            }
            else
                this.transform.parent.gameObject.SetActive(false);

            //this.transform.parent.name = "T" + builder.SelectedNetworks;
            SingleNetwork = GameObject.Find(networkName);
            NetworkList = dataLoader.NetworkGlobalList;
            NetworkNameIndex = dataLoader.NetworkNameIndex;
            NodeNumberRegionDictionary = SingleNetwork.GetComponent<SingleNetwork>().NodeNumberRegionDictionary;

            for (int i = 0; i < 4; i++)
            {
                RegionModuleName[i] = Instantiate(RegionModuleNamePref, new Vector3(-0.54f, -0.7f - 2 * i, -1), Quaternion.identity);
                RegionModuleName[i].SetActive(false);
                Vector3 temPosition3 = RegionModuleName[i].transform.position;
                RegionModuleName[i].transform.parent = this.transform;
                RegionModuleName[i].transform.localPosition = temPosition3;
            }


            TimeMark = Instantiate(RegionModuleNamePref, new Vector3(-0.52f, 1f, -1), Quaternion.identity);
            Vector3 temPosition4 = TimeMark.transform.position;
            TimeMark.transform.parent = this.transform;
            TimeMark.transform.localPosition = temPosition4;
            TimeMark.transform.name = "t";
            TimeMark.GetComponent<TextMesh>().text = "t";

            Title = Instantiate(RegionModuleNamePref, new Vector3(-0.5f, 25f, -1), Quaternion.identity);
            Vector3 temPosition6 = Title.transform.position;
            Title.name = "Title";
            Title.transform.parent = this.transform;
            Title.transform.localPosition = temPosition6;
            Title.GetComponent<TextMesh>().text = networkName;
            Title.transform.localScale = new Vector3(0.002f, 0.33f, 500);

            foreach (Transform child in Children)
            {
                if (child.name.Contains("TimeStep"))
                {
                    TimeMark = Instantiate(RegionModuleNamePref, new Vector3(0f, 0f, -1), Quaternion.identity);
                    Vector3 temPosition5 = TimeMark.transform.position;
                    TimeMark.transform.parent = child.transform;
                    TimeMark.transform.localPosition = temPosition5;
                    TimeMark.transform.name = "TimeStep";
                    TimeMark.GetComponent<TextMesh>().text = child.name.Substring(8);

                    HighlightRectangle = Instantiate(Rectangle, new Vector3(0, 10.6f, -1), Quaternion.identity);
                    Vector3 temPosition = HighlightRectangle.transform.position;
                    HighlightRectangle.transform.parent = child;
                    HighlightRectangle.transform.localPosition = temPosition;
                    HighlightRectangle.transform.localScale = new Vector3(1, 20.6f, 1);
                    HighlightRectangle.transform.name = "Rectangle";
                    HighlightRectangle.gameObject.layer = LayerMask.NameToLayer("UI");
                    HighlightRectangle.SetActive(false);

                    for (int i = 0; i < 1; i++)
                    {
                        Module[i] = Instantiate(ModulePref, new Vector3(0, (-1.5f - 2 * i), 0), Quaternion.identity);
                        Vector3 temPosition1 = Module[i].transform.position;
                        Module[i].transform.parent = child;
                        Module[i].transform.localPosition = temPosition1;
                        Module[i].transform.name = "Module" + i;
                        Module[i].transform.localScale = new Vector3(1, 2, 1);
                        Module[i].gameObject.layer = LayerMask.NameToLayer("UI");
                        Module[i].SetActive(false);
                    }


                    //ConnectivityGameObject = Instantiate(ConnectivityPref, new Vector3(0, 0f, 0), Quaternion.identity);
                    //Vector3 temPosition7 = ConnectivityGameObject.transform.position;
                    //ConnectivityGameObject.transform.parent = child;
                    //ConnectivityGameObject.transform.localPosition = temPosition7;
                    //ConnectivityGameObject.transform.name = "Connectivity";
                    //ConnectivityGameObject.transform.localScale = new Vector3(0.1f, 0.6f, 1);
                    //ConnectivityGameObject.gameObject.layer = LayerMask.NameToLayer("UI");
                    //ConnectivityGameObject.SetActive(false);
                }
            }
        }

        int currentTimeStep = SingleNetwork.GetComponent<SingleNetwork>().DynamicCurrentTime;
        foreach (Transform child in Children)
        {
            if (child.name == "TimeStep" + currentTimeStep)
            {
                child.gameObject.GetComponent<Renderer>().material.color = new Color32(38, 159, 148, 255);
                child.Find("Rectangle").gameObject.SetActive(true);
            }

            else if (child.name.Contains("TimeStep"))
            {
                child.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
                child.Find("Rectangle").gameObject.SetActive(false);
            }
        }

        NodeSelected = SingleNetwork.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule;
        if (NodeSelected.Count != 0 && !previousNodeSelected.SequenceEqual(NodeSelected))
        {
            previousNodeSelected = new List<string>(NodeSelected);
            Recaculating();
        }
        if (NodeSelected.Count == 0)
        {
            hideConnMod();
        }

        posThresholdMax = networkParents[0].GetComponent<SingleNetwork>().posThresholdMax;
        posThresholdMin = networkParents[0].GetComponent<SingleNetwork>().posThresholdMin;

        negThresholdMax = networkParents[0].GetComponent<SingleNetwork>().negThresholdMax;
        negThresholdMin = networkParents[0].GetComponent<SingleNetwork>().negThresholdMin;

        if (posPreThresholdMin != posThresholdMin || posPreThresholdMax != posThresholdMax)
        {
            posPreThresholdMax = posThresholdMax;
            posPreThresholdMin = posThresholdMin;
            //ClearAllPosModuleStat();
            //PosConnectivityMod();
        }

        if (negPreThresholdMin != negThresholdMin || negPreThresholdMax != negThresholdMax)
        {
            negPreThresholdMax = negThresholdMax;
            negPreThresholdMin = negThresholdMin;
            //ClearAllNegModuleStat();
            //NegConnectivityMod();
        }
        //if(builder.mainNetworkChange)
        //{
        //    builder.mainNetworkChange = false;
        //    //if (NodeSelected!=string.Empty)


        //    if (this.transform.parent.tag == "MainTimeLine")
        //    {
        //        this.transform.parent.name = "T" + builder.SelectedNetworks;
        //        networkName = builder.SelectedNetworks;
        //    }
        //    else
        //    {
        //        this.transform.parent.name = "T" + builder.TopFour[timelineNumber - 1];
        //        networkName = builder.TopFour[timelineNumber - 1];
        //        //this.transform.parent.gameObject.SetActive(false);
        //    }

        //    SingleNetwork = GameObject.Find(networkName);
        //    this.transform.Find("Title").GetComponent<TextMesh>().text = networkName;

        //    Recaculating();

        //}
    }

    public void SelectedNetworksChange()
    {
        if (this.transform.parent.tag == "TimeLine")
        {
            this.transform.parent.name = "T" + builder.SelectedNetworks[timelineNumber];
            networkName = builder.SelectedNetworks[timelineNumber];
        }
        SingleNetwork = GameObject.Find(networkName);
        NodeNumberRegionDictionary = SingleNetwork.GetComponent<SingleNetwork>().NodeNumberRegionDictionary;
        this.transform.Find("Title").GetComponent<TextMesh>().text = networkName;

        Recaculating();
    }

    void Recaculating()
    {
        hideConnMod();
        if (NodeSelected.Count != 0)
        {
            ///Modularity
            for (int i = 0; i < NodeSelected.Count; i++)
            {
                RegionModuleName[i].GetComponent<TextMesh>().text = NodeNumberRegionDictionary[int.Parse(NodeSelected[i])];
                RegionModuleName[i].SetActive(true);

                for (int p = 1; p < timestep + 1; p++)
                {
                    moduleOnTimeLine[p - 1] = NetworkList[NetworkNameIndex[networkName]]["Mod_" + p][int.Parse(NodeSelected[i])][1];
                }
                foreach (Transform child in Children)
                {
                    if (child.name.Contains("TimeStep"))
                    {
                        int timeStep = int.Parse(child.name.Substring(8));
                        GameObject Module = child.Find("Module" + i).gameObject;
                        Module.SetActive(true);
                        Module.GetComponent<Renderer>().material.color = ColorCoding[moduleOnTimeLine[timeStep - 1]];

                        //GameObject connectivityTemp = child.Find("Connectivity").gameObject;
                        //connectivityTemp.SetActive(true);
                        //connectivityTemp.transform.localPosition = new Vector3(0, ConnectivityArray[timeStep - 1] * 2, 0);
                    }
                }
            }
        }
        //PosConnectivityMod();
        //NegConnectivityMod();
    }
            ///connectivity
            

            //foreach (Transform child in Children)
            //{
            //    if (child.name.Contains("TimeStep"))
            //    {
            //        int timeStep = int.Parse(child.name.Substring(8));
            //        GameObject connectivityTemp = child.Find("Connectivity").gameObject;
            //        connectivityTemp.SetActive(true);
            //        connectivityTemp.transform.localPosition = new Vector3(0, 0.5f+ConnectivityArray[timeStep - 1] * 2, 0);
            //    }
            //}

            //float connectivity = 0;

            //for (int p = 1; p < timestep + 1; p++)
            //{
            //    moduleOnTimeLine[p - 1] = NetworkList[NetworkNameIndex[networkName]]["Mod_" + p][int.Parse(NodeSelected[p])][1];
            //    float connectivitySum = 0;
            //    for (int j = 0; j < nodeNumber; j++)
            //    {
            //        connectivity = float.Parse(NetworkList[NetworkNameIndex[networkName]]["Time_" + p][int.Parse(NodeSelected[p])][j]);
            //        if (connectivity > threshold)
            //            connectivitySum = connectivitySum + Mathf.Abs(connectivity);
            //    }
            //    ConnectivityArray[p - 1] = connectivitySum;
            //}

            //foreach (Transform child in +)
            //{
            //    if (child.name.Contains("TimeStep"))
            //    {
            //        int timeStep = int.Parse(child.name.Substring(8));
            //        GameObject Module = child.Find("Module").gameObject;
            //        Module.SetActive(true);
            //        Module.GetComponent<Renderer>().material.color = ColorCoding[moduleOnTimeLine[timeStep - 1]];

            //        GameObject connectivityTemp = child.Find("Connectivity").gameObject;
            //        connectivityTemp.SetActive(true);
            //        connectivityTemp.transform.localPosition = new Vector3(0, ConnectivityArray[timeStep - 1] * 2, 0);
            //    }
            //}
        
    

    //void PosConnectivityMod()
    //{
    //    float connectivity = 0;
    //    for (int p = 1; p < timestep + 1; p++)
    //    {
    //        for (int i = 0; i < moduleCount; i++)
    //        {
    //            ModulePattern[(i + 1).ToString()]= 0;
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
    //        float connectivityOffset = 0.5f;
    //        foreach (KeyValuePair<string, float> EachModulaStrength in sortedModule)
    //        {
    //            string moduleName = EachModulaStrength.Key;
    //            float connectivityStrength = Mathf.Abs(EachModulaStrength.Value);
    //            ModuleStat = Instantiate(ConnectivityPref, new Vector3(0f, connectivityOffset + connectivityStrength/2, 0), Quaternion.identity);
    //            connectivityOffset = connectivityOffset + connectivityStrength;
    //            ModuleStat.GetComponent<Renderer>().material.color = ColorCoding[moduleName];
    //            Vector3 temPos = ModuleStat.transform.position;
    //            ModuleStat.transform.parent = this.gameObject.transform.Find("TimeStep" + p);
    //            ModuleStat.transform.localPosition = temPos;
    //            ModuleStat.transform.name = "ModuleStat";
    //            ModuleStat.transform.localScale = new Vector3(0.3f, connectivityStrength, 1);
    //            allPosModuleStat.Add(ModuleStat);
    //        }
    //    }
    //}

    //void NegConnectivityMod()
    //{
    //    float connectivity = 0;
    //    for (int p = 1; p < timestep + 1; p++)
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
    //                if (connectivity > negThresholdMin && connectivity < negThresholdMax)
    //                {
    //                    moduleNumber = NetworkList[NetworkNameIndex[networkName]]["Mod_" + p][j + 1][1];
    //                    ModulePattern[moduleNumber] = ModulePattern[moduleNumber] + connectivity;
    //                }

    //            }
    //        }
    //        var sortedModule = from pair in ModulePattern orderby pair.Value ascending select pair;
    //        float connectivityOffset = -3.5f;
    //        foreach (KeyValuePair<string, float> EachModulaStrength in sortedModule)
    //        {
    //            string moduleName = EachModulaStrength.Key;
    //            float connectivityStrength = EachModulaStrength.Value;
    //            ModuleStat = Instantiate(ConnectivityPref, new Vector3(0f, connectivityOffset + connectivityStrength/2, 0), Quaternion.identity);
    //            connectivityOffset = connectivityOffset + connectivityStrength;
    //            ModuleStat.GetComponent<Renderer>().material.color = ColorCoding[moduleName];
    //            Vector3 temPos = ModuleStat.transform.position;
    //            ModuleStat.transform.parent = this.gameObject.transform.Find("TimeStep" + p);
    //            ModuleStat.transform.localPosition = temPos;
    //            ModuleStat.transform.name = "ModuleStat";
    //            ModuleStat.transform.localScale = new Vector3(0.3f, connectivityStrength , 1);
    //            allNegModuleStat.Add(ModuleStat);
    //        }
    //    }
    //}

    void hideConnMod()
    {
        for (int i = 0; i < 4; i++)
            RegionModuleName[i].SetActive(false);
        foreach (Transform child in Children)
        {
            if (child.name.Contains("TimeStep"))
            {
                Transform[] ModConn = child.GetComponentsInChildren<Transform>();
                foreach (Transform mod in ModConn)
                {
                    if (mod.name.StartsWith("Module"))
                        mod.gameObject.SetActive(false);
                }
                //    string moduleName = 
                //    GameObject Module = child.Find("Module*").gameObject;
                //    Module.SetActive(false);
                //GameObject connectivityTemp = child.Find("Connectivity").gameObject;
                //connectivityTemp.SetActive(false);
            }

        }
    }

    //void ClearAllPosModuleStat()
    //{
    //    foreach (GameObject mod in allPosModuleStat)
    //        if (mod != null)
    //            Destroy(mod);
    //}
    //void ClearAllNegModuleStat()
    //{
    //    foreach (GameObject mod in allNegModuleStat)
    //        if (mod != null)
    //            Destroy(mod);
    //}
}
