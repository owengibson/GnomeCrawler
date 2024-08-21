using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GnomeCrawler.Deckbuilding
{
    public class ActivatableCardGalleryElement : MonoBehaviour
    {
        [ShowInInspector]
        public CardSO Card { private set; get; }

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _durationText;
        

        public void SetCard(CardSO card)
        {
            Card = card;
            Initialise(card);
        }

        private void Initialise(CardSO card)
        {
            if (card == null) return;

            _icon.sprite = card.Icon;
            _durationText.text = card.ActiveDuration.ToString() + "s";
        }

        [Button]
        private void ResetCardUI()
        {
            _icon.sprite = null;
            _durationText.text = "00s";

            Card = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Initialise(Card);
        }
#endif
    }
}
