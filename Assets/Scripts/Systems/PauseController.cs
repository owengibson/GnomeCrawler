using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class PauseController : MonoBehaviour
    {
        void Update()
        {
            //check for player input

            GameState currentGameState = GameStateManager.Instance.CurrentGameState;
            GameState newGameState = currentGameState == GameState.Gameplay
                ? GameState.Paused
                : GameState.Gameplay;

            GameStateManager.Instance.SetState(newGameState);
        }
    }
}
