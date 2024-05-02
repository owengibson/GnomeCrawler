using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GnomeCrawler
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _volumePanel;
        [SerializeField] private GameObject _volumeSlider;

        private bool _canOpenMenu = true;

        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.Menu.performed += ToggleVolumePanel;
        }

        private void ToggleVolumePanel(InputAction.CallbackContext ctx)
        {
            if (!_canOpenMenu)
            {
                return;
            }
            if (_volumePanel.activeInHierarchy)
            {
                _volumePanel.SetActive(false);
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
            }
            else
            {
                _volumePanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_volumeSlider);
                EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            }
        }

        private void StopMenus()
        {
            _volumePanel.SetActive(false);
            _canOpenMenu = false;
        }

        private void OnEnable()
        {
            _playerControls.Enable();
            EventManager.OnPlayerKilled += StopMenus;
        }

        private void OnDisable()
        {
            _playerControls.Disable();
            EventManager.OnPlayerKilled -= StopMenus;
        }
    }
}
