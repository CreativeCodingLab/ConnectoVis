using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomInOut : MonoBehaviour
{
    private float cameraScrollSpeed = 2f;
    public Camera _cam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraAction();
    }

    void CameraAction()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
            _cam.transform.Translate(0, 0, cameraScrollSpeed * Input.GetAxis("Mouse ScrollWheel"), Space.Self);
    }
}
