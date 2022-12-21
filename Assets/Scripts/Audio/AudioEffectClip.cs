using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    [System.Serializable]
    public class AudioEffectClip
    {
        [SerializeField]
        DataManager.AudioEffectTypes audioEffect;

        [SerializeField]
        AudioClip audioClip;

        public DataManager.AudioEffectTypes GetAudioEffectType() { return audioEffect; }
        public AudioClip GetAudioClip() { return audioClip; }
    }
}