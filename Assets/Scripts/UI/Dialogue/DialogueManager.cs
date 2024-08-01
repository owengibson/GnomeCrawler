using GnomeCrawler.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GnomeCrawler
{
    public class DialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;

        public UnityEvent OnDialogueStartTyping;
        public UnityEvent OnDialogueStopTyping;
        public UnityEvent OnDialogueSkip;
        public UnityEvent OnDialogueStart;
        public UnityEvent OnDialogueEnd;

        [SerializeField] private Animator _dialoguePanelAnimator;

        private PlayerControls _playerControls;
        private Queue<string> _sentences;

        private Coroutine _typingCoroutine;
        private string _currentSentence;

        private bool _inDialogue = false;

        private void OnEnable()
        {
            _playerControls = new PlayerControls();
            _playerControls.Enable();
            _playerControls.Player.Jump.performed += SkipDialogue;

            EventManager.OnDialogueStarted += StartDialogue;
        }

        private void Start()
        {
            _sentences = new Queue<string>();
        }

        private void SkipDialogue(InputAction.CallbackContext unused)
        {
            if (!_inDialogue)
                return;

            if (_typingCoroutine == null)
            {
                OnDialogueSkip?.Invoke();
                DisplayNextSentence();
            }
            else
            {
                OnDialogueStopTyping?.Invoke();
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
                dialogueText.text = _currentSentence;
            }
        }

        public void StartDialogue(Dialogue dialogue)
        {
            Debug.Log("starting conversation with " + dialogue._name);

            _inDialogue = true;

            OnDialogueStart?.Invoke();

            OpenOrCloseDialoguePanel(true);

            nameText.text = dialogue._name;

            _sentences.Clear();

            foreach (string sentence in dialogue._sentences)
            {
                _sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (_sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            _currentSentence = _sentences.Dequeue();
            StopAllCoroutines();
            OnDialogueStartTyping?.Invoke();
            _typingCoroutine = StartCoroutine(TypeSentence(_currentSentence));
        }

        IEnumerator TypeSentence(string sentence)
        {
            dialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.01f);
            }
            _typingCoroutine = null;
            OnDialogueStopTyping?.Invoke();
        }

        private void EndDialogue()
        {
            OpenOrCloseDialoguePanel(false);
            Debug.Log("End of conversation");
            OnDialogueEnd?.Invoke();

            _inDialogue = false;
        }

        private void OpenOrCloseDialoguePanel(bool doOpen)
        {
            if (doOpen)
            {
                _dialoguePanelAnimator.SetBool("IsOpen", true);
            }
            else
            {
                _dialoguePanelAnimator.SetBool("IsOpen", false);
                EventManager.OnDialogueFinished?.Invoke();
            }
            
        }

        void OnDisable()
        {
            EventManager.OnDialogueStarted -= StartDialogue;
        }
    }
}
