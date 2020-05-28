using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicControl : MonoBehaviour
{

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    private DataLoader dataLoader;
    private bool isFirstUpdate = true;
    private GameObject timeStepText;
    private GameObject[] NetworkParents;
    private int timeStep = 16;
    void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        dataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate)
        {
            NetworkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
            isFirstUpdate = false;
            timeStep = dataLoader.timeStep;
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
                if (result.gameObject.name == "Play")
                {
                    result.gameObject.GetComponent<Image>().color = new Color(1, 0, 0, 1);
                    result.gameObject.transform.parent.Find("Pause").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Left_Arrow").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Right_Arrow").GetComponent<Image>().color = Color.white;
                    foreach (GameObject networkParent in NetworkParents)
                        networkParent.GetComponent<SingleNetwork>().isPause = false;
                }
                else if (result.gameObject.name == "Pause")
                {
                    result.gameObject.GetComponent<Image>().color = Color.red;
                    result.gameObject.transform.parent.Find("Play").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Left_Arrow").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Right_Arrow").GetComponent<Image>().color = Color.white;

                    foreach (GameObject networkParent in NetworkParents)
                        networkParent.GetComponent<SingleNetwork>().isPause = true;
                }
                else if (result.gameObject.name == "Left_Arrow")
                {
                    result.gameObject.GetComponent<Image>().color = Color.red;
                    result.gameObject.transform.parent.Find("Pause").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Play").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Right_Arrow").GetComponent<Image>().color = Color.white;
                    foreach (GameObject networkParent in NetworkParents)
                    {
                        int currentTimeStep = networkParent.GetComponent<SingleNetwork>().DynamicCurrentTime;
                        if (currentTimeStep > 1)
                            networkParent.GetComponent<SingleNetwork>().DynamicCurrentTime = currentTimeStep - 1;
                    }
                }
                else if (result.gameObject.name == "Right_Arrow")
                {
                    result.gameObject.GetComponent<Image>().color = Color.red;
                    result.gameObject.transform.parent.Find("Pause").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Left_Arrow").GetComponent<Image>().color = Color.white;
                    result.gameObject.transform.parent.Find("Play").GetComponent<Image>().color = Color.white;
                    foreach (GameObject networkParent in NetworkParents)
                    {
                        int currentTimeStep = networkParent.GetComponent<SingleNetwork>().DynamicCurrentTime;
                        if (currentTimeStep < timeStep)
                            networkParent.GetComponent<SingleNetwork>().DynamicCurrentTime = currentTimeStep + 1;
                    }
                }
               
            }
        }
    }
}
