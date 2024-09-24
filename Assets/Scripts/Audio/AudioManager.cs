using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ShrugWare
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        List<AudioEffectClip> audioEffects = new List<AudioEffectClip>();

        private AudioSource audioSourceMusic;
        private AudioSource audioSourceEffects;

        public static AudioManager Instance;

        const float MUSIC_PITCH_MULTIPLY_VALUE = 0.85f;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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
                        audioSourceMusic.pitch = BossGameManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                        audioSourceEffects.pitch = BossGameManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                    }
                }
                else if(InfiniteModeManager.Instance != null)
                {
                    // the audio change on timescale increase is way more noticable than the game speed increase
                    // lower the significance of the audio timescale increase
                    if (audioEffectType == DataManager.AudioEffectTypes.BetweenMicrogame)
                    {
                        audioSourceMusic.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                        audioSourceEffects.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                    }
                }

                audioSourceEffects.PlayOneShot(audioClip, volumeScale);
            }
        }

        private void PlayMusicClipSetupHelper(DataManager.AudioEffectTypes audioType)
        {
            if (audioSourceMusic == null)
            {
                return;
            }

            audioSourceMusic.Stop();

            // minigames are normal speed
            if (audioType == DataManager.AudioEffectTypes.MicrogameMusic)
            {
                float curTimeScale = 1;
                if (BossGameManager.Instance != null)
                {
                    curTimeScale = BossGameManager.Instance.GetCurTimeScale();
                }
                else if (InfiniteModeManager.Instance != null)
                {
                    curTimeScale = InfiniteModeManager.Instance.GetCurTimeScale();
                }

                audioSourceMusic.pitch = curTimeScale * MUSIC_PITCH_MULTIPLY_VALUE;
            }
        }

        public void PlayMusicClip(DataManager.AudioEffectTypes audioType, float volumeScale)
        {
            PlayMusicClipSetupHelper(audioType);
            AudioClip audioClip = GetAudioClip(audioType);
            audioSourceMusic.PlayOneShot(audioClip, volumeScale);
        }

        public void PlayMusicClip(AudioClip audioClip, DataManager.AudioEffectTypes audioType, float volumeScale = 1)
        {
            PlayMusicClipSetupHelper(audioType);
            audioSourceMusic.Stop();
            audioSourceMusic.PlayOneShot(audioClip, volumeScale);
        }

        public void StopAudio()
        {
            audioSourceEffects.Stop();
            audioSourceMusic.Stop();
        }

        public void ResetPitch(bool ignoreVal = false)
        {
            float curTimeScale = 1.0f;
            if (BossGameManager.Instance != null)
            {
                curTimeScale = BossGameManager.Instance.GetCurTimeScale();
            }
            else if(InfiniteModeManager.Instance != null)
            {
                curTimeScale = InfiniteModeManager.Instance.GetCurTimeScale();
            }

            if(!ignoreVal)
            {
                audioSourceMusic.pitch = curTimeScale * MUSIC_PITCH_MULTIPLY_VALUE;
                audioSourceEffects.pitch = curTimeScale * MUSIC_PITCH_MULTIPLY_VALUE;
            }
            else
            {
                audioSourceMusic.pitch = curTimeScale;
                audioSourceEffects.pitch = curTimeScale;
            }
        }
    }
}
