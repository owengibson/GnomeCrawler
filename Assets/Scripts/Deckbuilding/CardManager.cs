using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GnomeCrawler.Systems;
using UnityEngine.UI;
using GnomeCrawler.Audio;
using GnomeCrawler.UI;
using DG.Tweening;
using System;

namespace GnomeCrawler.Deckbuilding
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private List<CardSO> _allCards;
        [Space]

        [SerializeField] private List<CardSO> _deck;
        [Space]

        [Header("GameObject/Prefab References")]
        public GameObject[] CardGOs;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private GameObject _handApproveButton;
        [SerializeField] private GameObject _handQuickview;
        [SerializeField] private GameObject[] _quickviewCards;
        [Space]

        [Header("Parameters")]
        [SerializeField] private int _handSize = 3;
        [SerializeField] private int _numberOfCardsToDraw = 3;
        [Space]

        [Header("Animation")]
        [SerializeField] private GameObject[] _animationCards;
        [SerializeField] private GameObject _deckIcon;
        [SerializeField] private GameObject _animCardPrefab;

        [Space]
        [SerializeField] private float _handDrawAnimDuration;
        [SerializeField] private float _handToScreenAnimDuration;
        [SerializeField] private float _quickviewAnimDuration;

        [SerializeField] private List<CardSO> _hand;

        private PlayerControls _playerControls;

        // Animation default values
        private Vector3[] _animCardPos;
        private Vector3[] _animCardScales;
        private Vector3[] _animCardRots;

        private bool _hasFirstHandBeenDrawn = false;

        private enum CardAnimationStatus { Closed, Quickview, HandReview, Choice }
        private CardAnimationStatus _animationStatus = CardAnimationStatus.Closed;


        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.HandQuickview.performed += ToggleHandQuickview;
            _playerControls.Player.HandQuickview.canceled += ToggleHandQuickview;

            _animCardPos = new Vector3[3];
            _animCardScales = new Vector3[3];
            _animCardRots = new Vector3[3];
            for (int i = 0; i < _animationCards.Length; i++)
            {
                _animCardPos[i] = _animationCards[i].transform.localPosition;
                _animCardScales[i] = _animationCards[i].transform.localScale;
                _animCardRots[i] = _animationCards[i].transform.eulerAngles;
            }
        }

        /*private void Update()
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
        }*/

        private void DrawAndDisplayNewHand(int unused)
        {
            _hand = DrawCards(_deck, _handSize, true);
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(_hand, false);
        }

        private void InstantiateCards(List<CardSO> cards, bool isSelection)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                CardSO card = cards[i];
                CardUI cardUI = CardGOs[i].GetComponent<CardUI>();
                cardUI.SetCard(card);
                if (!isSelection)
                {
                    CardGOs[i].GetComponent<CardSelectionHandler>().enabled = false;
                    CardGOs[i].GetComponent<Button>().enabled = false;
                    cardUI.ButtonGraphic.SetActive(false);
                }
                else
                {
                    CardGOs[i].GetComponent<CardSelectionHandler>().enabled = true;
                    CardGOs[i].GetComponent<Button>().enabled = true;
                    cardUI.ButtonGraphic.SetActive(true);
                }
            }
            //AnimateQuickview(true, _handToScreenAnimDuration, CardGOs, null);
            if (!isSelection)
            {
                AnimateDrawHand(_handDrawAnimDuration, () =>
                {
                    _handApproveButton.transform.localScale = Vector3.zero;
                    _handApproveButton.SetActive(true);
                    _handApproveButton.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
                    EventSystem.current.SetSelectedGameObject(_handApproveButton);
                });
            }


            if (isSelection)
            {
                StartCoroutine(SetSelectedGameObject());
            }
        }

        private IEnumerator SetSelectedGameObject()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(CardGOs[0]);
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

            if (UnityEngine.Random.Range(0f, 100f) <= EventManager.GetPlayerStats?.Invoke().GetStat(Stat.Luck))
            {
                _deck.Add(DrawLuckCard());
            }
        }

        private CardSO DrawLuckCard()
        {
            int randomNum = UnityEngine.Random.Range(0, _allCards.Count);
            while (_deck.Contains(_allCards[randomNum]) || _allCards[randomNum].IsActivatableCard)
            {
                randomNum = UnityEngine.Random.Range(0, _allCards.Count);
            }
            return _allCards[randomNum];
        }

        public void ApproveHand()
        {
            EventManager.OnHandApproved?.Invoke(_hand);

            AnimateCardsBetweenScreenAndHand(false, _handToScreenAnimDuration, CardGOs, () =>
            {
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
                AudioManager.Instance.SetMusicParameter(PlayerStatus.Combat);
            });

            _handApproveButton.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(()=> _handApproveButton.SetActive(false));
            SetUpQuickview();
        }

        private void SetUpQuickview()
        {
            for (int i = 0; i < _hand.Count; i++)
            {
                CardSO card = _hand[i];
                CardUI cardUI = _quickviewCards[i].GetComponent<CardUI>();
                cardUI.SetCard(card);
                _quickviewCards[i].GetComponent<CardSelectionHandler>().enabled = false;
                _quickviewCards[i].GetComponent<Button>().enabled = false;
                cardUI.ButtonGraphic.SetActive(false);
            }
            _hasFirstHandBeenDrawn = true;
        }

        private void ToggleHandQuickview(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_hasFirstHandBeenDrawn) return;
            if (ctx.performed)
            {
                //_handQuickview.SetActive(true);
                _animationStatus = CardAnimationStatus.Quickview;
                AnimateCardsBetweenScreenAndHand(true, _quickviewAnimDuration, _quickviewCards, null);
            }
            else if (ctx.canceled)
                AnimateCardsBetweenScreenAndHand(false, _quickviewAnimDuration, _quickviewCards, () => _animationStatus = CardAnimationStatus.Closed);
                //_handQuickview.SetActive(false);
        }

        private void AnimateCardsBetweenScreenAndHand(bool isOpening, float duration, GameObject[] cards, Action callback)
        {
            if (isOpening && _animationStatus != CardAnimationStatus.Closed) return;
            if (!isOpening && _animationStatus == CardAnimationStatus.Closed) return;
            if (!_animationCards[0].activeSelf)
            {
                foreach (var card in _animationCards)
                {
                    card.SetActive(true);
                }
            }

            Sequence animation = DOTween.Sequence();

            // Card flip close
            if (!isOpening)
            {
                Sequence cardFlip = DOTween.Sequence();
                for (int i = 0; i < _animationCards.Length; i++)
                {
                    // Rotate actual card
                    cardFlip.Insert((duration * 0.5f) * i, CardGOs[i].transform.DORotate(new Vector3(0, 90, 0), duration * 0.25f));
                    // Rotate anim card
                    cardFlip.Append(_animationCards[i].transform.DORotate(Vector3.zero, duration * 0.25f));
                    
                   cardFlip.AppendCallback(() => CardGOs[i].SetActive(false));
                }
                animation.Append(cardFlip);
            }

            // Anim card move, scale, rotate 
            Sequence cardMoveScaleRot = DOTween.Sequence();
            for (int i = 0; i < _animationCards.Length; i++)
            {
                Vector3 endPos, endScale, endRot;
                if (isOpening)
                {
                    endPos = cards[i].transform.position;
                    endScale = Vector3.one * 7;
                    endRot = Vector3.zero;

                    _animationCards[i].transform.localPosition = _animCardPos[i];
                    _animationCards[i].transform.localScale = _animCardScales[i];
                    _animationCards[i].transform.eulerAngles = _animCardRots[i];
                }
                else
                {
                    endPos = _animCardPos[i];
                    endScale = _animCardScales[i];
                    endRot = _animCardRots[i];

                    _animationCards[i].transform.localPosition = cards[i].transform.localPosition;
                    _animationCards[i].transform.localScale = Vector3.one * 7;
                    _animationCards[i].transform.eulerAngles = new Vector3(0, 90, 0);
                }

                // Move
                if (isOpening)
                {
                    cardMoveScaleRot.Insert(0, _animationCards[i].transform.DOMove(endPos, duration));
                }
                else
                {
                    cardMoveScaleRot.Insert(0, _animationCards[i].transform.DOLocalMove(endPos, duration));
                }
                //Scale
                cardMoveScaleRot.Insert(0, _animationCards[i].transform.DOScale(endScale, duration));
                // Rotation
                cardMoveScaleRot.Insert(0, _animationCards[i].transform.DORotate(endRot, duration));
                // Time offset
                /*if (i != 2)
                    cardMoveScaleRot.AppendInterval(0.5f);*/
            }
            animation.Append(cardMoveScaleRot);

            // Card flip open
            if (isOpening)
            {
                Sequence cardFlip = DOTween.Sequence();
                for (int i = 0; i < _animationCards.Length; i++)
                {
                    CardGOs[i].transform.eulerAngles = new Vector3(0, 90, 0);
                    CardGOs[i].SetActive(true);
                    // Rotate anim card
                    cardFlip.Insert((duration * 0.5f) * i, _animationCards[i].transform.DORotate(new Vector3(0, 90, 0), duration * 0.25f));
                    // Rotate actual card
                    cardFlip.Append(CardGOs[i].transform.DORotate(Vector3.zero, duration * 0.25f));
                }
                animation.Append(cardFlip);
            }
            animation.SetUpdate(true);
            animation.Play().OnComplete(() => callback?.Invoke());
        }

        private void AnimateDrawHand(float duration, Action callback)
        {
            _animationStatus = CardAnimationStatus.HandReview;

            Sequence animation = DOTween.Sequence();

            // Deck icon scale
            _deckIcon.transform.localScale = Vector3.zero;
            _deckIcon.SetActive(true);
            animation.Append(_deckIcon.transform.DOScale(1, duration * 0.25f).SetEase(Ease.OutBack));

            // Card drawing (instantiate, scale, move, flip)
            Sequence cardDraw = DOTween.Sequence();
            for (int i = 0; i < _handSize; i++)
            {
                GameObject card = Instantiate(_animCardPrefab, _deckIcon.transform.position, Quaternion.identity, transform);
                card.transform.localScale = Vector3.zero;
                // Scale
                cardDraw.Insert(duration * 0.2f * i, card.transform.DOScale(Vector3.one * 7, duration));
                // Move
                cardDraw.Insert(duration * 0.2f * i, card.transform.DOMove(CardGOs[i].transform.position, duration));

                // Flip
                Sequence cardFlip = DOTween.Sequence();
                CardGOs[i].transform.eulerAngles = new Vector3(0, 90, 0);
                CardGOs[i].SetActive(true);
                // Rotate anim card
                cardFlip.Insert(duration * 0.005f * i, card.transform.DORotate(new Vector3(0, 90, 0), duration * 0.15f));
                // Rotate actual card
                cardFlip.Append(CardGOs[i].transform.DORotate(Vector3.zero, duration * 0.15f));
                cardFlip.AppendCallback(() => Destroy(card));
                cardDraw.Append(cardFlip);
            }
            // Deck icon hide
            cardDraw.Insert(duration * 0.25f * _handSize, _deckIcon.transform.DOScale(0, duration * 0.25f).SetEase(Ease.InBack));

            animation.Append(cardDraw);
            animation.SetUpdate(true);
            animation.Play().OnComplete(() => callback?.Invoke());
            
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
