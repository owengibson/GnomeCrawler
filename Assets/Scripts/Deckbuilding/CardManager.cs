using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GnomeCrawler.Systems;
using DG.Tweening;

namespace GnomeCrawler.Deckbuilding
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private List<CardSO> _allCards;
        [Space]

        [SerializeField] private List<CardSO> _deck;
        [SerializeField] private Transform _cardUIContainer;
        [SerializeField] private GameObject _cardPrefab;
        [Space]

        [Header("Parameters")]
        [SerializeField] private int _handSize = 3;
        [SerializeField] private int _numberOfCardsToDraw = 3;


        private void Start()
        {
            
        }

        private List<CardSO> DrawHand()
        {
            return DrawCards(_deck, _handSize);
        }

        private void InstantiateCards(List<CardSO> cards)
        {
            foreach(Transform child in _cardUIContainer)
                Destroy(child.gameObject);

            foreach (CardSO card in cards)
            {
                GameObject newCardGO = Instantiate(_cardPrefab, _cardUIContainer);
                CardUI newCard = newCardGO.GetComponent<CardUI>();
                newCard.SetCard(card);
            }
            EventSystem.current.SetSelectedGameObject(_cardUIContainer.GetChild(0).gameObject);
        }

        private List<CardSO> DrawCards(List<CardSO> pool, int noOfCardsToDraw)
        {
            List<CardSO> output = new List<CardSO>();
            for (int i = 0; i < noOfCardsToDraw; i++)
            {
                int randomNum = UnityEngine.Random.Range(0, pool.Count);
                while (output.Contains(pool[randomNum]))
                {
                    randomNum = UnityEngine.Random.Range(0, pool.Count);
                }
                output.Add(pool[randomNum]);
            }


            return output;
        }

        public void DrawAndDisplayCards()
        {
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(DrawCards(_allCards, _numberOfCardsToDraw));
        }

        private void AddCardToDeck(CardSO card)
        {
            _deck.Add(card);
        }

        private void OnEnable()
        {
            EventManager.OnRoomCleared += DrawAndDisplayCards;
            EventManager.OnCardChosen += AddCardToDeck;
            EventManager.GetNewHand += DrawHand;

        }
        private void OnDisable()
        {
            EventManager.OnRoomCleared -= DrawAndDisplayCards;
            EventManager.OnCardChosen -= AddCardToDeck;
            EventManager.GetNewHand -= DrawHand;
        }
    }
}
