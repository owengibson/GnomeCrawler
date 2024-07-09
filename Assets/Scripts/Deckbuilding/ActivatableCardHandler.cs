using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GnomeCrawler.Deckbuilding
{
    public class ActivatableCardHandler : MonoBehaviour
    {
        [SerializeField] private StatsSO _stats;

        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.Ability.performed += ActivateSelectedCard;
        }

        private void ActivateSelectedCard(InputAction.CallbackContext context)
        {
            ActivateCard(EventManager.GetSelectedActivatableCard?.Invoke());
            if (TutorialManager.StaticPopupIndex != 7) return;
            EventManager.OnRemoveTutoialPopupQuery?.Invoke(7);
            EventManager.OnTutoialPopupQuery?.Invoke(8);
        }

        private void ActivateCard(CardSO card)
        {
            if (!card.IsActivatableCard) return;

            if (card.Type != CardType.Ability)
            {
                _stats.ActivateCard(card);
            }

            EventManager.OnCardActivated?.Invoke(card);
            StartCoroutine(RemoveCardAfterDuration(card));
        }

        private IEnumerator RemoveCardAfterDuration(CardSO card)
        {
            yield return new WaitForSeconds(card.ActiveDuration);
            _stats.RemoveActiveCard(card);
            EventManager.OnCardDeactivated?.Invoke(card);
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
