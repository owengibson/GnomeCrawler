using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class CardSO : SerializedScriptableObject
    {
        public CardType Type;
        public string Name;

        [Multiline]
        public string Description;
        public CardCategory Category;
        [SerializeField] private Stat _statToUpgrade;
        [SerializeField] private float _value;

        public string AbilityClassName;
        public List<StringFloatPair> AbilityValues = new List<StringFloatPair>();

        public bool IsPercentUpgrade;
        public bool IsActivatableCard;
        [ShowIf("IsActivatableCard")]
        public float ActiveDuration;

        [HideInInspector]
        public KeyValuePair<Stat, float> UpgradedStat;

        private void OnValidate()
        {
            UpgradedStat = new KeyValuePair<Stat, float> (_statToUpgrade, _value);
        }
    }

    [System.Serializable]
    public class StringFloatPair
    {
        public string valueName;
        public float value;
    }
}
