using GnomeCrawler.Player;
using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace GnomeCrawler
{
    public class StaticBossSpawnLocation : MonoBehaviour
    {
        private PlayerControls _playerInput;
        private void Awake()
        {
            _playerInput = new PlayerControls();
            _playerInput.Developer.Enable();

            _playerInput.Developer.JumpToBoss.started += TPPlayerToBoss;
        }

        private void TPPlayerToBoss(InputAction.CallbackContext obj)
        {
            var player = PlayerStateMachine.instance.gameObject;

            player.GetComponent<CharacterController>().enabled = false;
                
            player.transform.position = transform.position;

            player.GetComponent<CharacterController>().enabled = true;
        }

        private void OnDestroy()
        {
            _playerInput.Developer.JumpToBoss.started -= TPPlayerToBoss;
        }
    }
}
