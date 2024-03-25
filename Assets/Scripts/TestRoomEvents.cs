using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GnomeCrawler.Systems;
using Sirenix.OdinInspector;

namespace GnomeCrawler
{
    public class TestRoomEvents : MonoBehaviour
    {
        [Button]
        public void RoomCleared()
        {
            EventManager.OnRoomCleared?.Invoke();
        }

        [Button]
        public void RoomStarted()
        {
            EventManager.OnRoomStarted?.Invoke();
        }
    }
}
