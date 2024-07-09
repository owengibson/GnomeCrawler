using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;

namespace GnomeCrawler
{
    public class TestRoomEvents : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12)) RoomCleared();
            if (Input.GetKeyDown(KeyCode.F11)) RoomStarted();
        }

        [Button]
        public void RoomCleared()
        {
            EventManager.OnRoomCleared?.Invoke();
        }

        [Button]
        public void RoomStarted()
        {
            EventManager.OnRoomStarted?.Invoke(0);
        }
    }
}
