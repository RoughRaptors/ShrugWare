using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class DataManager
    {
        // KEEP IN SYNC WITH BUILD SETTINGS
        public enum Scenes
        {
            MainScene = 0, // DO NOT CHANGE THIS
            InfernalDawnGetOutOfFire = 1,
            MICROGAME_END = 1 // PUT ALL MICROGAMES BEFORE THIS AND KEEP THE VALUE UP TO DATE
        }

        public enum StatEffectType
        {
            PlayerHealth = 0,
            BossHealth
        }

        public struct StatEffect
        {
            public StatEffectType effectType;
            public float amount;
        }

        public static float MICROGAME_DURATION_SECONDS = 3.0f; // we can override this in the child Microgame's Start() - microgameDurationRemaining
        public static float SECONDS_TO_START_MICROGAME = 1.0f; // how long we wait for the microgame to start when on the scene - to orient the player
        public static float SECONDS_BETWEEN_MICROGAMES = 3.0f; // how long we wait before starting another microgame inside of GameManager
    }
}