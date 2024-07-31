using GnomeCrawler.Deckbuilding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Systems
{
    public class EventManager
    {
        public static Action<CardSO> OnCardChosen;
        public static Action<GameObject, float> OnCardChosenAnimation;
        public static Action<int> OnRoomStarted;
        public static Action OnRoomCleared;
        public static Action<GameObject> OnEnemyKilled;
        public static Action OnPlayerKilled;
        public static Action<GameState> OnGameStateChanged;
        public static Action<CardSO> OnCardActivated;
        public static Action<CardSO> OnCardDeactivated;
        public static Action OnHandDrawn;
        public static Action<List<CardSO>> OnHandApproved;

        public static Action<float> OnPlayerHit;
        public static Action<float> OnPlayerLifeSteal;
        public static Action<float> OnPlayerHurtFromAbility;
        public static Action OnSwimActivated;
        public static Action<float> OnWeaponSizeChanged;
        public static Action<bool> OnAttackAbilityToggle;
        public static Action<float, GameObject> OnPlayerAttacked;
        public static Action<float> OnShieldHit;

        public static Action<int> OnTutoialPopupQuery;
        public static Action<Objective, int> OnObjectiveChange;
        public static Action OnEnteredBossRoom;
        public static Action<bool> OnChooseInversion;

        public static Action<CardAnimationStatus> OnCardAnimationStatusChange;

        public static Func<CardSO> GetSelectedActivatableCard;
        public static Func<StatsSO> GetPlayerStats;
        public static Func<bool> IsPlayerTargetable;
        public static Func<bool> IsShieldActive;

    }
}
