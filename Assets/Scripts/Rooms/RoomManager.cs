using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GnomeCrawler.Systems;
using GnomeCrawler.Audio;

namespace GnomeCrawler.Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _enemies;
        [SerializeField] private Analytics _analyticsScript;

        public void AddEnemyToList(GameObject enemy)
        {
            if (!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        private void RemoveEnemyFromList(GameObject enemy)
        {
            if (!_enemies.Contains(enemy))
                return;

            _enemies.Remove(enemy);

            if (_enemies.Count == 0)
            {
                EventManager.OnRoomCleared?.Invoke();
                AudioManager.Instance.SetMusicParameter(PlayerStatus.PostCombat);
            }
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
