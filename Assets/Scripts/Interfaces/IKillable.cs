using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public interface IKillable
    {
        bool IsDead { get; set; }
        void Die();
    }
}
