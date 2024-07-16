using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class DialogueTrigger : MonoBehaviour
    {
        public Dialogue _dialogue;

        public void TriggerDialogue()
        {
            EventManager.OnDialogueStarted(_dialogue);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }
    }
}
