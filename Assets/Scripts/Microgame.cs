using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Microgame : MonoBehaviour
    {
        protected float microgameDurationRemaining;

        // used to restrict player input and introduce a delay to the beginning of a microgame
        protected float timeElapsed = 0.0f;

        // the effects to trigger if you win or lose
        protected List<DataManager.StatEffect> winEffects = new List<DataManager.StatEffect>();
        protected List<DataManager.StatEffect> lossEffects = new List<DataManager.StatEffect>();

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

                if(wonMicrogame)
                {
                    RunEffects(winEffects);
                }
                else
                {
                    RunEffects(lossEffects);
                }

                GameManager.Instance.MicrogameCompleted(wonMicrogame);
                GameManager.Instance.LoadScene((int)DataManager.Scenes.MainScene);
            }
        }

        private void RunEffects(List<DataManager.StatEffect> effects)
        {
            foreach (DataManager.StatEffect effect in effects)
            {
                RunEffect(effect);
            }
        }

        private void RunEffect(DataManager.StatEffect effect)
        {
            if(effect.effectType == DataManager.StatEffectType.PlayerHealth)
            {
                GameManager.Instance.TakeDamage(effect.amount);
            }
            else if (effect.effectType == DataManager.StatEffectType.BossHealth)
            {
                GameManager.Instance.DamageBoss(effect.amount);
            }
            else if(effect.effectType == DataManager.StatEffectType.TimeScale)
            {
                GameManager.Instance.ModifyTimeScale(effect.amount);
            }
        }
    }
}