using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ShrugWare
{
    public class AudioManager : MonoBehaviour
    {
        public List<AudioEffectClip> audioEffects = new List<AudioEffectClip>();

        private AudioSource audioSourceMusic;
        private AudioSource audioSourceEffects;

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

            if (audioSourceMusic == null)
            {
                audioSourceMusic = gameObject.AddComponent<AudioSource>();
            }

            if (audioSourceEffects == null)
            {
                audioSourceEffects = gameObject.AddComponent<AudioSource>();
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
            if(audioSourceMusic == null || audioSourceEffects == null)
            {
                return;
            }

            audioSourceEffects.Stop();

            AudioClip audioClip = GetAudioClip(audioEffectType);
            if(audioClip != null)
            {
                // we want to play the audio clip scaled based on the current timescale
                // ie if timescale is 1.25, that's 25% faster
                audioSourceMusic.pitch = BossGameManager.Instance.GetCurTimeScale();
                audioSourceEffects.pitch = BossGameManager.Instance.GetCurTimeScale();

                // if it's main menu music, use the audio source for music
                if (audioEffectType == DataManager.AudioEffectTypes.MainMenu)
                {
                    audioSourceMusic.PlayOneShot(audioClip, volumeScale);
                }
                else
                {
                    audioSourceEffects.PlayOneShot(audioClip, volumeScale);
                }
            }
        }

        public void StopAudio()
        {
            audioSourceMusic.Stop();
        }
    }
}
