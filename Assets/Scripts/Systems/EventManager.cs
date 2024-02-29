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
        public static Action OnRoomCleared;
        public static Action<GameObject> OnEnemyKilled;
        public static Action<GameState> OnGameStateChanged;
    }
}
