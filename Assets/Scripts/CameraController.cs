using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera rocketCamera;
    public Camera mainCamera;
    
    void Start()
    {
        if (rocketCamera == null)
        {
            rocketCamera = GameObject.FindGameObjectWithTag("RocketCamera").GetComponent<Camera>();
        }
        if (mainCamera == null)
        {
            mainCamera = this.GetComponent<Camera>();
        }

        rocketCamera.rect = new Rect(0, 0, 0.5f, 1);
        mainCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
    }

    void Update()
    {
        
    }
}
