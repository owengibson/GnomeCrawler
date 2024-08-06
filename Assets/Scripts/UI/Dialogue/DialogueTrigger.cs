using GnomeCrawler.Systems;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace GnomeCrawler
{
    public class DialogueTrigger : MonoBehaviour
    {
        public Dialogue _dialogue;

        [SerializeField] private bool _canListenForPopup;
        [ShowIfGroup("a", Condition = "_canListenForPopup")]
        [SerializeField, BoxGroup("a/PopupEvents")] private int _popupToListenFor;
        [SerializeField, BoxGroup("a/PopupEvents")] private bool _delayDialogue;
        [ShowIf("_delayDialogue")]
        [SerializeField, BoxGroup("a/PopupEvents")] private int _dialogueDelayAmount;

        [SerializeField] private bool _doesTriggerPopup;
        [ShowIfGroup("b", Condition = "_doesTriggerPopup")]
        [SerializeField, BoxGroup("b/PopupEmit")] private int _popupToTrigger;
        [SerializeField, BoxGroup("b/PopupEmit")] private bool _delayPopup;
        [ShowIf("_delayPopup")]
        [SerializeField, BoxGroup("b/PopupEmit")] private int _popupDelayAmount;

        [SerializeField] private UnityEvent onPlayerEnterTrigger;
        [SerializeField] private UnityEvent onDialogueFinished;

        private bool _isCurrentDialogue = false;

        private Coroutine _dialogueDelayCO = null;
        private Coroutine _popupDelayCO = null;

        private void OnEnable()
        {
            EventManager.OnDialogueFinished += DialogueFinished;
            EventManager.OnTutorialPopupComplete += CheckForPopupListening;
        }

        private void OnDisable()
        {
            EventManager.OnDialogueFinished -= DialogueFinished;
            EventManager.OnTutorialPopupComplete -= CheckForPopupListening;
        }

        public void DialogueFinished()
        {
            if (!_isCurrentDialogue) return;
            onDialogueFinished?.Invoke();
            _isCurrentDialogue = false;

            if (!_doesTriggerPopup) return;
            if (_delayPopup && _popupDelayCO == null)
            {
                _popupDelayCO = StartCoroutine(DelayShowPopup());
                return;
            }

            EventManager.OnTutoialPopupQuery?.Invoke(_popupToTrigger);
        }

        public void CheckForPopupListening(int popupNumber)
        {
            if (!_canListenForPopup) return;
            if (popupNumber != _popupToListenFor) return;

            if (_delayDialogue && _dialogueDelayCO == null)
            {
                _dialogueDelayCO = StartCoroutine(DelayShowDialogue());
                return;
            }

            TriggerDialogue();
        }

        public void TriggerDialogue()
        {
            EventManager.OnDialogueStarted(_dialogue);

            _isCurrentDialogue = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDialogue();

                onPlayerEnterTrigger?.Invoke();
            }
        }

        private IEnumerator DelayShowDialogue()
        {
            yield return new WaitForSeconds(_dialogueDelayAmount);
            TriggerDialogue();
            _dialogueDelayCO = null;
        }

        private IEnumerator DelayShowPopup()
        {
            yield return new WaitForSeconds(_popupDelayAmount);
            EventManager.OnTutoialPopupQuery?.Invoke(_popupToTrigger);
            _popupDelayCO = null;
        }
    }
}
