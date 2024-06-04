using DG.Tweening;
using GnomeCrawler.Deckbuilding;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class SwordSpin : Ability
    {
        [SerializeField] private GameObject _regularWeapon;
        [SerializeField] private GameObject _spinWeapon;

        Tween _spinTween;

        private void OnEnable()
        {
            EventManager.OnAttackAbilityToggle?.Invoke(true);

            _regularWeapon.SetActive(false);
            _spinWeapon.SetActive(true);

            _spinTween = _spinWeapon.transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            EventManager.OnAttackAbilityToggle?.Invoke(false);

            _spinTween.Kill();
            _spinWeapon?.SetActive(false);
            _regularWeapon.SetActive(true);
        }
    }
}
