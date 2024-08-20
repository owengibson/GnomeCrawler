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
        [SerializeField] private bool _isRealRoom = false;
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
            if (!_isRealRoom) return;
            _boss.SetActive(true);
        }
    }
}
