using DG.Tweening;
using GnomeCrawler.Systems;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GnomeCrawler
{
    public class MainMenuNavigator : MonoBehaviour
    {
        [SerializeField] private GameObject _titleScreenCanvas;
        [SerializeField] private GameObject _signWithButtons;
        [SerializeField] private Animator _mainCamAnimator;
        [SerializeField] private GameObject _signButtonsFirst;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _settingsPanelFirst;
        [SerializeField] private float _animDuration = 0.5f;

        private bool _titleOpened;
        private bool _canOpenMenu = true;
        private PlayerControls _playerControls;


        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.Menu.performed += ToggleVolumePanel;
        }
        private void Update()
        {
            if (Input.anyKeyDown)
            {
                _titleScreenCanvas.SetActive(false);
                _mainCamAnimator.SetTrigger("PlayAnimation");
                if (!_titleOpened)
                {
                    StartCoroutine(EnableButtonsCoroutine(1));
                }
            }
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
            _signWithButtons.SetActive(false);
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            _settingsPanel.transform.localScale = Vector3.zero;
            _settingsPanel.SetActive(true);
            _settingsPanel.transform.DOScale(Vector3.one, _animDuration).SetEase(Ease.OutBack).SetUpdate(true);
            EventSystem.current.SetSelectedGameObject(_settingsPanelFirst);
        }

        private void CloseSettingsPanel()
        {
            _settingsPanel.transform.DOScale(Vector3.zero, _animDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                _settingsPanel.SetActive(false);
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
            });
            _signWithButtons.SetActive(true);
            _eventSystem.SetSelectedGameObject(_signButtonsFirst);
        }


        public void PlayGame()
        {
            SceneManager.LoadScene("Level Generation");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private IEnumerator EnableButtonsCoroutine(float delay)
        {
            _titleOpened = true;
            yield return new WaitForSeconds(delay);
            _signWithButtons.SetActive(true);
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
    }
}
