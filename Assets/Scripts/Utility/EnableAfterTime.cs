using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnableAfterTime : MonoBehaviour
    {
        public GameObject objectToEnable;  
        public float delayInSeconds = 5.0f; 

        private void Start()
        {
            
            StartCoroutine(EnableObjectAfterDelay());
        }

        private IEnumerator EnableObjectAfterDelay()
        {
          
            yield return new WaitForSeconds(delayInSeconds);

            // Enable the GameObject
            objectToEnable.SetActive(true);
        }
    }
}
