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

        public static float SECONDS_BETWEEN_MINIGAMES = 3.0f;
    }
}