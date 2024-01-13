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
                if (BossGameManager.Instance != null)
                {
                    // the audio change on timescale increase is way more noticable than the game speed increase
                    // lower the significance of the audio timescale increase
                    if (audioEffectType == DataManager.AudioEffectTypes.BetweenMicrogame)
                    {
                        audioSourceMusic.pitch = BossGameManager.Instance.GetCurTimeScale() * 0.85f;
                        audioSourceEffects.pitch = BossGameManager.Instance.GetCurTimeScale() * 0.85f;
                    }
                }
                else if(InfiniteModeManager.Instance != null)
                {
                    // the audio change on timescale increase is way more noticable than the game speed increase
                    // lower the significance of the audio timescale increase
                    if (audioEffectType == DataManager.AudioEffectTypes.BetweenMicrogame)
                    {
                        audioSourceMusic.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * 0.85f;
                        audioSourceEffects.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * 0.85f;
                    }
                }

                // if it's main menu music, use the audio source for music
                if (audioEffectType == DataManager.AudioEffectTypes.MainMenu || audioEffectType == DataManager.AudioEffectTypes.Overworld)
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
