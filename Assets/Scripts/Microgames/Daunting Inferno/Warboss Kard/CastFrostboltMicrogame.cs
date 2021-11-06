using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class CastFrostboltMicrogame : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        Button fireballButton;

        [SerializeField]
        Button frostboltButton;

        [SerializeField]
        Button healButton;

        private bool castedFrostbolt = false;

        new private void Start()
        {
            base.Start();

            DataManager.StatEffect damagePlayerEffect;
            damagePlayerEffect.effectType = DataManager.StatEffectType.PlayerHealth;
            damagePlayerEffect.amount = 34.0f;

            DataManager.StatEffect damageBossEffect;
            damageBossEffect.effectType = DataManager.StatEffectType.BossHealth;
            damageBossEffect.amount = 20.0f;

            DataManager.StatEffect timeScaleEffect;
            timeScaleEffect.effectType = DataManager.StatEffectType.TimeScale;
            timeScaleEffect.amount = 0.05f;

            winEffects.Add(damageBossEffect);
            winEffects.Add(timeScaleEffect);

            lossEffects.Add(damagePlayerEffect);
            lossEffects.Add(timeScaleEffect);

            StartCoroutine("DisableInstructionsText");
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;

            // don't "start" the microgame until we can orient the player to the microgame
            if (timeElapsed >= DataManager.SECONDS_TO_START_MICROGAME)
            {
                microgameDurationRemaining -= Time.deltaTime;
                timerText.text = microgameDurationRemaining.ToString("F2") + "s";

                if (microgameDurationRemaining <= 0.0f)
                {
                    // out of time
                    HandleMicrogameEnd(castedFrostbolt);
                }
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
        }

        public void CastFrostboltButtonPressed()
        {
            castedFrostbolt = true;

            fireballButton.gameObject.SetActive(false);
            frostboltButton.gameObject.SetActive(false);
            healButton.gameObject.SetActive(false);

            instructionsText.gameObject.SetActive(true);
            instructionsText.text = "Ice Cold Boss";
        }
    }
}