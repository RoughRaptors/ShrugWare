using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class InterruptTheAbility : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        Slider abilityBar;

        [SerializeField]
        Button interruptButton;

        // take some time to spawn to make it challenging
        private bool interruptVisible = false;
        private bool interrupted = false;
        private float castDelay = 0.0f;

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

            interruptButton.gameObject.SetActive(false);
            abilityBar.gameObject.SetActive(false);

            StartCoroutine(DisableInstructionsText());
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;

            // don't "start" the microgame until we can orient the player to the microgame
            if (timeElapsed >= DataManager.SECONDS_TO_START_MICROGAME)
            {
                if (microgameDurationRemaining <= 0.0f)
                {
                    // out of time
                    if (!interrupted)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Clicker";
                    }

                    HandleMicrogameEnd(interrupted);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";

                    if (abilityBar.isActiveAndEnabled && !interrupted)
                    {
                        abilityBar.value += Mathf.Lerp(abilityBar.minValue, abilityBar.maxValue, 
                            Time.deltaTime / (DataManager.MICROGAME_DURATION_SECONDS - castDelay));
                    }
                }
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
            interruptButton.gameObject.SetActive(true);

            castDelay = Random.Range(0.5f, 1.5f);
            Invoke("CastAbility", castDelay);
        }

        private void CastAbility()
        {
            abilityBar.gameObject.SetActive(true);
        }

        public void InterruptButtonPressed()
        {
            if (!abilityBar.isActiveAndEnabled)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Too soon";
            }
            else
            {
                interrupted = true;

                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Good timing";
            }

            interruptButton.gameObject.SetActive(false);
        }
    }
}