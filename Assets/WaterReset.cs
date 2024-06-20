using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class WaterReset : MonoBehaviour
    {
        private Analytics analyticsScript;
        [SerializeField] private Transform _resetTransform;

        private void Start()
        {
            analyticsScript = GetComponent<Analytics>();
        }

        private void OnEnable()
        {
            EventManager.OnSwimActivated += SolidifyWater;
        }

        private void OnDisable()
        {
            EventManager.OnSwimActivated -= SolidifyWater;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable component))
            {
                component.TakeDamage(1, gameObject);
                other.enabled = false;
                other.transform.position = _resetTransform.position;
                other.enabled = true;
            }

            /*string triggerName = gameObject.name;
            analyticsScript.TrackTriggerEntry(triggerName);*/
        }

        private void SolidifyWater()
        {
            Collider collider = GetComponent<Collider>();
            collider.isTrigger = !collider.isTrigger;
        }
    }
}
