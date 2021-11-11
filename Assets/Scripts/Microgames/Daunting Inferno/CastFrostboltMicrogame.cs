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

        [SerializeField]
        GameObject leftButtonObj;

        [SerializeField]
        GameObject middleButtonObj;

        [SerializeField]
        GameObject rightButtonObj;

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

            fireballButton.gameObject.SetActive(false);
            frostboltButton.gameObject.SetActive(false);
            healButton.gameObject.SetActive(false);

            RandomizeButtons();

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
                    if (!castedFrostbolt)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Clicker";
                    }

                    HandleMicrogameEnd(castedFrostbolt);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";
                }
            }
        }

        public void RandomizeButtons()
        {
            List<Button> buttonList = new List<Button>();
            buttonList.Add(fireballButton);
            buttonList.Add(frostboltButton);
            buttonList.Add(healButton);

            int randIndex = Random.Range(0, 3);
            Button button = buttonList[randIndex];
            button.gameObject.transform.position = leftButtonObj.transform.position;
            buttonList.RemoveAt(randIndex);

            int randIndex2 = Random.Range(0, 2);
            Button button2 = buttonList[randIndex2];
            button2.gameObject.transform.position = middleButtonObj.transform.position;
            buttonList.RemoveAt(randIndex2);

            Button button3 = buttonList[0];
            button3.gameObject.transform.position = rightButtonObj.transform.position;
            buttonList.RemoveAt(0);
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);

            fireballButton.gameObject.SetActive(true);
            frostboltButton.gameObject.SetActive(true);
            healButton.gameObject.SetActive(true);
        }

        public void CastFrostboltButtonPressed()
        {
            castedFrostbolt = true;

            fireballButton.gameObject.SetActive(false);
            frostboltButton.gameObject.SetActive(false);
            healButton.gameObject.SetActive(false);

            instructionsText.gameObject.SetActive(true);
            instructionsText.text = "Chilled Out";
        }
    }
}