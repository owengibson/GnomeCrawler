using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GnomeCrawler
{
    public class ColliderUnityEventEmitter : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnEnter;
        [SerializeField] private UnityEvent OnStay;
        [SerializeField] private UnityEvent OnExit;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            OnEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            OnExit?.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            OnStay?.Invoke();
        }
    }
}
