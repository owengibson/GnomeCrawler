using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GnomeCrawler.UI
{
    public class UISelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private float _verticalMoveAmount = 30f;
        [SerializeField] private float _moveTime = 0.1f;
        [Range(0f, 2f), SerializeField] private float _scaleAmount = 1.1f;

        private Vector3 _startPos;
        private Vector3 _startScale;


        private void OnEnable()
        {
            _startPos = transform.position;
            _startScale = transform.localScale;
        }

        private IEnumerator MoveUI(bool isStartingAnimation)
        {
            Vector3 startPos = transform.position;
            Vector3 startScale = transform.localScale;

            Vector3 endPosition;
            Vector3 endScale;

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

            float counter = 0f;
            while (counter < _moveTime)
            {
                counter += Time.fixedDeltaTime;

                // Calculate new position/scale
                Vector3 lerpedPos = Vector3.Lerp(startPos, endPosition, counter / _moveTime);
                Vector3 lerpedScale = Vector3.Lerp(startScale, endScale, counter / _moveTime);

                // Apply calculated pos/scale
                transform.position = lerpedPos;
                transform.localScale = lerpedScale;

                yield return null;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.selectedObject = gameObject;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            eventData.selectedObject = null;
        }

        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(MoveUI(true));
        }

        public void OnDeselect(BaseEventData eventData)
        {
            StartCoroutine (MoveUI(false));
        }
    }
}
