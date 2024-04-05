using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Systems
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        public GameState CurrentGameState { get; private set; }

        public void SetState(GameState newGameState)
        {
            if (newGameState == CurrentGameState) return;
            switch (newGameState)
            {
                case GameState.Gameplay:
                    //Time.timeScale = 1f;
                    //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.1f);
                    StartCoroutine(LerpTimeScale(0, 1, 0.5f));
                    break;

                case GameState.Paused:
                    //Time.timeScale = 0f;
                    //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 0.1f);
                    StartCoroutine(LerpTimeScale(1, 0, 0.5f));
                    break;

                default:
                    break;
            }

            CurrentGameState = newGameState;
        }

        private IEnumerator LerpTimeScale(float from,  float to, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(from, to, counter / duration);

                yield return null;
            }
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
