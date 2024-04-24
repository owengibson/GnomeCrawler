using FMODUnity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    [CreateAssetMenu(fileName = "New Event References List", menuName = "Event References")]
    public class EventReferencesSO : SerializedScriptableObject
    {
        [ShowInInspector] public List<NamedEventReference> Events { get; set; }
    }

    [Serializable]
    public class NamedEventReference
    {
        [ShowInInspector]
        public string Name { get; set; }
        [ShowInInspector]
        public EventReference EventReference { get; set; }
    }
}
