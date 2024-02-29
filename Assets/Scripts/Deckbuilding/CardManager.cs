using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GnomeCrawler.Systems;

namespace GnomeCrawler.Deckbuilding
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private CardSO[] _allCards;
        [Space]

        [SerializeField] private List<CardSO> _deck;
        [SerializeField] private Transform _cardUIContainer;
        [SerializeField] private GameObject _cardPrefab;

        private bool _isFirstDraw = true;
        private int _noOfCardsPerDraw = 3;

        private void Start()
        {
            DrawAndDisplayCards();
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

        private List<CardSO> DrawCards(List<CardSO> pool)
        {
            if (_isFirstDraw)
            {
                return DrawFirstCards();
            }

            List<CardSO> output = new List<CardSO>();
            for (int i = 0; i < _noOfCardsPerDraw; i++)
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
            InstantiateCards(DrawCards(_deck));
        }

        private List<CardSO> DrawFirstCards()
        {
            List<CardSO> output = new List<CardSO> ();
            foreach (CardCategory category in Enum.GetValues(typeof(CardCategory)))
            {
                CardSO card = _deck[UnityEngine.Random.Range(0, _deck.Count)];
                while (card.Category != category)
                {
                    card = _deck[UnityEngine.Random.Range(0, _deck.Count)];
                }
                output.Add(card);
            }

            _isFirstDraw = false;
            return output;
        }

        private void OnEnable()
        {
            EventManager.OnRoomCleared += DrawAndDisplayCards;
        }
        private void OnDisable()
        {
            EventManager.OnRoomCleared -= DrawAndDisplayCards;
        }
    }
}
