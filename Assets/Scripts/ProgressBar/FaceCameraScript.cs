using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    public Camera FaceCamera;

    private void Update()
    {
        transform.LookAt(FaceCamera.transform, Vector3.up);
    }
}