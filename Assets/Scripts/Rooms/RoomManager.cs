using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GnomeCrawler.Systems;

namespace GnomeCrawler.Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _enemies;

        public void AddEnemyToList(GameObject enemy)
        {
            if (!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        private void RemoveEnemyFromList(GameObject enemy)
        {
            if (_enemies.Contains(enemy))
                _enemies.Remove(enemy);

            if (_enemies.Count == 0)
                EventManager.OnRoomCleared?.Invoke();
        }

        private void OnEnable()
        {
            EventManager.OnEnemyKilled += RemoveEnemyFromList;
        }
        private void OnDisable()
        {
            EventManager.OnEnemyKilled -= RemoveEnemyFromList;
        }
    }
}
