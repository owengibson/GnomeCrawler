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


        public void SetState(GameState newGameState)
        {
            if (newGameState == CurrentGameState) return;
            switch (newGameState)
            {
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                default:
                    break;
            }


            CurrentGameState = newGameState;
            
        }

        private void OnEnable()
        {
            EventManager.OnGameStateChanged += SetState;
        }
        private void OnDisable()
        {
            EventManager.OnGameStateChanged -= SetState;
        }
    }
}
