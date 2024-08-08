using DG.Tweening;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    public class OnboardingCardManager : MonoBehaviour
    {
        private CardManager _onboardingCardManager;

        [InfoBox("Place cards in order they should be drawn", InfoMessageType.Warning)]
        [SerializeField] private CardSO[] _cardsToDraw;
        [Space]

        [SerializeField] private GameObject _cardManager;
        [SerializeField] private GameObject[] _cards;
        [SerializeField] private GameObject[] _animCards;
        [SerializeField] private GameObject[] _choiceAnimCards;

        private PlayerControls _playerControls;

        #region Enable/Disable

        private void OnEnable()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.HandQuickview.performed += ToggleHandQuickview;
            _playerControls.Player.HandQuickview.canceled += ToggleHandQuickview;
            Debug.Log(_playerControls);
        }

        private void OnDisable()
        {
            _playerControls.Player.HandQuickview.performed -= ToggleHandQuickview;
            _playerControls.Player.HandQuickview.canceled -= ToggleHandQuickview;
            _playerControls.Disable();
        }

        #endregion

        private void Awake()
        {
            _onboardingCardManager = GetComponent<CardManager>();
        }

        public void DrawCard(int cardIndex)
        {
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            List<CardSO> hand = new List<CardSO>() { _cardsToDraw[cardIndex] };
            _onboardingCardManager.InstantiateCards(hand, false);
            _onboardingCardManager.SetCurrentHand(hand);
        }

        public void ResetCardManager()
        {
            _cardManager.SetActive(true);
            gameObject.SetActive(false);
        }

        private void ToggleHandQuickview(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            Debug.Log("Quick View");
            if (ctx.performed)
            {
                _onboardingCardManager.handQuickView.SetActive(true);
            }
            else if (ctx.canceled)
                _onboardingCardManager.handQuickView.SetActive(false);
        }

        /*#if UNITY_EDITOR
                // Dev testing
                private void Update()
                {
                    if (Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        DrawCard(0);
                    }
                    else if (Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        DrawCard(1);
                    }
                }
        #endif*/
    }
}
