using GnomeCrawler.Rooms;
using UnityEngine;

namespace GnomeCrawler.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;

        private RoomManager _room;

        void Start()
        {
            if (transform.parent.TryGetComponent<RoomManager>(out _room))
            {
                Invoke("InstantiateRat", 1);
            }
        }

        private void InstantiateRat()
        {
            GameObject newEnemy = Instantiate(_enemyPrefab, gameObject.transform);
            _room.AddEnemyToList(newEnemy);
        }

    }
}
