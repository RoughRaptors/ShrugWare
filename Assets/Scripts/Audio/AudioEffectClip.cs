using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    [System.Serializable]
    public class AudioEffectClip
    {
        [SerializeField]
        DataManager.AudioType audioEffect;

        [SerializeField]
        AudioClip audioClip;

        public DataManager.AudioType GetAudioEffectType() { return audioEffect; }
        public AudioClip GetAudioClip() { return audioClip; }
    }
}