using GnomeCrawler.Deckbuilding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GnomeCrawler
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private CardSO[] _defenseCards;
        [SerializeField] private CardSO[] _attackCards;
        [SerializeField] private CardSO[] _utilityCards;
        [Space]

        [SerializeField] private List<CardSO> _currentPool;
        [SerializeField] private Transform _cardUIContainer;
        [SerializeField] private GameObject _cardPrefab;

        private bool _areFirstCards = true;
        private int _noOfCardsPerHand = 3;

        private void InstantiateCards(List<CardSO> cards)
        {
            foreach (CardSO card in cards)
            {
                GameObject newCardGO = Instantiate(_cardPrefab, _cardUIContainer);
                CardUI newCard = newCardGO.GetComponent<CardUI>();
                newCard.SetCard(card);
            }
        }

        private List<CardSO> ChooseCards(List<CardSO> pool)
        {
            List<CardSO> output = new List<CardSO>();
            List<int> usedNumbers = new List<int>();
            for (int i = 0; i < _noOfCardsPerHand; i++)
            {
                int randomNum = Random.Range(0, pool.Count - 1);
                while (usedNumbers.Contains(randomNum))
                {
                    randomNum = Random.Range(0, pool.Count - 1);
                }
                output.Add(pool[randomNum]);
            }


            return output;
        }

        private void ChooseAndDisplayCards()
        {
            
        }
    }
}
