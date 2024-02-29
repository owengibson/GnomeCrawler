using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager _instance;
        public static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameStateManager();
                return _instance;
            }
        }

        public GameState CurrentGameState { get; private set; }

        private GameStateManager()
        {

        }

        public void SetState(GameState newGameState)
        {
            if (newGameState == CurrentGameState) return;

            CurrentGameState = newGameState;
            EventManager.OnGameStateChanged?.Invoke(newGameState);
        }
    }
}
