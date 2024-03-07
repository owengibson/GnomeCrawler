using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GnomeCrawler.Deckbuilding
{
    public class ActivatableCardGalleryUI : MonoBehaviour
    {
        [SerializeField] private Transform _selectedSlot;
        [Space]

        [Header("Prefabs")]
        [SerializeField] private GameObject _emptyElement;
        [SerializeField] private GameObject _cardElement;

        private List<CardSO> _activatableCards = new List<CardSO>();
        [SerializeField] private int _galleryIndex = 0;
        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.CycleCards.performed += SelectNextCard;
        }

        [Button]
        private void SelectNextCard(InputAction.CallbackContext context)
        {
            _galleryIndex++;
            if (_galleryIndex == _activatableCards.Count)
            {
                _galleryIndex = 0;
            }
            RenderGalleryAtIndex(_galleryIndex);
        }

        private void AddCardToGallery(CardSO card)
        {
            if (!card.IsActivatableCard) return;
            _activatableCards.Add(card);

            if (_activatableCards.Count != 1) return;
            RenderGalleryAtIndex(0);
        }

        private void RemoveCardFromGallery(CardSO card)
        {
            if (!_activatableCards.Contains(card)) return;
            _activatableCards.Remove(card);
            Debug.Log($"Removed {card.Name} from list");

            if (_activatableCards.Count > 0)
            {
                RenderGalleryAtIndex(_galleryIndex);
            }
            else
            {
                RenderGalleryAtIndex(0);
            }

        }

        private void RenderGalleryAtIndex(int index)
        {
            foreach (Transform child in _selectedSlot)
                Destroy(child.gameObject);

            if (_activatableCards.Count == 0)
            {
                Instantiate(_emptyElement, _selectedSlot);
                return;
            }

            GameObject card = Instantiate(_cardElement, _selectedSlot);
            card.GetComponent<ActivatableCardGalleryElement>().SetCard(_activatableCards[index]);
        }

        private CardSO GetSelectedCard()
        {
            return _activatableCards[_galleryIndex];
        }

        private void OnEnable()
        {
            EventManager.OnCardChosen += AddCardToGallery;
            EventManager.OnCardActivated += RemoveCardFromGallery;
            EventManager.GetSelectedActivatableCard += GetSelectedCard;
            _playerControls.Enable();
        }
        private void OnDisable()
        {
            EventManager.OnCardChosen -= AddCardToGallery;
            EventManager.OnCardActivated -= RemoveCardFromGallery;
            EventManager.GetSelectedActivatableCard -= GetSelectedCard;
            _playerControls.Disable();
        }
    }
}
