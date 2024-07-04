using GnomeCrawler.Audio;
using GnomeCrawler.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Rooms
{
    public class RoomDoorway : MonoBehaviour
    {
        public enum DoorwayType { Entry, Exit, HealingRoom, BossRoom };
        public BoxCollider Collider { get; private set; }

        public DoorwayType Type;

        [SerializeField] private RoomDoorway _otherDoorway;
        [SerializeField] private GameObject _doorParticles; 
        [SerializeField] private ParticleSystem _beacon; 

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

            if (Type == DoorwayType.HealingRoom)
            {
                _otherDoorway.gameObject.SetActive(false);
                AudioManager.Instance.SetMusicParameter(PlayerStatus.StartRoom);
                return;
            }

            if (Type == DoorwayType.BossRoom)
            {
                EventManager.OnEnteredBossRoom?.Invoke();
                EventManager.OnRoomStarted?.Invoke(transform.parent.GetHashCode());
                return;
            }

            if (!_hasExitBeenSet)
            {
                _hasExitBeenSet = true;
                EventManager.OnRoomStarted?.Invoke(transform.parent.GetHashCode());

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

            if (Type == DoorwayType.BossRoom)
            {
                Collider.isTrigger = false;
                _doorParticles.SetActive(true);
                return;
            }
        }

        private void OpenRoomExit()
        {
            if (Type != DoorwayType.Exit)
                return;

            Collider.enabled = false;
            _doorParticles.SetActive(false);
            _beacon.Play();
        }

        private void ClearBeaconParticles(int unused)
        {
            _beacon.Stop();
        }

        private void OnEnable()
        {
            EventManager.OnRoomCleared += OpenRoomExit;
            EventManager.OnRoomStarted += ClearBeaconParticles;
        }
        private void OnDisable()
        {
            EventManager.OnRoomCleared -= OpenRoomExit;
            EventManager.OnRoomStarted -= ClearBeaconParticles;
        }
    }
}
