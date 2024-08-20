using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using GnomeCrawler.Audio;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GnomeCrawler.Deckbuilding
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private CardSO _card;
        [Space]

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Sprite _nonActivatableBackground;
        [SerializeField] private Sprite _activatableBackground;
        [Space]

        [Header("Animation")]
        [SerializeField] private GameObject _animCardPrefab;
        [SerializeField] private float _notChosenAnimDuration;
        [SerializeField] private float _chosenAnimDuration;

        public GameObject ButtonGraphic;

        public void SetCard(CardSO card)
        {
            _card = card;
            Initialise(card);
        }

        public void Initialise(CardSO card)
        {
            if (card == null) return;

            if (_icon != null)
                _icon.sprite = card.Icon;

            _titleText.text = card.Name;
            _descriptionText.text = card.Description;

            _backgroundImage.sprite = card.IsActivatableCard ? _activatableBackground : _nonActivatableBackground;
        }

        public void ChooseCard()
        {
            if (_card == null) return;
            EventManager.OnCardChosen?.Invoke(_card);
            EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);

            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("CardChosen"));

            // Animation
            EventManager.OnCardChosenAnimation?.Invoke(gameObject, _chosenAnimDuration);
        }

        public void CardChosen(CardSO card)
        {
            // This method is for handling when a card is NOT chosen
            if (card == _card) return;

            // Probably put an animation or something in here for when this card is not chosen
            transform.DOScale(Vector3.zero, _notChosenAnimDuration).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
        }

/*        private void AnimateChoosingCard(float duration)
        {
            Sequence animation = DOTween.Sequence();

            Vector3 sideOn = new Vector3(0, 90, 0);

            GameObject animCard = Instantiate(_animCardPrefab, transform.position, Quaternion.identity, transform.parent);
            animCard.transform.eulerAngles = sideOn;
            animCard.transform.localScale = transform.localScale;
            animCard.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
            // Scale
            animation.Append(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            animation.Insert(0, animCard.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

            // Flips
            Sequence flips = DOTween.Sequence();
            float flipDuration = duration * 0.15f;
            for (int i = 0; i < 1; i++)
            {
                flips.Append(transform.DORotate(sideOn, flipDuration));
                flips.Append(animCard.transform.DORotate(Vector3.zero, flipDuration));
                flips.Append(animCard.transform.DORotate(sideOn, flipDuration));
                flips.Append(transform.DORotate(Vector3.zero, flipDuration));
            }
            flips.Append(transform.DORotate(sideOn, flipDuration));
            flips.Append(animCard.transform.DORotate(Vector3.zero, flipDuration));
            flips.SetEase(Ease.InOutQuad);

            animation.Insert(0, flips);
            animation.SetUpdate(true);
            animation.Play().OnComplete(() =>
            {
                Destroy(animCard);
                EventManager.OnCardAnimationStatusChange?.Invoke(CardAnimationStatus.Closed);
                gameObject.SetActive(false);
            });

        }*/

        [Button]
        private void ResetCardUI()
        {
            _titleText.text = "Card Title";
            _descriptionText.text = "Card description";
            _icon.sprite = null;
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
