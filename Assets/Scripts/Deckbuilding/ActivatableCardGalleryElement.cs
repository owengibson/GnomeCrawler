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
        public CardSO Card { private set; get; }

        [SerializeField] private TextMeshProUGUI _numberText;
        [SerializeField] private TextMeshProUGUI _statText;
        [SerializeField] private TextMeshProUGUI _durationText;
        [SerializeField] private Image _backgroundImage;

        private readonly Color _defenseColour = new Color32(165, 255, 140, 157);
        private readonly Color _offenseColour = new Color32(255, 140, 140, 157);
        private readonly Color _utilityColour = new Color32(140, 198, 255, 157);

        public void SetCard(CardSO card)
        {
            Card = card;
            Initialise(card);
        }

        private void Initialise(CardSO card)
        {
            if (card == null) return;

            _numberText.text = card.UpgradedStat.Value.ToString() + (card.IsPercentUpgrade ? "%" : "");
            _statText.text = card.UpgradedStat.Key.ToString().ToUpper();
            _durationText.text = card.ActiveDuration.ToString() + "s";

            switch (card.Category)
            {
                case CardCategory.Defense:
                    _backgroundImage.color = _defenseColour;
                    break;
                case CardCategory.Offense:
                    _backgroundImage.color = _offenseColour;
                    break;
                case CardCategory.Utility:
                    _backgroundImage.color = _utilityColour;
                    break;
                default:
                    break;
            }
        }

        [Button]
        private void ResetCardUI()
        {
            _numberText.text = "00%";
            _statText.text = "STAT";
            _durationText.text = "00s";
            _backgroundImage.color = new Color32(255, 255, 255, 72);

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
