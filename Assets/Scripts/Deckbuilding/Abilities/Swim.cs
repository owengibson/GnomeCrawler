using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    public class Swim : Ability
    {
        private void OnEnable()
        {
            EventManager.OnSwimActivated?.Invoke();
        }

        private void OnDisable()
        {
            EventManager.OnSwimActivated?.Invoke();
        }
    }
}
