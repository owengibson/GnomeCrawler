using Dan.Main;
using DG.Tweening;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GnomeCrawler
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField] private float _gameOverDelay = 1.5f;
        [SerializeField] private GameObject _button;

        private void ShowGameOverScreen()
        {
            StartCoroutine(WaitForGameOverScreen());
        }

        private IEnumerator WaitForGameOverScreen()
        {
            yield return new WaitForSecondsRealtime(_gameOverDelay);
            GetComponent<Canvas>().enabled = true;
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            _button.SetActive(true);
            canvasGroup.DOFade(1, 1);
            EventSystem.current.SetSelectedGameObject(_button);
        }

        public void GameOverButtonPressed()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnEnable()
        {
            EventManager.OnPlayerKilled += ShowGameOverScreen;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerKilled -= ShowGameOverScreen;
        }
    }
}
