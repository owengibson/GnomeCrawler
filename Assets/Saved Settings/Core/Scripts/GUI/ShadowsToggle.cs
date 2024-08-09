using SavedSettings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    [RequireComponent(typeof(Toggle))]
    public class ShadowsToggle : BaseUILoadSetting
    {
        private UniversalRenderPipelineAsset urp;

        void Start()
        {
            urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

            Toggle toggle = GetComponent<Toggle>();
            LoadValue();
            toggle.onValueChanged.AddListener((x) => urp.shadowDistance = x ? 50 : 0);
        }

        public override void LoadValue()
        {
            bool isOn;
            if (urp.shadowDistance > 0) isOn = true;
            else isOn = false;
            GetComponent<Toggle>().isOn = isOn;
        }

        private void OnDestroy()
        {
            urp.shadowDistance = 50;
        }
    }
}
