using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ShrugWare
{
    public class AudioManager : MonoBehaviour
    {
        public List<AudioEffectClip> audioEffects = new List<AudioEffectClip>();

        private AudioSource audioSource;

        public static AudioManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        private AudioClip GetAudioClip(DataManager.AudioEffectTypes audioEffectType)
        {
            foreach (AudioEffectClip effectClip in audioEffects)
            {
                if (effectClip.GetAudioEffectType() == audioEffectType)
                {
                    return effectClip.GetAudioClip();
                }
            }

            return null;
        }

        public void PlayAudioClip(DataManager.AudioEffectTypes audioEffectType, float volumeScale = 1)
        {
            if(audioSource == null)
            {
                return;
            }

            audioSource.Stop();

            AudioClip audioClip = GetAudioClip(audioEffectType);
            if(audioClip != null)
            {
                // we want to play the audio clip scaled based on the current timescale
                // ie if timescale is 1.25, that's 25% faster
                audioSource.pitch = GameManager.Instance.GetCurTimeScale();
                audioSource.PlayOneShot(audioClip, volumeScale);
            }
        }

        public void StopAudio()
        {
            audioSource.Stop();
        }
    }
}
