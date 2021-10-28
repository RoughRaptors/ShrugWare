using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Minigame : MonoBehaviour
    {
        public enum MinigameCategory
        {
            Friendly = 0,
            Enemy
        }

        protected float minigameDurationRemaining;
        protected MinigameCategory minigameCategory;

        // used to restrict player input and introduce a delay to the beginning of a minigame
        protected float timeElapsed = 0.0f;

        protected void Start()
        {
            minigameDurationRemaining = DataManager.MINIGAME_DURATION_SECONDS;

            // will be null if individually loading scenes
            if (GameManager.Instance)
            {
                Time.timeScale = GameManager.Instance.GetCurTimeScale();
            }
        }

        protected void HandleMinigameEnd(bool wonMinigame)
        {
            // should only be null if running the minigame scene on its own
            if (GameManager.Instance)
            {
                GameManager.Instance.MinigameCompleted(wonMinigame);
                GameManager.Instance.LoadScene((int)DataManager.Scenes.MainScene);
            }
        }
    }
}