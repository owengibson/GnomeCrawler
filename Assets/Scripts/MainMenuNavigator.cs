using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GnomeCrawler
{
    public class MainMenuNavigator : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene("Level Generation");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
