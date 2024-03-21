using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class SpawnRats : MonoBehaviour
    {
        [SerializeField] private GameObject _ratsPrefab;

        void Start()
        {
            Invoke("InstantiateRat", 1);
        }

        private void InstantiateRat()
        {
            Instantiate(_ratsPrefab, gameObject.transform);
        }

    }
}
