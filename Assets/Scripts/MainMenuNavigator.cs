using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GnomeCrawler
{
    public class MainMenuNavigator : MonoBehaviour
    {
        private bool _canOpenMenu = true;

        private PlayerControls _playerControls;

        [SerializeField] private GameObject _volumePanel;
        [SerializeField] private GameObject _volumeSlider;
        [SerializeField] private GameObject _firstSelected;
        private bool _panelOpen = true;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.Menu.performed += ToggleVolumePanel;
        }

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_firstSelected);
        }

        public void PlayGame()
        {
            SceneManager.LoadScene("Level Generation");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OpenSettings()
        {
            _volumePanel.SetActive(true);
        }

        private void ToggleVolumePanel(InputAction.CallbackContext ctx)
        {
            //if (!_canOpenMenu)
            //{
            //    return;
            //}
            if (_volumePanel.activeInHierarchy)
            {
                _volumePanel.SetActive(false);
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
            }
            //else
            //{
            //    _volumePanel.SetActive(true);
            //    EventSystem.current.SetSelectedGameObject(_volumeSlider);
            //    EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            //}
        }
    }
}
