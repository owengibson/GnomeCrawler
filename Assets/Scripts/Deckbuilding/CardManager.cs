using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GnomeCrawler.Systems;
using UnityEngine.UI;
using GnomeCrawler.Audio;
using GnomeCrawler.UI;

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

        [SerializeField] private List<CardSO> _hand;

        private PlayerControls _playerControls;


        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Player.HandQuickview.performed += ToggleHandQuickview;
            _playerControls.Player.HandQuickview.canceled += ToggleHandQuickview;
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
                CardGOs[i].SetActive(true);
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
                int randomNum = Random.Range(0, pool.Count);
                if (isDrawingFromDeck)
                {
                    while (output.Contains(pool[randomNum]))
                    {
                        randomNum = Random.Range(0, pool.Count);
                    }
                    output.Add(pool[randomNum]);
                }
                else
                {
                    while (output.Contains(pool[randomNum]) || _deck.Contains(pool[randomNum]))
                    {
                        randomNum = Random.Range(0, pool.Count);
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

            if (Random.Range(0f, 100f) <= EventManager.GetPlayerStats?.Invoke().GetStat(Stat.Luck))
            {
                _deck.Add(DrawLuckCard());
            }
        }

        private CardSO DrawLuckCard()
        {
            int randomNum = Random.Range(0, _allCards.Count);
            while (_deck.Contains(_allCards[randomNum]) || _allCards[randomNum].IsActivatableCard)
            {
                randomNum = Random.Range(0, _allCards.Count);
            }
            return _allCards[randomNum];
        }

        public void ApproveHand()
        {
            EventManager.OnHandApproved?.Invoke(_hand);

            foreach (var card in CardGOs)
            {
                card.SetActive(false);
            }

            EventManager.OnGameStateChanged?.Invoke(GameState.Gameplay);

            _handApproveButton.SetActive(false);
            SetUpQuickview();

            AudioManager.Instance.SetMusicParameter(PlayerStatus.Combat);
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
                _handQuickview.SetActive(true);
            else if (ctx.canceled)
                _handQuickview.SetActive(false);
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
