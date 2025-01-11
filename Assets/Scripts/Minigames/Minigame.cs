using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Minigame : MonoBehaviour
    {
        [SerializeField]
        AudioClipData minigameMusicData;

        protected float minigameDuration = float.MaxValue;
        protected float timeElapsed = 0.0f;
        [NonSerialized] public bool gameOver = false;
        protected bool gameStarted = false;

        protected int healthToAdd = 0;
        protected const float START_DELAY = 1.5f;

        protected virtual void Start()
        {
            // apply the random event modifiers
            ApplyRandomEventModifiers();

            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.PlayMusicClip(minigameMusicData);
                Invoke("RestartMusic", minigameMusicData.clip.length);
            }
        }

        protected virtual void Update()
        {
            if (gameStarted)
            {
                timeElapsed += Time.deltaTime;
            }

            if (timeElapsed >= minigameDuration && !gameOver)
            {
                EndGame(true);
            }
        }

        private void ApplyRandomEventModifiers()
        {
            if (OverworldManager.Instance != null && OverworldManager.Instance.CurRandomEvent != null)
            {
                foreach (DataManager.StatEffect effect in OverworldManager.Instance.CurRandomEvent.eventEffects)
                {
                    if (effect.effectType == DataManager.StatModifierType.PlayerMaxHealth)
                    {
                        if (effect.asPercentage)
                        {
                            healthToAdd = (int)(DataManager.PLAYER_START_HEALTH_MINIGAME * (effect.amount / 100));
                        }
                        else
                        {
                            // nothing yet
                        }
                    }
                }

                // kill it so we don't run it again
                OverworldManager.Instance.CurRandomEvent = null;
            }
        }

        private void RestartMusic()
        {
            OverworldManager.Instance.PlayMusicClip(minigameMusicData);
        }
        
        public void PrematureEndGame(bool victory)
        {
            EndGame(false);
        }

        protected virtual void EndGame(bool success)
        {
            gameOver = true;
        }
    }
}