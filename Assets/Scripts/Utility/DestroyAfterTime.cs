using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float _timeToDestroy;

        private void OnEnable()
        {
            StartCoroutine(DoDestroy(_timeToDestroy));
        }

        private IEnumerator DoDestroy(float time)
        {
            yield return new WaitForSeconds(time);

            Destroy(gameObject);
        }
    }
}
