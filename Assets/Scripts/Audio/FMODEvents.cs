using FMODUnity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class FMODEvents : Singleton<FMODEvents>
    {
        [SerializeField] private EventReferencesSO _references;

        public EventReference GetEventReference(string name)
        {
            return _references.Events.Find(x => x.Name == name).EventReference;
        }
    }
}
