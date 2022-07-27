using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class InterruptTheAbility : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        Slider abilityBar;

        [SerializeField]
        Button interruptButton;

        // take some time to spawn to make it challenging
        private bool interrupted = false;
        private bool tooSoon = false;
        private float castDelay = 0.0f;

        new private void Start()
        {
            base.Start();

            DataManager.StatEffect damagePlayerEffect = new DataManager.StatEffect();
            damagePlayerEffect.effectType = DataManager.StatModifierType.PlayerCurHealth;
            damagePlayerEffect.amount = 34.0f;
            damagePlayerEffect.asPercentage = false;

            DataManager.StatEffect damageBossEffect = new DataManager.StatEffect();
            damageBossEffect.effectType = DataManager.StatModifierType.BossCurHealth;
            damageBossEffect.amount = 20.0f; 
            damageBossEffect.asPercentage = false;

            DataManager.StatEffect timeScaleEffect = new DataManager.StatEffect();
            timeScaleEffect.effectType = DataManager.StatModifierType.Timescale;
            timeScaleEffect.amount = 0.05f;
            timeScaleEffect.asPercentage = false;

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
                    if (!interrupted && !tooSoon)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Clicker";
                    }

                    HandleMicrogameEnd(interrupted);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();

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
            Invoke(nameof(CastAbility), castDelay);
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
                tooSoon = true;
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