using GnomeCrawler.Deckbuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    public class Ability : MonoBehaviour
    {
        public CardSO Card;
        [SerializeField] private List<StringFloatPair> values = new List<StringFloatPair>();

        public void InitialiseCard(CardSO card)
        {
            Card = card;
            values = card.AbilityValues;
        }
    }
}
