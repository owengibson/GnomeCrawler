using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class BossRoom : MonoBehaviour
    {
        [SerializeField] private GameObject _boss;
        private void OnEnable()
        {
            EventManager.OnEnteredBossRoom += SpawnBoss;
        }

        private void OnDisable()
        {
            EventManager.OnEnteredBossRoom -= SpawnBoss;
        }

        private void SpawnBoss()
        {
            _boss.SetActive(true);
        }
    }
}
