using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GnomeCrawler.Systems;
using UnityEngine.UI;

namespace GnomeCrawler.Deckbuilding
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private List<CardSO> _allCards;
        [Space]

        [SerializeField] private List<CardSO> _deck;
        [Space]

        [Header("GameObject/Prefab References")]
        [SerializeField] private Transform _cardUIContainer;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private GameObject _handApproveButton;
        [SerializeField] private GameObject _handQuickview;
        [Space]

        [Header("Parameters")]
        [SerializeField] private int _handSize = 3;
        [SerializeField] private int _numberOfCardsToDraw = 3;
        [Space]

        [SerializeField] private List<CardSO> _hand;

        private PlayerControls _playerControls;


        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.HandQuickview.performed += ToggleHandQuickview;
            _playerControls.Player.HandQuickview.canceled += ToggleHandQuickview;
        }

        private void DrawAndDisplayNewHand()
        {
            _hand = DrawCards(_deck, _handSize, true);
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(_hand, false, _cardUIContainer);
            _handApproveButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_handApproveButton);
        }

        private void InstantiateCards(List<CardSO> cards, bool isSelection, Transform container)
        {
            foreach(Transform child in container)
                Destroy(child.gameObject);

            foreach (CardSO card in cards)
            {
                GameObject newCardGO = Instantiate(_cardPrefab, container);
                CardUI newCard = newCardGO.GetComponent<CardUI>();
                newCard.SetCard(card);
                if (!isSelection)
                {
                    newCardGO.GetComponent<Button>().enabled = false;
                }
            }

            if (isSelection)
                Debug.Log(EventSystem.current.gameObject);
                EventSystem.current.SetSelectedGameObject(_cardUIContainer.GetChild(0).gameObject);
        }

        private List<CardSO> DrawCards(List<CardSO> pool, int noOfCardsToDraw, bool isDrawingFromDeck)
        {
            List<CardSO> output = new List<CardSO>();
            for (int i = 0; i < noOfCardsToDraw; i++)
            {
                int randomNum = Random.Range(0, pool.Count);
                if (isDrawingFromDeck)
                {
                    while (output.Contains(pool[randomNum]))
                    {
                        randomNum = Random.Range(0, pool.Count);
                    }
                    output.Add(pool[randomNum]);
                }
                else
                {
                    while (output.Contains(pool[randomNum]) || _deck.Contains(pool[randomNum]))
                    {
                        randomNum = Random.Range(0, pool.Count);
                    }
                    output.Add(pool[randomNum]);
                }

            }

            return output;
        }

        public void DrawAndDisplayCards()
        {
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(DrawCards(_allCards, _numberOfCardsToDraw, false), true, _cardUIContainer);
        }

        private void AddCardToDeck(CardSO card)
        {
            _deck.Add(card);

            if (Random.Range(0f, 100f) <= EventManager.GetPlayerStats?.Invoke().GetStat(Stat.Luck))
            {
                _deck.Add(DrawLuckCard());
            }
        }

        private CardSO DrawLuckCard()
        {
            int randomNum = Random.Range(0, _allCards.Count);
            while (_deck.Contains(_allCards[randomNum]) || _allCards[randomNum].IsActivatableCard)
            {
                randomNum = Random.Range(0, _allCards.Count);
            }
            return _allCards[randomNum];
        }

        public void ApproveHand()
        {
            EventManager.OnHandApproved?.Invoke(_hand);

            foreach (Transform child in _cardUIContainer)
                Destroy(child.gameObject);

            EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);

            _handApproveButton.SetActive(false);
            InstantiateCards(_hand, false, _handQuickview.transform);
        }

        private void ToggleHandQuickview(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                _handQuickview.SetActive(true);
            else if (ctx.canceled)
                _handQuickview.SetActive(false);
        }

        private void OnEnable()
        {
            EventManager.OnRoomStarted += DrawAndDisplayNewHand;
            EventManager.OnRoomCleared += DrawAndDisplayCards;
            EventManager.OnCardChosen += AddCardToDeck;

            _playerControls.Enable();
        }
        private void OnDisable()
        {
            EventManager.OnRoomStarted -= DrawAndDisplayNewHand;
            EventManager.OnRoomCleared -= DrawAndDisplayCards;
            EventManager.OnCardChosen -= AddCardToDeck;

            _playerControls.Disable();
        }
    }
}
