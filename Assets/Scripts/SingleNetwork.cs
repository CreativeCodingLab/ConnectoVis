using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SingleNetwork : MonoBehaviour
{
    private GameObject _nodePrefab;
    private GameObject _edgePrefab;
    private List<GameObject> Nodes = new List<GameObject>();

    public List<GameObject> AllEdges = new List<GameObject>();
    public List<string> ModuleList = new List<string>();
    public bool isPause = false;
    public List<string> SelectedRegions = new List<string>();
    public bool ShowInNetworkConnections = false;
    private List<string> PreviousFirstSelectedRegions = new List<string>();
    public Dictionary<string, int> NodeRegionNumberDictionary = new Dictionary<string, int>();
    public Dictionary<int, string> NodeNumberRegionDictionary = new Dictionary<int, string>();
    public int DynamicCurrentTime = 1;
    public List<string> NodeSelectedForConnectivityandModule = new List<string>();
    public float posThresholdMin = 0f;
    public float posThresholdMax = 1f;
    public float negThresholdMin = -1f;
    public float negThresholdMax = 0f;
    //private float preThresholdMax = 1;
    //private float preThresholdMin = -1;
    private int _DynamicPreviousTime = 1;
    private int _timeStep = 0;
    private bool isPlayingDynamicData = false;
    private string _networkName = string.Empty;
    private string _representationType = "anatomy";
    private string TimeStage = "Time_1";
    private string[][] _edgesMatrix;
    private string _atlas = "Mod_1";
    private string RegionName = string.Empty;
    private Dictionary<string, string[][]> _networkData = new Dictionary<string, string[][]>();

    private Dictionary<string, Color> _colorCoding;
    private MaterialPropertyBlock _propBlock;
    private GameObject TitlePref;
    private GameObject Title;
    private void Awake()
    {
        TitlePref = Resources.Load("Prefabs/3DText") as GameObject;
    }

    void Start()
    {
        _propBlock = new MaterialPropertyBlock();
        BrainRegionNumberDictionary(_networkData);
        DrawNetwork(_networkData);
        //thresholdMax = preThresholdMax;
        //thresholdMin = preThresholdMin;
    }

    void Update()
    {
        if (!isPlayingDynamicData && !isPause)
            DynamicDataFunction(DynamicCurrentTime, _timeStep);

        if (!PreviousFirstSelectedRegions.SequenceEqual(SelectedRegions))
        {
            DynamicSingleStep(DynamicCurrentTime);
            PreviousFirstSelectedRegions = new List<string>();
            foreach (string region in SelectedRegions)
                PreviousFirstSelectedRegions.Add(region);
        }

        if (_DynamicPreviousTime != DynamicCurrentTime)
        {
            DynamicSingleStep(DynamicCurrentTime);
            _DynamicPreviousTime = DynamicCurrentTime;
        }
    }

    public void attachNetworkData(string atlas,Dictionary<string, string [][]> NetworkData, string NetworkName, GameObject NodePrefab, GameObject EdgePrefab, int TimeStep, Dictionary<string, Color> ColorCoding)
    {
        _nodePrefab = NodePrefab;
        _edgePrefab = EdgePrefab;
        _networkData = NetworkData;
        _networkName = NetworkName;
        _timeStep = TimeStep;
        _colorCoding = ColorCoding;
        _atlas = atlas;
    }

    private void BrainRegionNumberDictionary(Dictionary<string, string[][]> networkData)
    {
        for (int i = 1; i < networkData["LookupTable"].Length; i++)
            NodeRegionNumberDictionary.Add(networkData["LookupTable"][i][2], int.Parse(networkData["LookupTable"][i][0]));

        for (int i = 1; i < networkData["LookupTable"].Length; i++)
            NodeNumberRegionDictionary.Add(int.Parse(networkData["LookupTable"][i][0]), networkData["LookupTable"][i][2]);
    }

    private void DrawNetwork(Dictionary<string, string[][]>networkData)
    {
        Nodes = new List<GameObject>();
        ModuleList = new List<string>();
        Material nodeMaterial = new Material(Shader.Find("Custom/Node"));
        float alpha = 1;
        Title = Instantiate(TitlePref, new Vector3(-0.5f, 1.5f, 0), Quaternion.identity);
        Vector3 tempPosition1= Title.transform.position;
        Title.transform.parent = this.transform;
        Title.transform.localPosition = tempPosition1;
        Title.GetComponent<TextMesh>().text = _networkName;
        Title.transform.localScale = new Vector3(0.01f, 0.01f, 1);


        for (int row = 1; row < networkData[_representationType].Length; row++)
        {
            GameObject node = Instantiate(_nodePrefab, new Vector3(float.Parse(networkData[_representationType][row][1]), float.Parse(networkData[_representationType][row][2]), float.Parse(networkData[_representationType][row][3])), Quaternion.identity);
            RegionName = networkData[_atlas][row][1];
            node.GetComponent<Renderer>().material = nodeMaterial;
            node.GetComponent<Renderer>().GetPropertyBlock(_propBlock);

            node.transform.Find("Text").GetComponent<TextMesh>().text = NodeNumberRegionDictionary[row];
            node.transform.Find("Text").gameObject.SetActive(false);

            _propBlock.SetColor("_Color1", _colorCoding[RegionName]);
            _propBlock.SetColor("_Color2", _colorCoding[RegionName]);
            
            _propBlock.SetColor("_Color1", _colorCoding[RegionName]);
            _propBlock.SetColor("_Color2", _colorCoding[RegionName]);
            _propBlock.SetFloat("_Alpha1", alpha);
            _propBlock.SetFloat("_Alpha2", alpha);
            node.GetComponent<Renderer>().SetPropertyBlock(_propBlock);

            Vector3 tempPosition = node.transform.position;
            Quaternion tempRotation = node.transform.localRotation;
            node.transform.parent = this.transform;
            node.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            node.transform.localRotation = tempRotation;
            node.transform.localPosition = tempPosition;
            node.name = row.ToString();

            node.AddComponent<SingleNodeBehaviour>();

            Nodes.Add(node);
            if (!ModuleList.Contains(RegionName))
                ModuleList.Add(RegionName);
        }
    }

    private void UpdateNetwork(List<GameObject> nodes)
    {
        ModuleList = new List<string>();
        Material nodeMaterial = new Material(Shader.Find("Custom/Node"));
        float alpha = 1;
        float alphaTranparent = 0.1f;
        //string selectedModule = string.Empty;

        //if (NodeSelectedForConnectivityandModule != string.Empty)
        //{
        //    selectedModule = _networkData[_atlas][int.Parse(NodeSelectedForConnectivityandModule)][1];
        //}
        for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
        {
            RegionName = _networkData[_atlas][nodeIndex+1][1];
            nodes[nodeIndex].GetComponent<Renderer>().material = nodeMaterial;
            nodes[nodeIndex].GetComponent<Renderer>().GetPropertyBlock(_propBlock);
            
            if (SelectedRegions.Count > 0)
            {
                _propBlock.SetColor("_Color1", Color.gray);
                _propBlock.SetColor("_Color2", Color.gray);
                _propBlock.SetFloat("_Alpha1", alphaTranparent);
                _propBlock.SetFloat("_Alpha2", alphaTranparent);
                
                //if (selectedModule != string.Empty && RegionName == selectedModule)
                //{
                //    _propBlock.SetColor("_Color1", _colorCoding[RegionName]);
                //    _propBlock.SetColor("_Color2", _colorCoding[RegionName]);
                //    _propBlock.SetFloat("_Alpha1", 0.2f);
                //    _propBlock.SetFloat("_Alpha2", 0.2f);
                //}

                foreach (string region in SelectedRegions)
                {
                    if (nodeIndex + 1 == NodeRegionNumberDictionary[region])
                    {
                        _propBlock.SetColor("_Color1", _colorCoding[RegionName]);
                        _propBlock.SetColor("_Color2", _colorCoding[RegionName]);
                        _propBlock.SetFloat("_Alpha1", alpha);
                        _propBlock.SetFloat("_Alpha2", alpha);
                    }
                }
            }
            else
            {
                NodeSelectedForConnectivityandModule = new List<string>();
                //selectedModule = string.Empty;
                _propBlock.SetColor("_Color1", _colorCoding[RegionName]);
                _propBlock.SetColor("_Color2", _colorCoding[RegionName]);
                _propBlock.SetFloat("_Alpha1", alpha);
                _propBlock.SetFloat("_Alpha2", alpha);
            }
            
            nodes[nodeIndex].GetComponent<Renderer>().SetPropertyBlock(_propBlock);
        }
    }

    private void DrawAllEdges(float ThresholdMin, float ThresholdMax)
    {
        
        Vector3 startPosition = new Vector3();
        Vector3 endPosition = new Vector3();
        Vector3 scale = new Vector3();
        Vector3 offset = new Vector3();
        Vector3 edgePosition = new Vector3();
        float connectivityScale = 0;
        if (_networkData[TimeStage] != null)
        {
            _edgesMatrix = _networkData[TimeStage];
            connectivityScale = 0.01f;
        }
        for (int nodeNumber = 1; nodeNumber < _edgesMatrix.Length; nodeNumber++)
        {
            for (int i = 0; i < _edgesMatrix[nodeNumber].Length; i++)
            {
                float connectivity = float.Parse(_edgesMatrix[nodeNumber][i]);
                float absConnectivity = Mathf.Abs(connectivity);

                if (connectivity < ThresholdMax && connectivity > ThresholdMin && connectivity != 0)
                {
                    startPosition = new Vector3(Nodes[nodeNumber - 1].transform.position.x, Nodes[nodeNumber - 1].transform.position.y, Nodes[nodeNumber - 1].transform.position.z);
                    endPosition = new Vector3(Nodes[i].transform.position.x, Nodes[i].transform.position.y, Nodes[i].transform.position.z);
                    offset = endPosition - startPosition;
                    edgePosition = startPosition + offset / 2.0f;
                    scale = new Vector3(Mathf.Max(Mathf.Min(absConnectivity * connectivityScale, 0.01f), 0.0005f), offset.magnitude / 2.0f, Mathf.Max(Mathf.Min(absConnectivity * connectivityScale, 0.01f), 0.0005f));

                    GameObject edge = Instantiate(_edgePrefab, edgePosition, Quaternion.identity);

                    string edgeName = nodeNumber + "_" + (i + 1);
                    edge.transform.name = edgeName;
                    Quaternion tempRotation = edge.transform.localRotation;
                    edge.transform.localScale = scale;
                    edge.transform.parent = this.transform;
                    edge.transform.localRotation = tempRotation;
                    edge.transform.up = offset;
                    AllEdges.Add(edge);
                }
            }
        }
    }

    private void DrawEdgeforSingleNode(int nodeNumber, float ThresholdMin, float ThresholdMax)
    {
        Material edgeMaterial = new Material(Shader.Find("Custom/Node"));
        Vector3 startPosition = new Vector3();
        Vector3 endPosition = new Vector3();
        Vector3 scale = new Vector3();
        Vector3 offset = new Vector3();
        Vector3 edgePosition = new Vector3();
        Color32 edgeColor;
        float connectivityScale = 0;
        if (_networkData[TimeStage] != null)
        {
            _edgesMatrix = _networkData[TimeStage];
            connectivityScale = 0.2f;
        }
        for (int i = 0; i < _edgesMatrix[nodeNumber].Length; i++)
        {
            float connectivity = float.Parse(_edgesMatrix[nodeNumber][i]);
            float absConnectivity = Mathf.Abs(connectivity);

            if (connectivity < ThresholdMax && connectivity > ThresholdMin && connectivity != 0)
            {
                startPosition = new Vector3(Nodes[nodeNumber - 1].transform.position.x, Nodes[nodeNumber - 1].transform.position.y, Nodes[nodeNumber - 1].transform.position.z);
                endPosition = new Vector3(Nodes[i].transform.position.x, Nodes[i].transform.position.y, Nodes[i].transform.position.z);
                offset = endPosition - startPosition;
                edgePosition = startPosition + offset / 2.0f;
                scale = new Vector3(Mathf.Max(Mathf.Min(absConnectivity * connectivityScale, 0.3f), 0.01f), offset.magnitude / 2.0f, Mathf.Max(Mathf.Min(absConnectivity * connectivityScale, 0.3f), 0.01f));

                GameObject edge = Instantiate(_edgePrefab, edgePosition, Quaternion.identity);

                string edgeName = nodeNumber + "_" + (i + 1);
                edge.transform.name = edgeName;
                Quaternion tempRotation = edge.transform.localRotation;
                edge.transform.localScale = scale;
                edge.transform.parent = this.transform;
                edge.transform.localRotation = tempRotation;
                edge.transform.up = offset;
                AllEdges.Add(edge);

                Nodes[i].GetComponent<Renderer>().GetPropertyBlock(_propBlock);
                RegionName = _networkData[_atlas][i + 1][1];
                _propBlock.SetColor("_Color1", _colorCoding[RegionName]);
                _propBlock.SetColor("_Color2", _colorCoding[RegionName]);
                _propBlock.SetFloat("_Alpha1", 1);
                _propBlock.SetFloat("_Alpha2", 1);
                Nodes[i].GetComponent<Renderer>().SetPropertyBlock(_propBlock);
                if (connectivity > 0)
                    edgeColor = new Color32(255, 178, 95, 255);
                else
                    edgeColor = new Color32(133, 211, 248, 255);
                edge.GetComponent<Renderer>().material = edgeMaterial;
                edge.GetComponent<Renderer>().GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_Color1", edgeColor);
                _propBlock.SetColor("_Color2", edgeColor);
                edge.GetComponent<Renderer>().SetPropertyBlock(_propBlock);
            }
        }
    }

    private void DrawEdgeStartEndNode(int startNode, int endNode, float ThresholdMin, float ThresholdMax)
    {
        Vector3 startPosition = new Vector3();
        Vector3 endPosition = new Vector3();
        Vector3 scale = new Vector3();
        Vector3 offset = new Vector3();
        Vector3 edgePosition = new Vector3();
        float connectivityScale = 0;
        if (_networkData[TimeStage] != null)
        {
            _edgesMatrix = _networkData[TimeStage];
            connectivityScale = 0.01f;
        }

        float connectivity = float.Parse(_edgesMatrix[startNode][endNode-1]);
        float absConnectivity = Mathf.Abs(connectivity);

        if (connectivity < ThresholdMax && connectivity > ThresholdMin && connectivity != 0)
        {
            startPosition = new Vector3(Nodes[startNode - 1].transform.position.x, Nodes[startNode - 1].transform.position.y, Nodes[startNode - 1].transform.position.z);
            endPosition = new Vector3(Nodes[endNode-1].transform.position.x, Nodes[endNode-1].transform.position.y, Nodes[endNode-1].transform.position.z);
            offset = endPosition - startPosition;
            edgePosition = startPosition + offset / 2.0f;
            scale = new Vector3(Mathf.Max(Mathf.Min(absConnectivity * connectivityScale, 0.2f), 0.05f), offset.magnitude / 2.0f, Mathf.Max(Mathf.Min(absConnectivity * connectivityScale, 0.2f), 0.05f));

            GameObject edge = Instantiate(_edgePrefab, edgePosition, Quaternion.identity);

            string edgeName = startNode + "_" + endNode;
            edge.transform.name = edgeName;
            Quaternion tempRotation = edge.transform.localRotation;
            edge.transform.localScale = scale;
            edge.transform.parent = this.transform;
            edge.transform.localRotation = tempRotation;
            edge.transform.up = offset;
            AllEdges.Add(edge);
        }
    }

    private void DynamicDataFunction(int startTime, int endTime)
    {
        if (endTime > startTime)
            StartCoroutine(DrawDynamicNetwork(startTime, endTime));
    }

    public void DynamicSingleStep(int TimeStep)
    {
        DynamicCurrentTime = TimeStep;
        _atlas = "Mod_" + TimeStep;
        TimeStage = "Time_" + TimeStep;
        UpdateNetwork(Nodes);
        ClearAllEdges();
        //if (SelectedRegions.Count > 0)// && SecondSelectedRegions.Count == 0)
        //{
        //    foreach (string region in SelectedRegions)
        //        DrawEdgeforSingleNode(NodeRegionNumberDictionary[region], 0.1f, 1);
        //}
        
        if (ShowInNetworkConnections)
            showInNetworkConnections();
        else showOutNetworkConnections();  
    }

    IEnumerator DrawDynamicNetwork(int startTime, int endTime)
    {
        isPlayingDynamicData = true;
        for (int i = startTime; i < endTime + 1; i++)
        {
            if (isPause)
            {
                isPlayingDynamicData = false;
                DynamicCurrentTime = i - 1;
                _DynamicPreviousTime = i - 1;
                yield break;
            }
            else
            {
                TimeStage = "Time_" + i;
                _atlas = "Mod_" + i;
                ClearAllEdges();
                DynamicCurrentTime = i;
                _DynamicPreviousTime = i;
                UpdateNetwork(Nodes);
                //if (SelectedRegions.Count > 0)// && SecondSelectedRegions.Count == 0)
                //{
                //    foreach (string region in SelectedRegions)
                //        DrawEdgeforSingleNode(NodeRegionNumberDictionary[region], 0.1f, 1);
                //}
                if (ShowInNetworkConnections)
                    showInNetworkConnections();
                else showOutNetworkConnections();
                yield return new WaitForSeconds(2f);
            }
        }
        isPlayingDynamicData = false;
        DynamicCurrentTime = 1;
        yield return null;
    }

    private void ClearAllEdges()
    {
        foreach (GameObject edge in AllEdges)
            if (edge != null)
                Destroy(edge);
    }

    private void showInNetworkConnections()
    {
        if(SelectedRegions.Count == 1)
        {
            ClearAllEdges();
        }
        if(SelectedRegions.Count > 1)
        {
            for(int i = 0; i< SelectedRegions.Count; i++)
            {
                for (int j = i + 1; j < SelectedRegions.Count; j++)
                {
                    DrawEdgeStartEndNode(NodeRegionNumberDictionary[SelectedRegions[i]], NodeRegionNumberDictionary[SelectedRegions[j]], posThresholdMin, posThresholdMax);
                    DrawEdgeStartEndNode(NodeRegionNumberDictionary[SelectedRegions[i]], NodeRegionNumberDictionary[SelectedRegions[j]], negThresholdMin, negThresholdMax);
                }
            } 
        }
    }

    private void showOutNetworkConnections()
    {
        if (SelectedRegions.Count > 0)
        {
            foreach (string region in SelectedRegions)
            {
                DrawEdgeforSingleNode(NodeRegionNumberDictionary[region], posThresholdMin, posThresholdMax);
                DrawEdgeforSingleNode(NodeRegionNumberDictionary[region], negThresholdMin, negThresholdMax);
            }
                
        }
    }
}
