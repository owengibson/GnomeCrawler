using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GnomeCrawler
{
    /// <summary>
    /// Inspired by the Llama Academy WorldSpace HealthBar tutorial
    /// https://www.youtube.com/watch?v=cUQVLgohAjY&t=1s 
    /// </summary>
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _progressImage;
        [SerializeField] private float _defaultSpeed = 20f;
        [SerializeField] private UnityEvent<float> _onProgress;
        [SerializeField] private UnityEvent _onCompleted;

        private Coroutine _animationCoroutine;


        private void Start()
        {
            if(_progressImage.type != Image.Type.Filled)
            {
                Debug.LogError($"{name}'s ProgressImage is not of type filled. Disabling Progress Bar");
                this.enabled = false;
            }
        }

        public void SetProgress(float progress)
        {
            SetProgress(progress, _defaultSpeed);
        }

        public void SetProgress(float progress, float speed)
        {
            if (progress is <0 || progress > 1)
            {
                Debug.LogWarning($"Invaled number passed. Expected value between 0 & 1. Got {progress} Claimping ");
                progress = Mathf.Clamp01(progress);
            }

            if(progress != _progressImage.fillAmount)
            {
                if(_animationCoroutine!= null)
                {
                    StopCoroutine(_animationCoroutine);
                }

                _animationCoroutine = StartCoroutine(AnimateProgress(progress, speed));
            }
        }

        private IEnumerator AnimateProgress(float progress, float speed)
        {
            float time = 0;
            float initialProgress = _progressImage.fillAmount;

            while (time < 1)
            {
                _progressImage.fillAmount = Mathf.Lerp(initialProgress, progress, time);
                time +=Time.deltaTime * speed;
                _onProgress?.Invoke(_progressImage.fillAmount); // 
                yield return null;
            }

            _progressImage.fillAmount = progress;
            _onProgress?.Invoke(progress);
            _onCompleted.Invoke();
        }
    }
}
