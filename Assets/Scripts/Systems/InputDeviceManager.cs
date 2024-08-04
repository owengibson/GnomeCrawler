using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GnomeCrawler
{
    public class InputDeviceManager : Singleton<InputDeviceManager>
    {
        public bool isKeyboardAndMouse;
        public void OnEnable()
        {
            InputSystem.onActionChange += InputActionChangeCallback;
            if (InputSystem.devices.Count == 1)
            {
                InputDevice inputDevice = InputSystem.devices[0];
                isKeyboardAndMouse = inputDevice.name.Equals("Keyboard") || inputDevice.name.Equals("Mouse");
            }
        }

        private void OnDisable()
        {
            InputSystem.onActionChange -= InputActionChangeCallback;
        }

        private void InputActionChangeCallback(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                if (!typeof(InputAction).IsAssignableFrom(obj.GetType())) return;
                InputAction receivedInputAction = (InputAction)obj;
                if (receivedInputAction.activeControl == null) return;
                InputDevice lastDevice = receivedInputAction.activeControl.device;

                isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
                SwapControlSchemeSettings(isKeyboardAndMouse);
            }
        }

        private void SwapControlSchemeSettings(bool isKeyboard)
        {
            if (!isKeyboard)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
    