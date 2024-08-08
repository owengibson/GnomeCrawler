using Dan.Main;
using DG.Tweening;
using GnomeCrawler.Systems;
using MoreMountains.Feedbacks;
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
        [SerializeField] private GameObject _feedbacks;

        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void ShowGameOverScreen()
        {
            _feedbacks.SetActive(true);
            StartCoroutine(WaitForGameOverScreen());
        }

        private IEnumerator WaitForGameOverScreen()
        {
            yield return new WaitForSecondsRealtime(_gameOverDelay);
            GetComponent<Canvas>().enabled = true;
            _button.SetActive(true);
            _canvasGroup.DOFade(1, 1);
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
