using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GnomeCrawler
{
    public class TutorialSkip : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _teleportParticleSystem;

        private void OnTriggerEnter(Collider other)
        {
            _teleportParticleSystem.Play();
        }

        private void OnTriggerExit(Collider other)
        {
            _teleportParticleSystem.Stop();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (_teleportParticleSystem.isEmitting) return;

            Debug.LogWarning("Teleport player");
            SceneManager.LoadScene(1);
        }
    }
}
