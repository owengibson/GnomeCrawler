using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        private List<EventInstance> _eventInstances = new List<EventInstance>();
        private List<StudioEventEmitter> _eventEmitters = new List<StudioEventEmitter>();

        private EventInstance _musicEventInstance;

        private void Start()
        {
            InitialiseMusic(FMODEvents.Instance.GetEventReference("Music"));
        }

        private void InitialiseMusic(EventReference musicEventReference)
        {
            _musicEventInstance = CreateEventInstance(musicEventReference);
            _musicEventInstance.start();
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
