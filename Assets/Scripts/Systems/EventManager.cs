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
        public static Action OnRoomStarted;
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

        public static Func<CardSO> GetSelectedActivatableCard;
        public static Func<StatsSO> GetPlayerStats;
    }
}
