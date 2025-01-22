using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ShrugWare
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        List<AudioClipData> audioEffects = new List<AudioClipData>();

        [SerializeField]
        List<AudioClip> microgameMusic = new List<AudioClip>();
        public List<AudioClip> GetMicrogameMusic() { return microgameMusic; }

        [SerializeField]
        List<AudioClipData> microgameMusicSO = new List<AudioClipData>();
        public List<AudioClipData> GetMicrogameMusicSO() { return microgameMusicSO; }

        private AudioSource audioSourceMusic;
        private AudioSource audioSourceEffects;
        private AudioSource audioSourceSecondaryEffects;

        public static AudioManager Instance;

        private const float MUSIC_PITCH_MULTIPLY_VALUE = 0.85f;

        private float audioVolume = 100.0f;

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

            if (audioSourceSecondaryEffects == null)
            {
                audioSourceSecondaryEffects = gameObject.AddComponent<AudioSource>();
            }
        }

        private AudioClipData GetAudioClipData(DataManager.AudioType audioEffectType)
        {
            foreach (AudioClipData effectClipData in audioEffects)
            {
                if (effectClipData.audioEffectType == audioEffectType)
                {
                    return effectClipData;
                }
            }

            return null;
        }

        public void PlayAudioClip(DataManager.AudioType audioEffectType, bool secondary = false)
        {
            AudioClipData audioClipData = GetAudioClipData(audioEffectType);
            if(audioClipData != null)
            {
                PlayAudioClip(audioClipData, secondary);   
            }
        }

        public void PlayAudioClip(AudioClipData audioClipData, bool secondary = false)
        {
            if (audioSourceMusic == null || audioSourceEffects == null || audioSourceSecondaryEffects == null)
            {
                return;
            }

            if (!secondary)
            {
                audioSourceEffects.Stop();
            }
            else
            {
                audioSourceSecondaryEffects.Stop();
            }

            // we want to play the audio clip scaled based on the current timescale
            // ie if timescale is 1.25, that's 25% faster
            if (BossGameManager.Instance != null)
            {
                // the audio change on timescale increase is way more noticable than the game speed increase
                // lower the significance of the audio timescale increase
                if (audioClipData.audioEffectType == DataManager.AudioType.BetweenMicrogame)
                {
                    audioSourceMusic.pitch = BossGameManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                    audioSourceEffects.pitch = BossGameManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                    audioSourceSecondaryEffects.pitch = BossGameManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                }
                else
                {
                    audioSourceMusic.pitch = BossGameManager.Instance.GetCurTimeScale();
                    audioSourceEffects.pitch = BossGameManager.Instance.GetCurTimeScale();
                    audioSourceSecondaryEffects.pitch = BossGameManager.Instance.GetCurTimeScale();
                }
            }
            else if (InfiniteModeManager.Instance != null)
            {
                // the audio change on timescale increase is way more noticable than the game speed increase
                // lower the significance of the audio timescale increase
                if (audioClipData.audioEffectType == DataManager.AudioType.BetweenMicrogame)
                {
                    audioSourceMusic.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                    audioSourceEffects.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                    audioSourceSecondaryEffects.pitch = InfiniteModeManager.Instance.GetCurTimeScale() * MUSIC_PITCH_MULTIPLY_VALUE;
                }
            }

            if (!secondary)
            {
                audioSourceEffects.PlayOneShot(audioClipData.clip, audioClipData.maxVolume * audioClipData.volumeMultiplier);
            }
            else
            {
                audioSourceSecondaryEffects.PlayOneShot(audioClipData.clip, audioClipData.maxVolume * audioClipData.volumeMultiplier);
            }
        }

        private void PlayMusicClipSetupHelper(DataManager.AudioType audioType, float extraTimePercent = 1.0f)
        {
            if (audioSourceMusic == null)
            {
                return;
            }
            
            audioSourceMusic.Stop();

            // minigames are normal speed
            if (audioType == DataManager.AudioType.MicrogameMusic)
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
                if(extraTimePercent != 0.0f)
                {
                    audioSourceMusic.pitch *= extraTimePercent;
                }
            }
        }

        public void PlayMusicClip(DataManager.AudioType audioType, float volumeScale = 1)
        {
            PlayMusicClipSetupHelper(audioType);
            AudioClipData audioClip = GetAudioClipData(audioType);
            audioSourceMusic.volume = audioClip.maxVolume * audioClip.volumeMultiplier;
            audioSourceMusic.clip = audioClip.clip;
            audioSourceMusic.Play();
        }

        public void PlayMusicClip(AudioClip audioClip, DataManager.AudioType audioType, float volumeScale = 1, float extraTimePercent = 1.0f)
        {
            PlayMusicClipSetupHelper(audioType, extraTimePercent);
            audioSourceMusic.volume = volumeScale;
            audioSourceMusic.clip = audioClip;
            audioSourceMusic.Play();
        }

        public void PlayMusicClip(AudioClipData audioClipData)
        {
            PlayMusicClipSetupHelper(audioClipData.audioEffectType);
            audioSourceMusic.volume = audioClipData.maxVolume * audioClipData.volumeMultiplier;
            audioSourceMusic.clip = audioClipData.clip;
            audioSourceMusic.Play();
        }

        public void StopAudio()
        {
            audioSourceEffects.Stop();
            audioSourceSecondaryEffects.Stop();
            audioSourceMusic.Stop();
        }

        public AudioClip GetMicrogameAudioClipFromIndex(int index)
        {
            if (index < microgameMusic.Count)
            {
                return microgameMusic[index];
            }

            return null;
        }

        public AudioClipData GetMicrogameAudioClipFromIndexSO(int index)
        {
            if (index < microgameMusicSO.Count)
            {
                return microgameMusicSO[index];
            }

            return null;
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
                audioSourceSecondaryEffects.pitch = curTimeScale * MUSIC_PITCH_MULTIPLY_VALUE;
            }
            else
            {
                audioSourceMusic.pitch = curTimeScale;
                audioSourceEffects.pitch = curTimeScale;
                audioSourceSecondaryEffects.pitch = curTimeScale;
            }
        }

        public float GetAudioVolume()
        {
            return audioVolume;
        }

        public void SetAudioVolume(float newAudioVolume)
        {
            audioVolume = newAudioVolume;
            audioSourceMusic.volume = audioVolume / 100;
            audioSourceEffects.volume = audioVolume / 100;
            audioSourceSecondaryEffects.volume = audioVolume / 100;
        }

        public bool IsMusicPlaying()
        {
            return audioSourceMusic.isPlaying;
        }

        public void LoopMusic(bool looping)
        {
            audioSourceMusic.loop = looping;
        }

        public void PauseMusic()
        {
            audioSourceMusic.Pause();
        }

        public void UnPauseMusic()
        {
            audioSourceMusic.UnPause();
        }
    }
}
