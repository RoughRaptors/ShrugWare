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

        protected void Start()
        {
            Time.timeScale = GameManager.Instance.GetCurTimeScale();
        }

        protected void HandleMinigameSuccess()
        {
            // should only be null if running the minigame scene on its own
            if (GameManager.Instance)
            {
                GameManager.Instance.MinigameSucceeded();
                GameManager.Instance.LoadScene((int)DataManager.Scenes.MainScene);
            }
        }

        protected void HandleMinigameFailure()
        {
            // should only be null if running the minigame scene on its own
            if (GameManager.Instance)
            {
                GameManager.Instance.MinigameFailed();
                GameManager.Instance.LoadScene((int)DataManager.Scenes.MainScene);
            }
        }
    }
}