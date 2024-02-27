using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Simple object that rotates stuff around
public class Rotate : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_speed = default;

    void Update()
    {
        transform.Rotate( m_speed * Time.deltaTime, Space.World);
    }
}
