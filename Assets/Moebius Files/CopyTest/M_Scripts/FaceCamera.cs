using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Makes the transform always face the camera
public class FaceCamera : MonoBehaviour
{

    private Camera m_camera = null;
    public Camera Camera => m_camera ?? (m_camera = Camera.main);

    private void Start()
    {
        if (Camera == null)
        {
            Debug.LogError("Main Camera is not assigned and couldn't be found.");
        }
    }
    void Update()
    {
        if (Camera != null)
        {
            transform.forward = Camera.transform.forward;
        }
    }
}
