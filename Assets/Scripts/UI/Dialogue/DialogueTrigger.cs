using GnomeCrawler.Systems;
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
        [SerializeField] private int _popupToListenFor;
        [SerializeField] private bool _doesTriggerPopup;
        [SerializeField] private int _popupToTrigger;
        [SerializeField] private UnityEvent onPlayerEnterTrigger;
        [SerializeField] private UnityEvent onDialogueFinished;

        private bool _isCurrentDialogue = false;

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
            EventManager.OnTutoialPopupQuery?.Invoke(_popupToTrigger);
        }

        public void CheckForPopupListening(int popupNumber)
        {
            if (!_canListenForPopup) return;
            if (popupNumber != _popupToListenFor) return;

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
    }
}
