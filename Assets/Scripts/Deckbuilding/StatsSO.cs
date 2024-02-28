using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Stats")]
    public class StatsSO : SerializedScriptableObject
    {
        public Dictionary<Stat, float> _stats = new Dictionary<Stat, float>();
        [SerializeField] private List<CardSO> _passiveCards = new List<CardSO>();
        private List<CardSO> _activatableCards = new List<CardSO>();

        public float GetStat(Stat stat)
        {
            // Order of operations for stat calculation:
            // Base stat --> + flat stats --> + percent stats (calculated multiplicatively, applied to base + stats)

            float statToReturn = _stats[stat];
            float flatStats = 0;

            float percentStat = 1;

            // Passive cards
            foreach (CardSO card in _passiveCards)
            {
                if (card.UpgradedStat.Key == stat)
                {
                    // Flat stat addition
                    if (!card.IsPercentUpgrade)
                    {
                        flatStats += card.UpgradedStat.Value;
                    }
                    // Percentage stat multiplication
                    else
                    {
                        percentStat *= 1 - (card.UpgradedStat.Value / 100);
                    }
                }
            }
            // Activatable cards
            foreach (CardSO card in _activatableCards)
            {
                if (card.UpgradedStat.Key == stat)
                {
                    // Flat stat addition
                    if (!card.IsPercentUpgrade)
                    {
                        flatStats += card.UpgradedStat.Value;
                    }
                    // Percentage stat multiplication
                    else
                    {
                        percentStat *= 1 - (card.UpgradedStat.Value / 100);
                    }
                }
            }
            statToReturn += flatStats; // Add flat stats to total
            percentStat = 1 - percentStat; // Convert percentage stat
            statToReturn += statToReturn * percentStat; // Apply percentage stat to total

            return (float)Math.Round(statToReturn, 1);
        }

        public void AddCard(CardSO card)
        {
            if (card.IsActivatableCard)
            {
                _activatableCards.Add(card);
                
            }
            else
            {
                _passiveCards.Add(card);
            }
        }

        public void RemoveCard(CardSO card)
        {
            if (card.IsActivatableCard)
            {
                _activatableCards.Remove(card);
            }
            else
            {
                _passiveCards.Remove(card);
            }
        }

        public void ResetCards()
        {
            _activatableCards.Clear();
            _passiveCards.Clear();

            _statsDisplay = "";
        }

        #region STATS_DISPLAY
        [Button("Show/Update Stats Display")]
        private void UpdateStatsDisplay()
        {
            _statsDisplay = "";
            foreach (Stat stat in _stats.Keys)
            {
                _statsDisplay += "<b>" + stat.ToString() + ": </b>" + GetStat(stat) + "\n";
            }
            _statsDisplay = _statsDisplay.Trim();
        }
        [BoxGroup("Stats Display")]
        [HideLabel]
        [DisplayAsString(false, EnableRichText = true)]
        [SerializeField] private string _statsDisplay = "";
        #endregion
    }
}
