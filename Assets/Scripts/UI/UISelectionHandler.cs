using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GnomeCrawler.UI
{
    public class UISelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _verticalMoveAmount = 30f;
        [SerializeField] private float _moveTime = 0.1f;
        [Range(0f, 2f), SerializeField] private float _scaleAmount = 1.1f;

        private Vector3 _startPos;
        private Vector3 _startScale;


        private void OnEnable()
        {
            Debug.Log(transform.position);
            _startPos = transform.position;
            _startScale = transform.localScale;
        }

        private IEnumerator MoveUI(bool isStartingAnimation)
        {
            Debug.Log("moveui called");
            Vector3 startPos = transform.position;
            Vector3 startScale = transform.localScale;

            Vector3 endPosition;
            Vector3 endScale;

            float counter = 0f;
            while (counter < _moveTime)
            {
                counter += Time.fixedDeltaTime;

                if (isStartingAnimation)
                {
                    endPosition = _startPos + new Vector3(0f, _verticalMoveAmount, 0f);
                    endScale = _startScale * _scaleAmount;
                }
                else
                {
                    endPosition = _startPos;
                    endScale = _startScale;
                }

                Vector3 lerpedPos = Vector3.Lerp(startPos, endPosition, counter / _moveTime);
                Vector3 lerpedScale = Vector3.Lerp(startScale, endScale, counter / _moveTime);

                transform.position = lerpedPos;
                transform.localScale = lerpedScale;

                yield return null;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(MoveUI(true));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(MoveUI(false));
        }
    }
}
