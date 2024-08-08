using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class InstantiatePrefab : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        public void InstantiatePrefabAtPosition()
        {
            GameObject prefabPrefab = Instantiate(prefab, transform.position, Quaternion.identity);
        }

        public void DisconnectFromParent()
        {
            transform.parent = null;
        }
    }
}
