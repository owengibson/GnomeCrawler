using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the toggle with the soft particles setting.
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ShadowResolutionDropdown : BaseUILoadSetting
    {
        void Start()
        {
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            options.Add(new TMP_Dropdown.OptionData("Low"));
            options.Add(new TMP_Dropdown.OptionData("Medium"));
            options.Add(new TMP_Dropdown.OptionData("High"));
            options.Add(new TMP_Dropdown.OptionData("Very High"));
            dropdown.options = options;
            LoadValue();
            dropdown.onValueChanged.AddListener(delegate { QualitySettings.shadowResolution = (ShadowResolution)dropdown.value; });
        }

        public override void LoadValue()
        {
            GetComponent<TMP_Dropdown>().value = (int)QualitySettings.shadowResolution;
        }
    }
}
