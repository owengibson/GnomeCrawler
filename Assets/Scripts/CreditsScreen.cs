using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GnomeCrawler
{
    public class CreditsScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _buttonToSelect;

        private Tween _fadeUp;
        private CanvasGroup _canvasGroup;
        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _fadeUp = _canvasGroup.DOFade(1, 1.5f);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                DOTween.Kill(_fadeUp);
                _canvasGroup.DOFade(0, 1f).OnComplete(() =>
                {
                    EventSystem.current.SetSelectedGameObject(_buttonToSelect);
                    enabled = false;
                });
            }
        }
    }
}
