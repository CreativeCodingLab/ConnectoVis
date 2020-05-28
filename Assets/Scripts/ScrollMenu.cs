using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollMenu : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public List<string> FirstSelectedRegions = new List<string>();
    //public List<string> SecondSelectedRegions = new List<string>();

    private List<string> previousFirstSelectedRegions = new List<string>();
    //private List<string> previousSecondSelectedRegions = new List<string>();
    private DataLoader dataLoader;
    private Builder builder;
    //private OverViewSceneManager sceneManager;
    private GameObject _ScrollTextWithColorBox;
    private GameObject _ScrollText;
    private string[] ModuleList;
    private string[] BrainRegionList;
    private string[] FolderNames;
    private string scrollMenu;
    private bool isFirstUpdate = true;
    private List<GameObject> LegendItems = new List<GameObject>();
    private GameObject[] NetworkParents;
    private GameObject[] mainNetwork;
    private Dictionary<string, Color> ColorCoding = new Dictionary<string, Color>();
    private List<string> selectedNetworks = new List<string>();
    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        //_propBlock = new MaterialPropertyBlock();
        scrollMenu = this.transform.name;
        dataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
        builder = GameObject.Find("Builder").GetComponent<Builder>();
        //sceneManager = GameObject.Find("SceneManager").GetComponent<OverViewSceneManager>();
        //_ScrollTextWithColorBox = Resources.Load("Prefabs/ListItemButtonWithColorBox") as GameObject;
        _ScrollText = Resources.Load("Prefabs/ListItemButton") as GameObject;
        _ScrollTextWithColorBox = Resources.Load("Prefabs/ListItemButtonWithColorBox") as GameObject;
        
    }

    void Update()
    {
        if (isFirstUpdate)
        {
            NetworkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
            ModuleList = dataLoader.colorCodingList.ToArray();
            BrainRegionList = dataLoader.brainRegionList.ToArray();
            FirstSelectedRegions = NetworkParents[0].GetComponent<SingleNetwork>().SelectedRegions;
            ColorCoding = dataLoader.colorCodingGlobal;
            FolderNames = dataLoader.FolderNames;
            if (scrollMenu == "Module")
            {
                for (int i = 0; i < ModuleList.Length; i++)
                {
                    GameObject ScrollText = Instantiate(_ScrollTextWithColorBox, new Vector3(0, 0, 0), Quaternion.identity);
                    LegendItems.Add(ScrollText);
                    ScrollText.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = "Mod " + ModuleList[i];
                    ScrollText.transform.Find("ColorBoxParent").Find("ColorBox").GetComponent<Image>().color = ColorCoding[ModuleList[i]];
                    Vector3 scale = ScrollText.transform.localScale;
                    Vector3 TempPosition = ScrollText.transform.localPosition;
                    ScrollText.transform.name = ModuleList[i];
                    ScrollText.transform.tag = "Module";
                    ScrollText.transform.parent = this.transform;
                    ScrollText.transform.localPosition = TempPosition;
                    ScrollText.transform.localScale = scale;
                }
            }

            if (scrollMenu == "FirstRegionList" )
            {
                for (int i = 0; i < BrainRegionList.Length; i++)
                {
                    GameObject ScrollText = Instantiate(_ScrollText, new Vector3(0, 0, 0), Quaternion.identity);
                    LegendItems.Add(ScrollText);
                    ScrollText.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = BrainRegionList[i];
                    Vector3 scale = ScrollText.transform.localScale;
                    Vector3 TempPosition = ScrollText.transform.localPosition;
                    ScrollText.transform.name = BrainRegionList[i];
                    ScrollText.transform.tag = "FirstRegionList";
                    ScrollText.transform.parent = this.transform;
                    ScrollText.transform.localPosition = TempPosition;
                    ScrollText.transform.localScale = scale;
                }
            }
            else if (scrollMenu == "NetworkList")
            {
                for (int i = 0; i < FolderNames.Length; i++)
                {
                    GameObject ScrollText = Instantiate(_ScrollText, new Vector3(0, 0, 0), Quaternion.identity);
                    LegendItems.Add(ScrollText);
                    ScrollText.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = FolderNames[i];
                    Vector3 scale = ScrollText.transform.localScale;
                    Vector3 TempPosition = ScrollText.transform.localPosition;
                    ScrollText.transform.name = FolderNames[i];
                    ScrollText.transform.tag = "Network";
                    ScrollText.transform.parent = this.transform;
                    ScrollText.transform.localPosition = TempPosition;
                    ScrollText.transform.localScale = scale;
                    if (i < builder.SelectedNetworks.Count)
                        ScrollText.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 0, 0);
                }
            }
            isFirstUpdate = false;
        }
       
        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                FirstSelectedRegions = NetworkParents[0].GetComponent<SingleNetwork>().SelectedRegions;
                if (result.gameObject.tag == "FirstRegionList")
                {
                    string regionName = result.gameObject.name;
                    //Color buttonColor = result.gameObject.transform.Find("ColorBox").GetComponent<Image>().color;
                    Color textColor = result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color;
                    if (textColor == Color.white)
                    {
                        Color highlightColor = new Color(1, 0, 0);
                        result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = highlightColor;

                        if (!FirstSelectedRegions.Contains(regionName))
                            FirstSelectedRegions.Add(regionName);
                    }
                    else
                    {
                        Color highlightColor = new Color(1, 1, 1);
                        result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = highlightColor;

                        if (FirstSelectedRegions.Contains(regionName))
                            FirstSelectedRegions.Remove(regionName);
                    }
                    if (!previousFirstSelectedRegions.SequenceEqual(FirstSelectedRegions))
                    {
                        foreach (GameObject networkParent in NetworkParents)
                            networkParent.GetComponent<SingleNetwork>().SelectedRegions = FirstSelectedRegions;
                        previousFirstSelectedRegions = FirstSelectedRegions;
                    }
                }

                if (result.gameObject.tag == "Network")
                {
                    string Network = "Connectome_"+result.gameObject.name+"_D";
                    Color textColor = result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color;
                    selectedNetworks = builder.SelectedNetworks;
                    //if (textColor == Color.white)
                    //{
                    //    mainNetwork = GameObject.FindGameObjectsWithTag("Network");
                    //    foreach(GameObject network in mainNetwork)
                    //    {
                    //        network.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1,1,1);
                    //    }
                    //    Color highlightColor = new Color(1, 0, 0);
                    //    result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = highlightColor;
                    //    builder.mainNetwork = Network;
                    //    if (NetworkParents[0].GetComponent<SingleNetwork>().SelectedRegions.Count == 0)
                    //    {
                    //        builder.putFourAside();
                    //        //GameObject.Find("Sorting").GetComponent<Sorting>().nodeSelected = string.Empty;
                    //        foreach (GameObject networkParent in NetworkParents)
                    //            networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule = string.Empty;
                    //    }

                    //}
                    //else
                    //{
                    //    Color highlightColor = new Color(1, 1, 1);
                    //    result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = highlightColor;
                    //    builder.mainNetwork = string.Empty;
                    //    foreach (GameObject networkParent in NetworkParents)
                    //        networkParent.GetComponent<SingleNetwork>().SelectedRegions = new List<string>();
                    //    GameObject RegionList = GameObject.Find("FirstRegionList");
                    //    foreach(Transform region in RegionList.transform)
                    //        region.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 1, 1);
                    //    foreach (GameObject networkParent in NetworkParents)
                    //        networkParent.GetComponent<SingleNetwork>().NodeSelectedForConnectivityandModule = string.Empty;
                    //    //GameObject.Find("Sorting").GetComponent<Sorting>().nodeSelected = string.Empty; 
                    //}
                    if (textColor == Color.white)
                    {
                        Color highlightColor = new Color(1, 0, 0);
                        result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = highlightColor;

                        if (!selectedNetworks.Contains(Network))
                            selectedNetworks.Add(Network);
                    }
                    else
                    {
                        Color highlightColor = new Color(1, 1, 1);
                        result.gameObject.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = highlightColor;

                        if (selectedNetworks.Contains(Network))
                            selectedNetworks.Remove(Network);
                    }
                }
            }
        }
    }
}
