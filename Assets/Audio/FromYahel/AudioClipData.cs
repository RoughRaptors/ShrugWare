using System;
using UnityEngine;

namespace ShrugWare
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewAudioClipData", menuName = "ScriptableObjects/AudioClipData", order = 2)]
    public class AudioClipData : ScriptableObject
    {
        public AudioClip clip;
        [Range(0.1f, 1f)]
        public float maxVolume = 1f;
        [Range(.1f, 3f)]
        public float pitch = 1f;
        public bool randomPitch = false;
        [Range(-1, 1)]
        public float variablePitchIfRandom;
        public DataManager.AudioType audioEffectType;
        public float volumeMultiplier = 1.0f;
    }
}
