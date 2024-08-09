using DG.Tweening;
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
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _firstSelectable;
        [SerializeField] private float _animDuration = 0.5f;

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
            if (_settingsPanel.activeInHierarchy)
            {
                CloseSettingsPanel();
            }
            else
            {
                OpenSettingsPanel();
            }
        }

        private void OpenSettingsPanel()
        {
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            _settingsPanel.transform.localScale = Vector3.zero;
            _settingsPanel.SetActive(true);
            _settingsPanel.transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.OutBack).SetUpdate(true);
            EventSystem.current.SetSelectedGameObject(_firstSelectable);
        }

        private void CloseSettingsPanel()
        {
            _settingsPanel.transform.DOScale(Vector3.zero, _animDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                _settingsPanel.SetActive(false);
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
            });
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void StopMenus()
        {
            _settingsPanel.SetActive(false);
            _canOpenMenu = false;
        }

        public void ToggleSettingsPanel(bool activate)
        {
            if (activate)
            {
                OpenSettingsPanel();
            }
            else
            {
                CloseSettingsPanel();
            }
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
