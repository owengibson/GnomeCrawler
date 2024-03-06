using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public interface IKillable
    {
        bool IsKilled { get; set; }
        void Die();
    }
}
