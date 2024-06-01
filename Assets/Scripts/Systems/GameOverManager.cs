using Dan.Main;
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
        [SerializeField] private GameObject _button;

        private void ShowGameOverScreen()
        {
            GetComponent<Canvas>().enabled = true;
            _button.SetActive(true);
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
