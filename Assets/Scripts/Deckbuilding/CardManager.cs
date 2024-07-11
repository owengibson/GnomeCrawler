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
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;

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
        [SerializeField] private float _handToScreenAnimDuration;
        [SerializeField] private float _quickviewAnimDuration;

        [SerializeField] private List<CardSO> _hand;

        private PlayerControls _playerControls;

        // Animation default values
        private Vector3[] _animCardPos;
        private Vector3[] _animCardScales;
        private Vector3[] _animCardRots;


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
            _handApproveButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_handApproveButton);
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
            AnimateBetweenHandAndScreen(true, _handToScreenAnimDuration, CardGOs, null);

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

            AnimateBetweenHandAndScreen(false, _handToScreenAnimDuration, CardGOs, () =>
            {
                EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);
                AudioManager.Instance.SetMusicParameter(PlayerStatus.Combat);
            });

            _handApproveButton.SetActive(false);
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
        }

        private void ToggleHandQuickview(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                //_handQuickview.SetActive(true);
                AnimateBetweenHandAndScreen(true, _quickviewAnimDuration, _quickviewCards, null);
            else if (ctx.canceled)
                AnimateBetweenHandAndScreen(false, _quickviewAnimDuration, _quickviewCards, null);
                //_handQuickview.SetActive(false);
        }

        private void AnimateBetweenHandAndScreen(bool isOpening, float duration, GameObject[] cards, Action callback)
        {
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
                }
                else
                {
                    endPos = _animCardPos[i];
                    endScale = _animCardScales[i];
                    endRot = _animCardRots[i];
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
