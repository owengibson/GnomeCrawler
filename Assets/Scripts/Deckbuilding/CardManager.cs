using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GnomeCrawler.Systems;
using DG.Tweening;
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
        [Space]

        [Header("Parameters")]
        [SerializeField] private int _handSize = 3;
        [SerializeField] private int _numberOfCardsToDraw = 3;
        [Space]

        [SerializeField] private List<CardSO> _hand;


        private void Start()
        {
            
        }

        private void DrawAndDisplayNewHand()
        {
            _hand = DrawCards(_deck, _handSize, true);
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(_hand, false);
            _handApproveButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_handApproveButton);
        }

        private List<CardSO> GetCurrentHand()
        {
            return _hand;
        }

        private void InstantiateCards(List<CardSO> cards, bool isSelection)
        {
            foreach(Transform child in _cardUIContainer)
                Destroy(child.gameObject);

            foreach (CardSO card in cards)
            {
                GameObject newCardGO = Instantiate(_cardPrefab, _cardUIContainer);
                CardUI newCard = newCardGO.GetComponent<CardUI>();
                newCard.SetCard(card);
                if (!isSelection)
                {
                    newCardGO.GetComponent<Button>().enabled = false;
                }
            }

            if (!isSelection)
                EventSystem.current.SetSelectedGameObject(_cardUIContainer.GetChild(0).gameObject);
        }

        private List<CardSO> DrawCards(List<CardSO> pool, int noOfCardsToDraw, bool isDrawingFromDeck)
        {
            List<CardSO> output = new List<CardSO>();
            for (int i = 0; i < noOfCardsToDraw; i++)
            {
                int randomNum = UnityEngine.Random.Range(0, pool.Count);
                if (isDrawingFromDeck)
                {
                    while (output.Contains(pool[randomNum]))
                    {
                        randomNum = UnityEngine.Random.Range(0, pool.Count);
                    }
                    output.Add(pool[randomNum]);
                }
                else
                {
                    while (output.Contains(pool[randomNum]) || _deck.Contains(pool[randomNum]))
                    {
                        randomNum = UnityEngine.Random.Range(0, pool.Count);
                    }
                    output.Add(pool[randomNum]);
                }

            }


            return output;
        }

        public void DrawAndDisplayCards()
        {
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(DrawCards(_allCards, _numberOfCardsToDraw, false), true);
        }

        private void AddCardToDeck(CardSO card)
        {
            _deck.Add(card);
        }

        public void ApproveHand()
        {
            EventManager.OnHandApproved?.Invoke(_hand);

            foreach (Transform child in _cardUIContainer)
                Destroy(child.gameObject);

            EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);

            _handApproveButton.SetActive(false);
        }

        private void OnEnable()
        {
            EventManager.OnRoomStarted += DrawAndDisplayNewHand;
            EventManager.OnRoomCleared += DrawAndDisplayCards;
            EventManager.OnCardChosen += AddCardToDeck;
        }
        private void OnDisable()
        {
            EventManager.OnRoomStarted -= DrawAndDisplayNewHand;
            EventManager.OnRoomCleared -= DrawAndDisplayCards;
            EventManager.OnCardChosen -= AddCardToDeck;
        }
    }
}
