using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Rooms
{
    public class RoomDoorway : MonoBehaviour
    {
        public enum DoorwayType { Entry, Exit };
        public BoxCollider Collider { get; private set; }

        public DoorwayType Type;

        [SerializeField] private RoomDoorway _otherDoorway;
        [SerializeField] private GameObject _doorParticles; 

        private bool _hasExitBeenSet = false;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();

            if (_otherDoorway == null)
                Debug.LogError("Other doorway has not been set in " + transform.parent);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (!_hasExitBeenSet)
            {
                _hasExitBeenSet = true;
                EventManager.OnRoomStarted?.Invoke();

                Type = DoorwayType.Entry;
                _otherDoorway.Type = DoorwayType.Exit;
                _otherDoorway.Collider.isTrigger = false;
                _otherDoorway._doorParticles.SetActive(true);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (Type == DoorwayType.Entry)
            {
                Collider.isTrigger = false;
                _doorParticles.SetActive(true);
            }
        }

        private void OpenRoomExit()
        {
            if (Type != DoorwayType.Exit)
                return;

            Collider.enabled = false;
            _doorParticles.SetActive(false);
        }

        private void OnEnable()
        {
            EventManager.OnRoomCleared += OpenRoomExit;
        }
        private void OnDisable()
        {
            EventManager.OnRoomCleared -= OpenRoomExit;
        }
    }
}
