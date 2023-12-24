using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Minigame : MonoBehaviour
    {
        [SerializeField]
        protected float minigameDuration;

        protected int healthToAdd = 0;

        protected virtual void Start()
        {
            // apply the random event modifiers
            ApplyRandomEventModifiers();
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
                            healthToAdd = (int)(DataManager.PLAYER_START_HEALTH_MICROGAME * (effect.amount / 100));
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
    }
}