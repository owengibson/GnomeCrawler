using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    [HideInInspector] public Camera FaceCamera;

    private void Start()
    {
        FaceCamera = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(FaceCamera.transform, Vector3.up);
    }
}