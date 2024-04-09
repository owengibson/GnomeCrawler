using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GnomeCrawler.Player
{
    public class PlayerCombat : CombatBrain
    {
        public float PoisionTickTime;
        private List<GameObject> _damagedGameObjects;
        private bool _isInvincible = false;
        private PlayerStateMachine _stateMachine;
        [SerializeField] private Slider _healthbarSlider;
        [SerializeField] private GameObject hat;

        private void Start()
        {
            base.InitialiseVariables();
            _stateMachine = gameObject.GetComponent<PlayerStateMachine>();
            _damagedGameObjects = new List<GameObject>();
            _healthbarSlider.maxValue = _maxHealth;
            _healthbarSlider.value = CurrentHealth;
        }

        private void Update()
        {
            base.InternalUpdate();
            if (_isInvincible)
            {
                _stateMachine.IsInvincible = true;
                //hat.SetActive(false);
            }
            else
            {
                _stateMachine.IsInvincible = false;
                //hat.SetActive(true);
            }
        }

        protected override void CheckForRaycastHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponSize, _layerMask))
            {
                if (hit.transform.TryGetComponent(out IDamageable damageable) && !_damagedGameObjects.Contains(hit.transform.gameObject))
                {
                    //print("hit " + hit.transform.gameObject);
                    float damage = _stats.GetStat(Stat.Damage);
                    if (Random.Range(0, 100) <= _stats.GetStat(Stat.CritChance))
                    {
                        damage *= _stats.GetStat(Stat.CritDamageMultiplier);
                        Debug.Log(damage);
                    }
                    damageable.TakeDamage(damage);
                    _damagedGameObjects.Add(hit.transform.gameObject);
                }
            }
        }

        public override void TakeDamage(float amount)
        {
            if (_isInvincible) return;
            if (Random.Range(0,100) <= _stats.GetStat(Stat.BlockChance)) return;

            _stateMachine.IsFlinching = true;
            base.TakeDamage(amount);
            _healthbarSlider.value = CurrentHealth;
        }

        public override void StartDealDamage()
        {
            _damagedGameObjects.Clear();
            base.StartDealDamage();
        }
		
        private void AddHandToStats(List<CardSO> hand)
        {
            _stats.ResetCards();

            foreach (CardSO card in hand)
            {
                _stats.AddCard(card);
                if (card.UpgradedStat.Key == Stat.Health)
                {
                    float currentHealthRatio = CurrentHealth / _maxHealth;
                    _maxHealth = _stats.GetStat(Stat.Health);
                    CurrentHealth = _maxHealth * currentHealthRatio;

                    _healthbarSlider.maxValue = _maxHealth;
                    _healthbarSlider.transform.localScale = new Vector3(_maxHealth / 10, _healthbarSlider.transform.localScale.y, _healthbarSlider.transform.localScale.z);
                    _healthbarSlider.value = CurrentHealth;
                }
            }

            EventManager.OnHandDrawn?.Invoke();
        }

        private void OnApplicationQuit()
        {
            _stats.ResetCards();
        }

        public override void Die()
        {
            base.Die();
        }

        public void StartIFrames()
        {
            _isInvincible = true;
        }

        public void StopIFrames()
        {
            _isInvincible = false;
        }

        private StatsSO GetPlayerStats()
        {
            return _stats;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "PoisonArea")
            {
                PoisionTickTime -= Time.deltaTime;

                if (PoisionTickTime <= 0)
                {
                    TakeDamage(1);
                    PoisionTickTime = 2.0f;
                }
            }
        }

        private void OnEnable()
        {
            EventManager.OnHandApproved += AddHandToStats;
            EventManager.GetPlayerStats += GetPlayerStats;
        }

        private void OnDisable()
        {
            EventManager.OnHandApproved -= AddHandToStats;
            EventManager.GetPlayerStats -= GetPlayerStats;
        }
    }
}
