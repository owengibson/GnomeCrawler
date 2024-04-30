using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Volume")]
        [Range(0f, 1f)] public float MasterVolume = 1f;
        [Range(0f, 1f)] public float MusicVolume = 1f;
        [Range(0f, 1f)] public float AmbienceVolume = 1f;
        [Range(0f, 1f)] public float SfxVolume = 1f;

        private Bus _masterBus;
        private Bus _musicBus;
        private Bus _ambienceBus;
        private Bus _sfxBus;

        private List<EventInstance> _eventInstances = new List<EventInstance>();
        private List<StudioEventEmitter> _eventEmitters = new List<StudioEventEmitter>();

        private EventInstance _musicEventInstance;
        private EventInstance _ambienceEventInstance;

        private void Awake()
        {
            _masterBus = RuntimeManager.GetBus("bus:/");
            _musicBus = RuntimeManager.GetBus("bus:/Music");
            _ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
            _sfxBus = RuntimeManager.GetBus("bus:/SFX");

            UpdateBuses();
        }

        private void Start()
        {
            InitialiseMusic(FMODEvents.Instance.GetEventReference("Music"));
            InitialiseAmbience(FMODEvents.Instance.GetEventReference("Ambience"));
        }

        public void UpdateBuses()
        {
            _masterBus.setVolume(MasterVolume);
            _musicBus.setVolume(MusicVolume);
            _ambienceBus.setVolume(AmbienceVolume);
            _sfxBus.setVolume(SfxVolume);
        }

        private void InitialiseMusic(EventReference musicEventReference)
        {
            _musicEventInstance = CreateEventInstance(musicEventReference);
            _musicEventInstance.start();
        }

        private void InitialiseAmbience(EventReference ambienceEventReference)
        {
            _ambienceEventInstance = CreateEventInstance(ambienceEventReference);
            _ambienceEventInstance.start();
        }

        public void SetMusicParameter(PlayerStatus status)
        {
            _musicEventInstance.setParameterByName("PLAYER_STATUS", (float)status);
        }

        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }

        public void PlayOneShot(EventReference sound)
        {
            RuntimeManager.PlayOneShot(sound);
        }

        public EventInstance CreateEventInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }

        public StudioEventEmitter InitialiseEventEmitter(EventReference eventReference, GameObject emitterGameObject)
        {
            StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            emitter.EventReference = eventReference;
            _eventEmitters.Add(emitter);
            return emitter;
        }

        private void CleanUp()
        {
            // stop and release any created event instances
            foreach (var eventInstance in _eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }

            //stop all event emitters
            foreach (var eventEmitter in _eventEmitters)
            {
                eventEmitter.Stop();
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}
