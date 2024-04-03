using Cinemachine;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Player
{
    public class PlayerCameraPauseToggle : MonoBehaviour
    {
        private CinemachineInputProvider _inputProvider;

        private void Start()
        {
            _inputProvider = GetComponent<CinemachineInputProvider>();
        }

        private void TogglePlayerCameraInput(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Paused:
                    _inputProvider.enabled = false;
                    Debug.Log("disabling input provider");
                    break;
                case GameState.Gameplay:
                    _inputProvider.enabled = true;
                    Debug.Log("enabling input provider");
                    break;
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            EventManager.OnGameStateChanged += TogglePlayerCameraInput;
        }
        private void OnDisable()
        {
            EventManager.OnGameStateChanged -= TogglePlayerCameraInput;
        }
    }
}
