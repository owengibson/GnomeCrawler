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
        public List<NamedEventReference> Events;
    }

    [Serializable]
    public class NamedEventReference
    {
        public string Name;
        public EventReference EventReference;
    }
}
