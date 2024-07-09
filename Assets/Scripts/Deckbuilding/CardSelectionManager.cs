using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GnomeCrawler.Deckbuilding
{
    public class CardSelectionManager : Singleton<CardSelectionManager>
    {
        public GameObject[] Cards { get; private set; }
        public GameObject LastSelected {  get; set; }
        public int LastSelectedIndex { get; set; }

        private void Awake()
        {
            Cards = GetComponent<CardManager>().CardGOs;
        }

        private void Update()
        {
            // If we move right
            if (InputManager.Instance.NavigationInput.x > 0)
            {
                // Select next card
                HandleNextCardSelection(1);
            }

            // If we move left
            if (InputManager.Instance.NavigationInput.x < 0)
            {
                // Select previous card
                HandleNextCardSelection(-1);
            }
        }

        private void HandleNextCardSelection(int addition)
        {
            if (EventSystem.current.currentSelectedGameObject == null && LastSelected != null)
            {
                int newIndex = LastSelectedIndex + addition;
                newIndex = Mathf.Clamp(newIndex, 0, Cards.Length - 1);
                EventSystem.current.SetSelectedGameObject(Cards[newIndex]);
            }
        }
    }
}
