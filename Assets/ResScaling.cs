using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GnomeCrawler
{
    public class ResScaling : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 1)] private float resValue =1;
        private UniversalRenderPipelineAsset urpAsset;
        void Start()
        {
            urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
           
        }
        private void OnValidate()
        {
            urpAsset.renderScale = resValue;
        }
    }
}
