using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class GetOutOfFireMinigame : Minigame
    {
        [SerializeField]
        Text timerText;

        new private void Start()
        {
            base.Start();

            minigameDurationRemaining = 3.0f;
            minigameCategory = MinigameCategory.Friendly;
        }

        private void Update()
        {
            minigameDurationRemaining -= Time.deltaTime;
            timerText.text = minigameDurationRemaining.ToString("F2") + "s";

            float timeScale = Time.timeScale;

            if (minigameDurationRemaining <= 0.0f)
            {
                // out of time
                HandleMinigameFailure();
            }
        }
    }
}