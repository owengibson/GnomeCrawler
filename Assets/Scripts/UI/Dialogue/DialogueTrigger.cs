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

        [SerializeField] private bool _doesTriggerPopup;
        [SerializeField] private int _popupNumber;
        [SerializeField] private UnityEvent onPlayerEnterTrigger;

        private bool _canShowPopup = false;

        private void OnEnable()
        {
            EventManager.OnDialogueFinished += DialogueFinished;
        }

        private void OnDisable()
        {
            EventManager.OnDialogueFinished -= DialogueFinished;
        }

        public void DialogueFinished() => _canShowPopup = true;

        public void TriggerDialogue()
        {
            EventManager.OnDialogueStarted(_dialogue);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDialogue();

                onPlayerEnterTrigger?.Invoke();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_doesTriggerPopup && _canShowPopup)
                {
                    EventManager.OnTutoialPopupQuery?.Invoke(_popupNumber);
                }
            }
        }

    }
}
