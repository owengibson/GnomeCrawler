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
                    if (Vector3.Distance(collider.transform.position, transform.position) < _shockwaveSize - 0.5f) return;

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
            _shockwaveSize = 0.5f;
        }

        private void OnDrawGizmos()
        {
            Color color1 = Color.yellow;
            Color color2 = Color.red;
            color1.a = 0.5f;
            color2.a = 0.5f;

            Gizmos.color = color1;
            Gizmos.DrawSphere(transform.position, _shockwaveSize);

            Gizmos.color = color2;
            Gizmos.DrawSphere(transform.position, _shockwaveSize - 0.5f);
        }
    }
}
