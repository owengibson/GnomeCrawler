using GnomeCrawler.Deckbuilding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GnomeCrawler
{
    public class Shockwave : MonoBehaviour
    {
        private bool _hasDealtDamage;
        private float _shockwaveSize;
        
        private float _shockwaveSpeed;

        private ParticleSystem _particleSystem;
        void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _shockwaveSpeed = _particleSystem.main.startSpeed.constant;
            _shockwaveSize = 1f;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_particleSystem.isStopped)
            {
                gameObject.SetActive(false);
            }
            ExpandOverlapSphere();
            CheckForShockwaveHit();
        }

        private void CheckForShockwaveHit()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _shockwaveSize, LayerMask.GetMask("Player"));
         
            if (hitColliders.Length <= 0) return;

            foreach (var collider in hitColliders)
            {
                if (collider.transform.TryGetComponent(out IDamageable damageable) && !_hasDealtDamage)
                {
                    if (Vector3.Distance(collider.transform.position, transform.position) < _shockwaveSize - 1) return;

                    damageable.TakeDamage(2f, gameObject);
                    _hasDealtDamage = true;
                }
            }
        }

        private void ExpandOverlapSphere()
        {
            _shockwaveSize += _shockwaveSpeed * Time.deltaTime;
        }

        private void OnEnable()
        {
            _hasDealtDamage = false;
            _shockwaveSize = 1f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, _shockwaveSize);
        }
    }
}
