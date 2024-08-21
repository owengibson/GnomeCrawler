using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GnomeCrawler
{
    public class EndSceneManager : MonoBehaviour
    {
        [SerializeField] private GameObject _signButtonsFirst;
        [SerializeField] private EventSystem _eventSystem;

        void Start()
        {
            EventSystem.current.SetSelectedGameObject(_signButtonsFirst);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

    }
}
