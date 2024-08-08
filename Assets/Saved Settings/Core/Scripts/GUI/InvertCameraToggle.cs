using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    [RequireComponent(typeof(Toggle))]
    public class InvertCameraToggle : BaseUILoadSetting
    {
        void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            LoadValue();
            toggle.onValueChanged.AddListener((x) => EventManager.OnChooseInversion?.Invoke(!x));
        }

        public override void LoadValue()
        {
            GetComponent<Toggle>().isOn = (bool)EventManager.IsCameraInverted?.Invoke();
        }
    }
}
