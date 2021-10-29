using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Microgame : MonoBehaviour
    {
        public enum MicrogameCategory
        {
            Friendly = 0,
            Enemy
        }

        protected float microgameDurationRemaining;
        protected MicrogameCategory microgameCategory;

        // used to restrict player input and introduce a delay to the beginning of a microgame
        protected float timeElapsed = 0.0f;

        protected void Start()
        {
            microgameDurationRemaining = DataManager.MICROGAME_DURATION_SECONDS;

            // will be null if individually loading scenes
            if (GameManager.Instance)
            {
                Time.timeScale = GameManager.Instance.GetCurTimeScale();
            }
        }

        protected void HandleMicrogameEnd(bool wonMicrogame)
        {
            // should only be null if running the microgame scene on its own
            if (GameManager.Instance)
            {
                GameManager.Instance.MicrogameCompleted(wonMicrogame);
                GameManager.Instance.LoadScene((int)DataManager.Scenes.MainScene);
            }
        }
    }
}