using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GnomeCrawler.Systems;

namespace GnomeCrawler.Deckbuilding
{
    public class WeaponGrowth : Ability
    {
        [SerializeField] private Transform _weaponHolder;

        private void OnEnable()
        {

            float growthAmount = Card.AbilityValues[0].value;
            //if (_weaponHolder.localScale == Vector3.one * growthAmount) return;

            //DOTween.KillAll();
            //_weaponHolder.DOScale(growthAmount, 1f);

            StartCoroutine(ChangeWeaponSize(growthAmount, 1));
            EventManager.OnWeaponSizeChanged?.Invoke(growthAmount);
        }

        private void OnDisable()
        {
            //_weaponHolder.DOScale(1, 1f);

            StartCoroutine(ChangeWeaponSize(1, 1));
            EventManager.OnWeaponSizeChanged?.Invoke(1);
        }

        private IEnumerator ChangeWeaponSize(float growthAmount, float time)
        {
            float counter = 0f;
            Vector3 startingScale = _weaponHolder.localScale;

            while (counter < time)
            {
                _weaponHolder.localScale = Vector3.Lerp(_weaponHolder.localScale, Vector3.one * growthAmount, counter / time);
                counter += Time.deltaTime;

                yield return null;
            }

            yield return null;
        }
    }
}
