using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Microgame : MonoBehaviour
    {
        // todo - add string instructionsTextString field to allow for cleaner, more data driven code

        protected float microgameDurationRemaining;

        // used to restrict player input and introduce a delay to the beginning of a microgame
        protected float timeElapsed = 0.0f;

        // the effects to trigger if you win or lose
        protected List<DataManager.StatEffect> winEffects = new List<DataManager.StatEffect>();
        protected List<DataManager.StatEffect> lossEffects = new List<DataManager.StatEffect>();

        // temp hack to allow for a brief pause in between microgame timers running out
        private bool hasRunEndCondition = false;

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
            if (GameManager.Instance && !hasRunEndCondition)
            {
                hasRunEndCondition = true;
                if (wonMicrogame)
                {
                    RunEffects(winEffects);
                }
                else
                {
                    RunEffects(lossEffects);
                }

                // defer this a bit to allow the player to see the results at the end of the time limit
                StartCoroutine(LoadScene(wonMicrogame));
            }
        }

        public IEnumerator LoadScene(bool wonMicrogame)
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            
            GameManager.Instance.MicrogameCompleted(wonMicrogame);
        }

        private void RunEffects(List<DataManager.StatEffect> effects)
        {
            GameManager.Instance.ClearPreviousEffects();
            foreach (DataManager.StatEffect effect in effects)
            {
                RunEffect(effect);
            }
        }

        private void RunEffect(DataManager.StatEffect effect)
        {
            GameManager.Instance.AddPreviouslyRanEffect(effect);

            if (effect.effectType == DataManager.StatModifierType.PlayerCurHealth)
            {
                GameManager.Instance.TakeDamage(effect.amount);
            }
            else if (effect.effectType == DataManager.StatModifierType.BossCurHealth)
            {
                GameManager.Instance.DamageBoss(effect.amount);
            }
            else if(effect.effectType == DataManager.StatModifierType.Timescale)
            {
                GameManager.Instance.ModifyTimeScale(effect.amount);
            }
        }
    }
}