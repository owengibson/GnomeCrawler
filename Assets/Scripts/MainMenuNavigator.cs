using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GnomeCrawler
{
    public class MainMenuNavigator : MonoBehaviour
    {
        [SerializeField] private GameObject _volumePanel;
        private bool _panelOpen = true;

        public void PlayGame()
        {
            SceneManager.LoadScene("Level Generation");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OpenSettings()
        {
            _volumePanel.SetActive(true);
        }
    }
}
