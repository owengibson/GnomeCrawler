using GnomeCrawler.Rooms;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class WaterReset : MonoBehaviour
    {
        private Analytics analyticsScript;
        private EnemySpawning _enemySpawning;
        [SerializeField] private Transform _resetTransform;


        private void Start()
        {
            analyticsScript = GetComponent<Analytics>();
            _enemySpawning = transform.parent.GetComponentInChildren<EnemySpawning>();
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
            StartCoroutine(RespawnPlayer(other));
            /*string triggerName = gameObject.name;
            analyticsScript.TrackTriggerEntry(triggerName);*/
        }

        private IEnumerator RespawnPlayer(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable component)) yield break;

            Vector3 freezePos = other.transform.position;
            freezePos.y = transform.position.y + 3;
            other.transform.position = freezePos;

            yield return new WaitForSeconds(0.5f);

            Vector3 position = other.transform.position;
            position.y = transform.position.y + 3;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 5, NavMesh.AllAreas))
            {
                component.TakeDamage(1);
                other.enabled = false;
                other.transform.position = hit.position;
                other.enabled = true;
            }

            yield return null;
        }

        private void SolidifyWater()
        {
            Collider collider = GetComponent<Collider>();
            collider.isTrigger = !collider.isTrigger;
        }
    }
}
