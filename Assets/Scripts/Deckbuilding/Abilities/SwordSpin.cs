using DG.Tweening;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Deckbuilding
{
    public class SwordSpin : Ability
    {
        [SerializeField] private Transform _weapon;
        [SerializeField] private Transform _regularWeaponParent;
        [SerializeField] private Transform _spinWeaponParent;
        [SerializeField] private Transform _spinWeaponRoot;

        Tween _spinTween;

        private void OnEnable()
        {
            EventManager.OnAttackAbilityToggle?.Invoke(true);

            _regularWeaponParent.gameObject.SetActive(false);
            _spinWeaponRoot.gameObject.SetActive(true);
            _weapon.parent = _spinWeaponParent;
            _weapon.localPosition = Vector3.zero;
            _weapon.localEulerAngles = Vector3.zero;

            _spinTween = _spinWeaponRoot.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            EventManager.OnAttackAbilityToggle?.Invoke(false);
            _weapon.parent = _regularWeaponParent;
            _weapon.localPosition = Vector3.zero;
            _weapon.localEulerAngles = Vector3.zero;

            _spinTween.Kill();
            _spinWeaponRoot.gameObject.SetActive(false);
            _regularWeaponParent.gameObject.SetActive(true);
        }
    }
}
