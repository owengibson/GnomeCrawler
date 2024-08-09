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
        [SerializeField] private GameObject[] _choiceAnimationCards;
        [SerializeField] private GameObject _deckIcon;
        [SerializeField] private GameObject _animCardPrefab;

        [Space]
        [SerializeField] private float _cardChoiceAnimDuration;
        [SerializeField] private float _handDrawAnimDuration;
        [SerializeField] private float _handToScreenAnimDuration;
        [SerializeField] private float _quickviewAnimDuration;

        [SerializeField] private List<CardSO> _hand;

        private PlayerControls _playerControls;

        // Animation default values
        private Vector3[] _animCardPos;
        private Vector3[] _animCardScales;
        private Vector3[] _animCardRots;

        private Vector3[] _initialCardPos;

        private bool _hasFirstHandBeenDrawn = false;

        private CardAnimationStatus _animationStatus = CardAnimationStatus.Closed;

        public GameObject handQuickView => _handQuickview;


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

            _initialCardPos = new Vector3[3];
            for (int i = 0; i < CardGOs.Length; i++)
            {
                _initialCardPos[i] = CardGOs[i].transform.localPosition;
            }
        }

        /*private void Update()
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
        }*/

        private void ResetCardPositions()
        {
            for (int i = 0; i < CardGOs.Length; i++)
            {
                CardGOs[i].transform.localPosition = _initialCardPos[i];
            }
        }

        private void DrawAndDisplayNewHand(int unused)
        {
            _hand = DrawCards(_deck, _handSize, true);
            EventManager.OnGameStateChanged?.Invoke(GameState.Paused);
            InstantiateCards(_hand, false);
        }

        public void SetCurrentHand(List<CardSO> hand)
        {
            _hand.Clear();
            _hand = hand;
        }

        public void InstantiateCards(List<CardSO> cards, bool isSelection)
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

            if (!isSelection)
            {
                AnimateDrawHand(_handDrawAnimDuration, () =>
                {
                    _handApproveButton.transform.localScale = Vector3.zero;
                    _handApproveButton.SetActive(true);
                    _handApproveButton.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("Pop"));
                    EventSystem.current.SetSelectedGameObject(_handApproveButton);
                });
            }

            else
            {
                // Draw new card to add to deck
                _animationStatus = CardAnimationStatus.Choice;
                AnimateCardChoicePresentation(_cardChoiceAnimDuration, () => StartCoroutine(SetSelectedGameObject()));
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

            _handApproveButton.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(()=>
            {
                _handApproveButton.SetActive(false);
                _animationStatus = CardAnimationStatus.Closed;
            });
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
                AnimateCardsBetweenScreenAndHand(true, _quickviewAnimDuration, _quickviewCards, null);
                _animationStatus = CardAnimationStatus.Quickview;
            }
            else if (ctx.canceled)
                AnimateCardsBetweenScreenAndHand(false, _quickviewAnimDuration, _quickviewCards, () => _animationStatus = CardAnimationStatus.Closed);
                //_handQuickview.SetActive(false);
        }

        private void AnimateCardsBetweenScreenAndHand(bool isOpening, float duration, GameObject[] cards, Action callback)
        {
            DOTween.KillAll();
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
                    cardFlip.InsertCallback((duration * 0.5f) * i, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("FlipCard")));
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
            if (_animationStatus != CardAnimationStatus.Closed)
            {
                ForceCloseCardsOnScreen();
            }
            _animationStatus = CardAnimationStatus.HandReview;

            ResetCardPositions();
            Sequence animation = DOTween.Sequence();

            // Deck icon scale
            _deckIcon.transform.localScale = Vector3.zero;
            _deckIcon.SetActive(true);
            animation.Append(_deckIcon.transform.DOScale(1, duration * 0.25f).SetEase(Ease.OutBack));
            animation.InsertCallback(0, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("Appear")));

            // Card drawing (instantiate, scale, move, flip)
            Sequence cardDraw = DOTween.Sequence();
            for (int i = 0; i < _handSize; i++)
            {
                GameObject card = Instantiate(_animCardPrefab, _deckIcon.transform.position, Quaternion.identity, transform);
                card.transform.localScale = Vector3.zero;
                cardDraw.InsertCallback(duration * 0.2f * i, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("DealCard")));
                // Scale
                cardDraw.Insert(duration * 0.2f * i, card.transform.DOScale(Vector3.one * 7, duration));
                // Move
                cardDraw.Insert(duration * 0.2f * i, card.transform.DOMove(CardGOs[i].transform.position, duration));

                // Flip
                Sequence cardFlip = DOTween.Sequence();
                CardGOs[i].transform.eulerAngles = new Vector3(0, 90, 0);
                CardGOs[i].transform.localScale = Vector3.one;
                CardGOs[i].SetActive(true);
                // Rotate anim card
                cardFlip.Insert(duration * 0.005f * i, card.transform.DORotate(new Vector3(0, 90, 0), duration * 0.15f));
                cardFlip.InsertCallback(duration * 0.005f * i, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("FlipCard")));
                // Rotate actual card
                cardFlip.Append(CardGOs[i].transform.DORotate(Vector3.zero, duration * 0.15f));
                cardFlip.AppendCallback(() => Destroy(card));
                cardDraw.Append(cardFlip);
            }
            // Deck icon hide
            cardDraw.Insert(duration * 0.25f * _handSize, _deckIcon.transform.DOScale(0, duration * 0.25f).SetEase(Ease.InBack));
            cardDraw.InsertCallback(duration * 0.25f * _handSize, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("Dissapear")));

            animation.Append(cardDraw);
            animation.SetUpdate(true);
            animation.Play().OnComplete(() => callback?.Invoke());
            
        }

        private void AnimateCardChoicePresentation(float duration, Action callback)
        {
            if (_animationStatus != CardAnimationStatus.Closed)
            {
                ForceCloseCardsOnScreen();
            }
            _animationStatus = CardAnimationStatus.Choice;

            Sequence animation = DOTween.Sequence();
            for (int i = 0; i < _numberOfCardsToDraw; i++)
            {
                // Prime UI elements
                _choiceAnimationCards[i].transform.localScale = Vector3.zero;
                _choiceAnimationCards[i].SetActive(true);
                CardGOs[i].transform.eulerAngles = new Vector3(0, 90, 0);
                CardGOs[i].SetActive(true);

                // Scale anim cards
                animation.Insert(duration * 0.15f * i, _choiceAnimationCards[i].transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));

                // Flip
                animation.Insert(duration * 0.15f * i + duration, _choiceAnimationCards[i].transform.DORotate(new Vector3(0, 90, 0), duration * 0.25f));
                animation.Insert(duration * 0.15f * i + duration + duration * 0.25f, CardGOs[i].transform.DORotate(Vector3.zero, duration * 0.25f));
                animation.InsertCallback(duration * 0.15f * i + duration, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("FlipCard")));

                animation.AppendCallback(() =>
                {
                    _choiceAnimationCards[i].SetActive(false);
                    _choiceAnimationCards[i].transform.eulerAngles = Vector3.zero;
                });
            }
            
            animation.SetUpdate(true);
            animation.Play().OnComplete(() => callback?.Invoke());
        }

        private void AnimateCardChosen(GameObject card, float duration)
        {
            Sequence animation = DOTween.Sequence();

            Vector3 sideOn = new Vector3(0, 90, 0);
            Vector3 cardStartPos = card.transform.position;

            GameObject animCard = Instantiate(_animCardPrefab, card.transform.position, Quaternion.identity, transform);
            animCard.transform.eulerAngles = sideOn;
            animCard.transform.localScale = card.transform.localScale;
            animCard.GetComponent<RectTransform>().sizeDelta = card.GetComponent<RectTransform>().sizeDelta;
            // Scale
            animation.Append(card.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InOutQuad));
            animation.Insert(0, animCard.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InOutQuad));

            // Move
            animation.Insert(0, card.transform.DOMove(_deckIcon.transform.position, duration));
            animation.Insert(0, animCard.transform.DOMove(_deckIcon.transform.position, duration));

            // Deck icon appear
            _deckIcon.transform.localScale = Vector3.zero;
            _deckIcon.SetActive(true);
            animation.Insert(0, _deckIcon.transform.DOScale(1, duration * 0.25f).SetEase(Ease.OutBack));
            animation.InsertCallback(0, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("Appear")));

            // Flips
            Sequence flips = DOTween.Sequence();
            float flipDuration = duration * 0.15f;
/*            for (int i = 0; i < 1; i++)
            {
                flips.Append(card.transform.DORotate(sideOn, flipDuration));
                flips.Append(animCard.transform.DORotate(Vector3.zero, flipDuration));
                flips.Append(animCard.transform.DORotate(sideOn, flipDuration));
                flips.Append(transform.DORotate(Vector3.zero, flipDuration));
            }*/
            flips.Append(card.transform.DORotate(sideOn, flipDuration));
            flips.Append(animCard.transform.DORotate(Vector3.zero, flipDuration));
            flips.SetEase(Ease.InOutQuad);

            animation.Insert(0, flips);

            // Deck icon disappear
            animation.Append(_deckIcon.transform.DOScale(0, duration * 0.25f).SetEase(Ease.InBack));

            animation.InsertCallback(animation.Duration() - duration * 0.25f, () => AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetEventReference("Dissapear")));

            animation.SetUpdate(true);
            animation.Play().OnComplete(() =>
            {
                Destroy(animCard);
                EventManager.OnCardAnimationStatusChange?.Invoke(CardAnimationStatus.Closed);
                card.transform.position = cardStartPos;
                card.gameObject.SetActive(false);
            });
        }

        private void ForceCloseCardsOnScreen()
        {
            switch (_animationStatus)
            {
                case CardAnimationStatus.Closed:
                    break;

                case CardAnimationStatus.Quickview:
                    AnimateCardsBetweenScreenAndHand(false, _quickviewAnimDuration, _quickviewCards, ()=>_animationStatus = CardAnimationStatus.Closed);
                    break;

                case CardAnimationStatus.HandReview:
                    ApproveHand();
                    break;

                case CardAnimationStatus.Choice:
                    break;

                default:
                    break;
            }
        }

        private void SetCardAnimationStatus(CardAnimationStatus status) => _animationStatus = status;

        private void OnEnable()
        {
            EventManager.OnRoomStarted += DrawAndDisplayNewHand;
            EventManager.OnRoomCleared += DrawAndDisplayCards;
            EventManager.OnCardChosen += AddCardToDeck;
            EventManager.OnCardAnimationStatusChange += SetCardAnimationStatus;
            EventManager.OnCardChosenAnimation += AnimateCardChosen;

            _playerControls.Enable();
        }
        private void OnDisable()
        {
            EventManager.OnRoomStarted -= DrawAndDisplayNewHand;
            EventManager.OnRoomCleared -= DrawAndDisplayCards;
            EventManager.OnCardChosen -= AddCardToDeck;
            EventManager.OnCardAnimationStatusChange += SetCardAnimationStatus;
            EventManager.OnCardChosenAnimation -= AnimateCardChosen;

            _playerControls.Disable();
        }
    }
}
