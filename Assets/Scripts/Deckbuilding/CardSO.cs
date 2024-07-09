using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class CardSO : SerializedScriptableObject
    {
        public CardType Type;
        public Sprite Icon;
        public string Name;
        [Multiline] public string Description;

        [ShowIfGroup("S", Condition = "@Type == CardType.Stat || Type == CardType.StatAndAbility")]
        [BoxGroup("S/Stat Card Parameters")][SerializeField] private Stat _statToUpgrade;
        [BoxGroup("S/Stat Card Parameters")] public float Value;
        [BoxGroup("S/Stat Card Parameters")] public bool IsPercentUpgrade;

        [ShowIfGroup("A", Condition = "@Type == CardType.Ability || Type == CardType.StatAndAbility")]
        [InfoBox("This is case-sensitive and must exactly match the ability's class name.", InfoMessageType.Warning)]
        [BoxGroup("A/Ability Card Parameters")] public string AbilityClassName;
        [BoxGroup("A/Ability Card Parameters")] public List<StringFloatPair> AbilityValues = new List<StringFloatPair>();

        public bool IsActivatableCard;
        [ShowIf("IsActivatableCard")]
        public float ActiveDuration;


        [HideInInspector]
        public KeyValuePair<Stat, float> UpgradedStat;

        private void OnValidate()
        {
            ValidateStats();
        }

        public void ValidateStats()
        {
            UpgradedStat = new KeyValuePair<Stat, float>(_statToUpgrade, Value);
        }
    }

    [System.Serializable]
    public class StringFloatPair
    {
        public string valueName;
        public float value;
    }
}
