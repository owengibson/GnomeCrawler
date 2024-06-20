using GnomeCrawler.Player;
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
            if (other.TryGetComponent(out IDamageable component))
            {
                StartCoroutine(RespawnPlayer(other, component));
            }
            
            /*string triggerName = gameObject.name;
            analyticsScript.TrackTriggerEntry(triggerName);*/
        }

        private IEnumerator RespawnPlayer(Collider playerCollider, IDamageable iDamageable)
        {
            PlayerStateMachine stateMachine = playerCollider.gameObject.GetComponent<PlayerStateMachine>();
            stateMachine.enabled = false;
            Vector3 freezePos = playerCollider.transform.position;
            freezePos.y = transform.position.y + 2;
            playerCollider.transform.position = freezePos;

            yield return new WaitForSeconds(0.5f);

            iDamageable.TakeDamage(1);
            playerCollider.enabled = false;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(freezePos, out hit, 5, NavMesh.AllAreas))
            {  
                playerCollider.transform.position = hit.position; 
            }
            else
            {
                playerCollider.transform.position = _resetTransform.position;
            }

            playerCollider.enabled = true;
            stateMachine.enabled = true;

            yield return null;
        }

        private void SolidifyWater()
        {
            Collider collider = GetComponent<Collider>();
            collider.isTrigger = !collider.isTrigger;
        }
    }
}
