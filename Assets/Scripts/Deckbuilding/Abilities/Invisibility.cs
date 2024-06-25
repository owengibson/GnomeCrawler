using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    public class Invisibility : Ability
    {
        [SerializeField] private GameObject _invisText;

        private bool _isPlayerTargetable = true;

        private void OnEnable()
        {
            EventManager.IsPlayerTargetable += IsPlayerTargetable;
            EventManager.OnPlayerHit += EndInvisibility;

            _isPlayerTargetable = false;
            _invisText?.SetActive(true);
        }

        private void EndInvisibility(float unused)
        {
            if (_isPlayerTargetable) return;

            _isPlayerTargetable = true;
            _invisText?.SetActive(false);

            enabled = false;
        }

        private bool IsPlayerTargetable() => _isPlayerTargetable;

        private void OnDisable()
        {
            _isPlayerTargetable = true;

            EventManager.IsPlayerTargetable -= IsPlayerTargetable;
            EventManager.OnPlayerHit -= EndInvisibility;
        }
    }
}
