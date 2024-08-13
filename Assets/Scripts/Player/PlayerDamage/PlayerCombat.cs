using Cinemachine.Utility;
using Dan.Main;
using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GnomeCrawler.Player
{
    public class PlayerCombat : CombatBrain
    {
        private float _healTickTime;
        [HideInInspector] public float PoisionTickTime;
        private List<GameObject> _damagedGameObjects;
        public bool _isInvincible = false;
        private PlayerStateMachine _stateMachine;
        [SerializeField] private Slider _healthbarSlider;
        [SerializeField] private GameObject _abilitiesGO;

        [SerializeField] private UnityEvent OnDamagedNoStun;

        [SerializeField] private Transform _spinWeaponOrigin;

        private void Start()
        {
            base.InitialiseVariables();
            _stateMachine = gameObject.GetComponent<PlayerStateMachine>();
            _damagedGameObjects = new List<GameObject>();
            _healthbarSlider.maxValue = _maxHealth;
            _healthbarSlider.value = CurrentHealth;
            _stats.ResetCards();
        }

        private void Update()
        {
            base.InternalUpdate();
            if (_isInvincible)
            {
                _stateMachine.IsInvincible = true;
            }
            else
            {
                _stateMachine.IsInvincible = false;
            }
        }

        protected override void CheckForRaycastHit()
        {
            RaycastHit hit;
            
            if (!_stateMachine.IsAttackDisabled)
            {
                if (Physics.Raycast(_originTransform.position, -_originTransform.up, out hit, _weaponSize, _layerMask))
                {
                    if (hit.transform.TryGetComponent(out IDamageable damageable) && !_damagedGameObjects.Contains(hit.transform.gameObject))
                    {
                        DamageHitTarget(hit, damageable);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(_spinWeaponOrigin.position, -_spinWeaponOrigin.up, out hit, _weaponSize, _layerMask))
                {
                    if (hit.transform.TryGetComponent(out IDamageable damageable) && !_damagedGameObjects.Contains(hit.transform.gameObject))
                    {
                        DamageHitTarget(hit, damageable);
                        Invoke("StartDealDamage", 0.25f);
                    }
                }
            }

        }

        private void DamageHitTarget(RaycastHit hit, IDamageable damageable)
        {
            //print("hit " + hit.transform.gameObject);
            float damage = _stats.GetStat(Stat.Damage);
            if (Random.Range(0, 100) <= _stats.GetStat(Stat.CritChance))
            {
                damage *= _stats.GetStat(Stat.CritDamageMultiplier);
                Debug.Log(damage);
            }
            bool? nullableIsPlayerTargetable = EventManager.IsPlayerTargetable?.Invoke();
            bool isPlayerTargetable = nullableIsPlayerTargetable == true || nullableIsPlayerTargetable == null;
            if (!isPlayerTargetable) damage *= 2;

            damageable.TakeDamage(damage, gameObject);
            EventManager.OnPlayerHit?.Invoke(damage);
            OnDamageConnected?.Invoke();
            _damagedGameObjects.Add(hit.transform.gameObject);
        }

        public override void TakeDamage(float amount, GameObject damager)
        {
            if (_isInvincible) return;
            if (Random.Range(0,100) <= _stats.GetStat(Stat.BlockChance)) return;

            EventManager.OnPlayerAttacked?.Invoke(amount, damager);

            if (EventManager.IsShieldActive?.Invoke() == true)
            {
                EventManager.OnShieldHit?.Invoke(amount);
                Debug.Log("shield hit");
                return;
            }

            base.TakeDamage(amount, damager);
            _stateMachine.IsFlinching = true;
            _healthbarSlider.value = CurrentHealth;
        }

        public void TakeDamageNoDeath(float amount)
        {
            float healthAfterDamage = CurrentHealth - amount;
            if (CurrentHealth == 1) return;
            else if (healthAfterDamage <= 0)
            {
                CurrentHealth = 1;
            }
            else if (healthAfterDamage >= 1)
            {
                CurrentHealth -= amount;
            }
            OnDamagedNoStun?.Invoke();
            _healthbarSlider.value = CurrentHealth;
        }

        public void TakeDamageNoStun(float amount, GameObject damager)
        {
            if (_isInvincible) return;
            if (Random.Range(0, 100) <= _stats.GetStat(Stat.BlockChance)) return;

            OnDamagedNoStun?.Invoke();
            if (EventManager.IsShieldActive?.Invoke() == true)
            {
                EventManager.OnShieldHit?.Invoke(amount);
                Debug.Log("shield hit");
                return;
            }
            _healthbarSlider.value = CurrentHealth;
            CurrentHealth -= amount;
            DamageTaken?.Invoke();
            OnDamagedNoStun.Invoke();

            if (CurrentHealth <= 0) Die();
            

            EventManager.OnPlayerAttacked?.Invoke(amount, damager);
        }

        public override void StartDealDamage()
        {
            _damagedGameObjects.Clear();
            base.StartDealDamage();
        }

        private void AddHandToStats(List<CardSO> hand)
        {

            _stats.ResetCards();
            foreach (Ability abiltyComponent in _abilitiesGO.GetComponents<Ability>())
            {
                abiltyComponent.enabled = false;
            }

            foreach (CardSO card in hand)
            {
                switch (card.Type)
                {
                    case CardType.Stat:
                        _stats.AddCard(card);
                        if (card.UpgradedStat.Key == Stat.Health)
                        {
                            float currentHealthRatio = CurrentHealth / _maxHealth;
                            _maxHealth = _stats.GetStat(Stat.Health);
                            CurrentHealth = _maxHealth * currentHealthRatio;

                            _healthbarSlider.maxValue = _maxHealth;
                            RectTransform rectTransform = _healthbarSlider.GetComponent<RectTransform>();
                            rectTransform.sizeDelta = new Vector2(_maxHealth * 60, rectTransform.sizeDelta.y);
                            _healthbarSlider.value = CurrentHealth;
                        }
                        break;
                    case CardType.Ability:
                        if (card.IsActivatableCard)
                            continue;

                        EnableAbility(card);
                        break;
                    case CardType.StatAndAbility:
                        _stats.AddCard(card);
                        if (card.UpgradedStat.Key == Stat.Health)
                        {
                            float currentHealthRatio = CurrentHealth / _maxHealth;
                            _maxHealth = _stats.GetStat(Stat.Health);
                            CurrentHealth = _maxHealth * currentHealthRatio;

                            _healthbarSlider.maxValue = _maxHealth;
                            RectTransform rectTransform = _healthbarSlider.GetComponent<RectTransform>();
                            rectTransform.sizeDelta = new Vector2(_maxHealth * 60, rectTransform.sizeDelta.y);
                            _healthbarSlider.value = CurrentHealth;
                        }
                        if (card.IsActivatableCard)
                            continue;

                        EnableAbility(card);
                        break;
                    default:
                        break;
                }
                
            }

            EventManager.OnHandDrawn?.Invoke();
        }

        private void EnableAbility(CardSO card)
        {
            var abilityComponent = _abilitiesGO.GetComponent(card.AbilityClassName);
            foreach (Ability ability in _abilitiesGO.GetComponentsInChildren<Ability>())
            {
                if (ability == abilityComponent)
                {
                    ability.InitialiseCard(card);
                    ability.enabled = true;
                }
            }
        }

        private void DisableAbility(CardSO card)
        {
            var abilityComponent = _abilitiesGO.GetComponent(card.AbilityClassName);
            foreach (Ability ability in _abilitiesGO.GetComponentsInChildren<Ability>())
            {
                if (ability == abilityComponent)
                {
                    ability.InitialiseCard(card);
                    ability.enabled = false;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _stats.ResetCards();
        }

        public override void Die()
        {
            // total deaths leaderboard
            string uid = SystemInfo.deviceUniqueIdentifier;
            int noOfDeaths = 0;
            Leaderboards.TotalDeaths.GetEntries(msg =>
            {
                var entry = System.Array.Find(msg, x => x.Username == uid);

                noOfDeaths = entry.Score;
                noOfDeaths++;
                LeaderboardManager.Instance.SetLeaderboardEntry(uid, noOfDeaths);
                Debug.Log("Leaderboard set pog");
            });

            EventManager.OnPlayerKilled?.Invoke();
            OnDeath?.Invoke();
            gameObject.GetComponent<CharacterController>().enabled = false;
            _stateMachine.enabled = false;
            IsDead = true;
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
            if (other.gameObject.tag == "HealArea")
            {
                _healTickTime -= Time.deltaTime;

                if (_healTickTime <= 0)
                {
                    HealPlayer(1);
                    _healTickTime = .2f;
                }
            }
        }

        private void ChangeWeaponSize(float size)
        {
            _weaponSize = 1.05f * size;
        }

        private void ChangeCanDealDamage(bool canDealDamage)
        {
            _canDealDamage = canDealDamage;
        }

        private void OnEnable()
        {
            EventManager.OnRoomStarted += PerRoomHeal;

            EventManager.OnHandApproved += AddHandToStats;
            EventManager.GetPlayerStats += GetPlayerStats;
            EventManager.OnPlayerLifeSteal += HealPlayer;
            EventManager.OnPlayerHurtFromAbility += TakeDamageNoDeath;
            EventManager.OnCardActivated += EnableAbility;
            EventManager.OnCardDeactivated += DisableAbility;
            EventManager.OnWeaponSizeChanged += ChangeWeaponSize;
            EventManager.OnAttackAbilityToggle += ChangeCanDealDamage;
        }

        private void OnDisable()
        {
            EventManager.OnRoomStarted -= PerRoomHeal;

            EventManager.OnHandApproved -= AddHandToStats;
            EventManager.GetPlayerStats -= GetPlayerStats;
            EventManager.OnPlayerLifeSteal -= HealPlayer;
            EventManager.OnPlayerHurtFromAbility -= TakeDamageNoDeath;
            EventManager.OnCardActivated -= EnableAbility;
            EventManager.OnCardDeactivated -= DisableAbility;
            EventManager.OnWeaponSizeChanged -= ChangeWeaponSize;
            EventManager.OnAttackAbilityToggle -= ChangeCanDealDamage;
        }

        public void HealPlayer(float amount)
        {
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);
            _healthbarSlider.value = CurrentHealth;
        }

        private void PerRoomHeal(int unUsed)
        {
            HealPlayer(4);
        }
    }
}
