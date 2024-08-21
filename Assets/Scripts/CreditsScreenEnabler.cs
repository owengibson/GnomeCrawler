using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class CreditsScreenEnabler : MonoBehaviour
    {
        [SerializeField] private GameObject _credits;

        public void EnableCreditsScreen()
        {
            _credits.SetActive(true);
        }
    }
}
