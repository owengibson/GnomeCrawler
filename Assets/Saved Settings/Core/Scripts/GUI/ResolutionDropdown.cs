using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the scroll rect with the resolution setting.
    /// </summary>
    //[RequireComponent(typeof(ScrollRect))]
    //[RequireComponent(typeof(AutoScrollDropDown))]
    public class ResolutionDropdown : BaseUILoadSetting
    {
        void Start()
        {
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            Resolution[] resolutions = Screen.resolutions;
            Resolution res = Screen.currentResolution;
            int index = 0;
            int atIndex = 0;
            string str;

            for (int i = 0; i < resolutions.Length; ++i)
            {
                str = resolutions[i].ToString();
                atIndex = str.IndexOf('@');
                str = str.Remove(atIndex - 1);
                if (resolutions[i].width == res.width && resolutions[i].height == res.height)
                {
                    index = i;
                }
                options.Add(new TMP_Dropdown.OptionData(str));
            }
            dropdown.options = options;
            dropdown.value = index;


            dropdown.onValueChanged.AddListener(delegate
            {
                Screen.SetResolution(resolutions[dropdown.value].width, resolutions[dropdown.value].height, Screen.fullScreen);
            });
        }

        public override void LoadValue()
        {
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            Resolution[] resolutions = Screen.resolutions;
            Resolution res = Screen.currentResolution;
            int index = -1;
            for (int i = 0; i < resolutions.Length; ++i)
            {
                if (resolutions[i].width == res.width && resolutions[i].height == res.height)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                dropdown.value = index;
            }
        }
    }
}