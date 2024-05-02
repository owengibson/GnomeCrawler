using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GnomeCrawler
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _volumePanel;

        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.Menu.performed += ToggleVolumePanel;
        }

        private void ToggleVolumePanel(InputAction.CallbackContext ctx)
        {
            if (_volumePanel.activeInHierarchy)
            {
                _volumePanel.SetActive(false);
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
            }
            else
            {
                _volumePanel.SetActive(true);
                EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            }
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }
    }
}
