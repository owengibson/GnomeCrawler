using GnomeCrawler.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GnomeCrawler
{
    public class VolumeSlider : MonoBehaviour
    {
        public enum VolumeSliderType
        {
            Master,
            Music,
            Ambience,
            SFX
        }

        [SerializeField] private VolumeSliderType type;
        private Slider _slider;

        public void SliderValueChanged()
        {
            float value = _slider.value;

            switch (type)
            {
                case VolumeSliderType.Master:
                    AudioManager.Instance.MasterVolume = value;
                    break;
                case VolumeSliderType.Music:
                    AudioManager.Instance.MusicVolume = value;
                    break;
                case VolumeSliderType.Ambience:
                    AudioManager.Instance.AmbienceVolume = value;
                    break;
                case VolumeSliderType.SFX:
                    AudioManager.Instance.SfxVolume = value;
                    break;
            }
        }

        private void Start()
        {
            _slider = GetComponentInChildren<Slider>();

            switch (type)
            {
                case VolumeSliderType.Master:
                    _slider.value = AudioManager.Instance.MasterVolume;
                    break;
                case VolumeSliderType.Music:
                    _slider.value = AudioManager.Instance.MusicVolume;
                    break;
                case VolumeSliderType.Ambience:
                    _slider.value = AudioManager.Instance.AmbienceVolume;
                    break;
                case VolumeSliderType.SFX:
                    _slider.value = AudioManager.Instance.SfxVolume;
                    break;
            }
        }
    }
}
