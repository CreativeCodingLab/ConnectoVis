using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

public class DataLoader : MonoBehaviour
{

    public string m_Path;
    public string[] DataFolder;
    public Dictionary<string, string[][]> NetworkData;
    public Dictionary<string, Color> colorCodingGlobal = new Dictionary<string, Color>();
    public Dictionary<string, int> NetworkTimeStep = new Dictionary<string, int>();
    public List<Dictionary<string, string[][]>> NetworkGlobalList = new List<Dictionary<string, string[][]>>();
    public Dictionary<string, int> NetworkNameIndex = new Dictionary<string, int>();
    public string[] FolderNames;
    public int timeStep = 0;
    public int totalNodeNumber = 32;
    private const int subStringStartChar = 17;
    public List<string> colorCodingList = new List<string>();
    public List<string> brainRegionList = new List<string>();

    void Awake()
    {
        DataFolder = System.IO.Directory.GetDirectories(m_Path, "*", System.IO.SearchOption.TopDirectoryOnly);
    }

    private void Start()
    {
        FolderNames = GetNetworkFolderName(DataFolder);
    }
    void ReadCSVFile(string NetworkPath, string filePath, string FileType)
    {
        filePath = filePath.Substring(subStringStartChar);
        TextAsset initialdata = Resources.Load<TextAsset>(filePath);
        string[] data = initialdata.ToString().Split('\n');
        List<string[]> dataMatrix = new List<string[]>();

        for (int i = 0; i < data.Length; i++)
        {
            if (!string.IsNullOrEmpty(data[i]) && !string.IsNullOrWhiteSpace(data[i]))
            {
                dataMatrix.Add(data[i].Split(','));
                for (int j = 0; j < dataMatrix[i].Length; j++)
                    dataMatrix[i][j] = string.Concat(dataMatrix[i][j].Where(c => !char.IsWhiteSpace(c)));
            }
        }
        NetworkData.Add(FileType, dataMatrix.ToArray());
    }

    public string[] GetNetworkFolderName(string[] NetworkFolders)
    {
        string[] NetworkFolderName = new string[NetworkFolders.Length];
        string networkName;
        for (int i = 0; i < NetworkFolders.Length; i++)
        {
            networkName = new DirectoryInfo(NetworkFolders[i]).Name;
            NetworkFolderName[i] = networkName;
            NetworkNameIndex.Add("Connectome_"+networkName+"_D", i);
        }
        return NetworkFolderName;
    }


    public List<Dictionary<string, string[][]>> LoadNetworks(string[] NetworkFolders)
    {
        List<Dictionary<string, string[][]>> NetworkList = new List<Dictionary<string, string[][]>>();
        foreach (string Network in NetworkFolders)
        {
            Dictionary<string, string> filePath = new Dictionary<string, string>();
            string NetworkName = new DirectoryInfo(Network).Name;
            NetworkData = new Dictionary<string, string[][]>();
            filePath.Add("anatomy", Network + "/topologies/anatomy");
            filePath.Add("label", Network + "/labels/label");
            filePath.Add("LookupTable", Network + "/atlas/LookupTable");
            filePath.Add("NW", Network + "/edges/NW");
            string dynamicDir = Network + "/atlas/DynamicModularity";
            if (System.IO.Directory.Exists(dynamicDir))
                timeStep = new DirectoryInfo(dynamicDir).GetFiles("*.csv").Count();
            else timeStep = 0;
            NetworkTimeStep.Add(NetworkName, timeStep);
            
            for (int i = 1; i < timeStep + 1; i++)
            {
                string filename = Network + "/edges/" + i;
                if (System.IO.File.Exists(filename + ".csv"))
                {
                    filePath.Add("Time_" + i, filename);
                }
                    
                filename = Network + "/atlas/DynamicModularity/" + i;
                if (System.IO.File.Exists(filename + ".csv"))
                    filePath.Add("Mod_" + i, filename);
            }
            ///Reading in the data
            foreach (KeyValuePair<string, string> file in filePath)
                if (System.IO.File.Exists(file.Value + ".csv"))
                {
                    ReadCSVFile(Network, file.Value, file.Key);
                }
                else
                    NetworkData.Add(file.Key, null);

            if (System.IO.Directory.Exists(Network + "/atlas/DynamicModularity"))
            {
                for (int i = 1; i < timeStep + 1; i++)
                {
                    if (NetworkData["Mod_" + i] != null)
                    {
                        for (int j = 1; j < NetworkData["Mod_" + i].Length; j++)
                        {
                            colorCodingList.Add(NetworkData["Mod_" + i][j][1]);
                        }
                    }
                }
            }
            NetworkList.Add(NetworkData);
            for (int i = 1; i < NetworkData["LookupTable"].Length; i++)
                if (!brainRegionList.Contains(NetworkData["LookupTable"][i][2]))
                    brainRegionList.Add(NetworkData["LookupTable"][i][2]);
        }
        colorCodingGlobal = new Dictionary<string, Color>();
        colorCodingList = colorCodingList.Distinct().ToList();
        string[] colorList = null;
        string ColorListPath = m_Path + "/ColorList";

        if (System.IO.File.Exists(ColorListPath + ".csv"))
        {
            ColorListPath = ColorListPath.Substring(subStringStartChar);
            TextAsset colorListData = Resources.Load<TextAsset>(ColorListPath);
            colorList = colorListData.ToString().Split('\n');
        }

        for (int i = 0; i < colorCodingList.Count; i++)
        {
            Color32 new_color;
            string[] rgb_string = colorList[i].Split(',');
            new_color.r = (byte)int.Parse(rgb_string[0]);
            new_color.g = (byte)int.Parse(rgb_string[1]);
            new_color.b = (byte)int.Parse(rgb_string[2]);

            colorCodingGlobal.Add(colorCodingList[i], new Color32(new_color.r, new_color.g, new_color.b, 255));
        }
        NetworkGlobalList = NetworkList;
        return NetworkList;
    }
}
