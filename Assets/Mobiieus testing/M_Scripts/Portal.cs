using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The portals for the racing game fire an event when the player enters them 
/// to update the checkpoint count
/// the toggling of colors happens when the **camera** enters the portal (cf CameraCollider.cs)
public class Portal : MonoBehaviour
{
    [SerializeField]
    private bool m_isEnd = false;

    public RacingController Parent { get; set; }
    public bool IsEnd { get { return m_isEnd; } }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Parent.OnEnter(this);
        }
    }
}
