using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public static class DataManager
    {
        // KEEP IN SYNC WITH BUILD SETTINGS
        public enum Scenes
        {
            MainScene = 0, // DO NOT CHANGE THIS
            GetOutOfFire = 1,
            MINIGAME_END = 1 // PUT ALL MINIGAMES BEFORE THIS AND KEEP THE VALUE UP TO DATE
        }

        public static float MINIGAME_DURATION_SECONDS = 3.0f; // we can override this in the child Minigame's Start() - minigameDurationRemaining
        public static float SECONDS_TO_START_MINIGAME = 1.0f; // how long we wait for the minigame to start when on the scene - to orient the player
        public static float SECONDS_BETWEEN_MINIGAMES = 3.0f; // how long we wait before starting another minigame inside of GameManager
    }
}