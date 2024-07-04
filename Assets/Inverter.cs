using Cinemachine;
using GnomeCrawler.Player;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GnomeCrawler
{
    public class Inverter : MonoBehaviour
    {
        private CinemachineFreeLook _camera;
        private PlayerControls _playerInput;
        private void Awake()
        {
            _camera = GetComponent<CinemachineFreeLook>();

            _playerInput = new PlayerControls();
            _playerInput.Developer.Enable();

            _playerInput.Developer.InvertCam.started += InvertCameraKeyBind;
        }

        private void InvertCameraKeyBind(InputAction.CallbackContext obj)
        {
            _camera.m_YAxis.m_InvertInput = !_camera.m_YAxis.m_InvertInput;
        }

        private void InvertCamera(bool isInverted)
        {
            _camera.m_YAxis.m_InvertInput = isInverted ? false : true;
        }

        private void OnEnable()
        {
            EventManager.OnChooseInversion += InvertCamera;
        }

        private void OnDisable()
        {
            EventManager.OnChooseInversion -= InvertCamera;
        }

        private void OnDestroy()
        {
            _playerInput.Developer.InvertCam.started -= InvertCameraKeyBind;
        }
    }
}
