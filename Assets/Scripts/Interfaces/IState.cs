using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}
