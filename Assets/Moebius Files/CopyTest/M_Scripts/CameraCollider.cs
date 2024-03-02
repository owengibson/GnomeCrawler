using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Collider for the camera
/// in charge of notifying the CameraController when the camera
/// enters a portal, for toggling colors
public class CameraCollider : MonoBehaviour
{
    [SerializeField]
    private CameraController m_camera = null;
    [SerializeField]
    private Transform m_target = null;
    [SerializeField]
    private LayerMask m_portal = default;

    private void OnTriggerEnter(Collider other)
    {
        if((m_portal.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            //Debug.Log("Collision with portal");
            m_camera.ToggleColors();
            other.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((m_portal.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            other.gameObject.GetComponent<Renderer>().enabled = true;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
        Vector3 pos = m_camera.transform.position;
        pos.y = m_target.position.y;
        transform.position = pos;
    }
}
