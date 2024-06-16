using DinoFracture;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class PlayerFractureHandler : MonoBehaviour
    {
        private void Fracture()
        {
            GetComponent<FractureGeometry>().FractureAndForget();
        }

        private void OnEnable()
        {
            EventManager.OnPlayerKilled += Fracture;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerKilled -= Fracture;
        }
    }
}
