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
        private void InputActionChangeCallback(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                if (!typeof(InputAction).IsAssignableFrom(obj.GetType())) return;
                InputAction receivedInputAction = (InputAction)obj;
                if (receivedInputAction.activeControl == null) return;
                InputDevice lastDevice = receivedInputAction.activeControl.device;

                isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
                Debug.Log(isKeyboardAndMouse);
                //If needed we could check for "XInputControllerWindows" or "DualShock4GamepadHID"
                //Maybe if it Contains "controller" could be xbox layout and "gamepad" sony? More investigation needed
            }
        }
    }
}
