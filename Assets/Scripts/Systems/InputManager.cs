using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GnomeCrawler.Systems
{
    public class InputManager : Singleton<InputManager>
    {
        public static PlayerInput PlayerInput { get; set; }
        public Vector2 NavigationInput { get; set; }

        private InputAction _navigationAction;

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            _navigationAction = PlayerInput.actions["Navigate"];
        }
        private void Update()
        {
            NavigationInput = _navigationAction.ReadValue<Vector2>();
        }
    }
}
