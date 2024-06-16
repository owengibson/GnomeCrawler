using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace GnomeCrawler
{
    public class FireFeet : Ability
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private GameObject _weapon;

        private ParticleSystem.Particle[] _particles;
        private List<GameObject> _damagedTargets;

        private void OnEnable()
        {
            EventManager.OnAttackAbilityToggle?.Invoke(true);
            _weapon.SetActive(false);

            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Play();

            _particles = new ParticleSystem.Particle[1000];
        }

        private void FixedUpdate()
        {
            if (_particles == null) return;
            _particleSystem.GetParticles(_particles);

            if (_particles.Length <= 0) return;

            foreach (var particle in _particles)
            {
                RaycastHit hit;
                if (Physics.SphereCast(particle.position, particle.GetCurrentSize(_particleSystem) / 4f, Vector3.up, out hit, 0.5f, LayerMask.GetMask("Enemy")))
                {
                    IDamageable target;
                    if (hit.collider.gameObject.TryGetComponent(out target))
                    {
                        StartCoroutine(DamageTarget(hit.collider.gameObject));
                    }
                }
            }
        }
        /*private void OnDrawGizmos()
        {
            foreach (var particle in _particles)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(particle.position, particle.GetCurrentSize(_particleSystem) / 3);
            }
        }*/

        private IEnumerator DamageTarget(GameObject target)
        {
            if (_damagedTargets.Contains(target)) yield break;

            _damagedTargets.Add(target);
            target.GetComponent<IDamageable>().TakeDamage(Card.AbilityValues[0].value);
            yield return new WaitForSeconds(1);
            _damagedTargets.Remove(target);
        }

        private void OnDisable()
        {
            _particleSystem.Clear();
            _particleSystem.Stop();
            _particleSystem.gameObject.SetActive(false);

            EventManager.OnAttackAbilityToggle?.Invoke(false);
            _weapon.SetActive(true);
        }
    }
}
