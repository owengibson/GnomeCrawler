using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class RotateClouds : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        void Update()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
