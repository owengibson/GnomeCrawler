using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GnomeCrawler.Systems;

namespace GnomeCrawler.Deckbuilding
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private CardSO _card;
        [Space]

        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _backgroundImage;

        private readonly Color _defenseColour = new Color32(165, 255, 140, 157);
        private readonly Color _offenseColour = new Color32(255, 140, 140, 157);
        private readonly Color _utilityColour = new Color32(140, 198, 255, 157);

        public void SetCard(CardSO card)
        {
            _card = card;
            Initialise(card);
        }

        public void Initialise(CardSO card)
        {
            if (card == null) return;

            _titleText.text = card.Name;
            _descriptionText.text = card.Description;

            switch (_card.Category)
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

        public void ChooseCard()
        {
            if (_card == null) return;
            EventManager.OnCardChosen?.Invoke(_card);
            EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
            Destroy(gameObject);
        }

        public void CardChosen(CardSO card)
        {
            // This method is for handling when a card is NOT chosen
            if (card == _card) return;

            // Probably put an animation or something in here for when this card is not chosen
            Destroy(gameObject);
        }

        [Button]
        private void ResetCardUI()
        {
            _titleText.text = "Card Title";
            _descriptionText.text = "Card description";
            _backgroundImage.color = new Color32(255, 255, 255, 157);
        }

        private void OnEnable()
        {
            EventManager.OnCardChosen += CardChosen;
        }
        private void OnDisable()
        {
            EventManager.OnCardChosen -= CardChosen;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Initialise(_card);
        }
#endif
    }
}
