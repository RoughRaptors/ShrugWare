using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ShrugWare
{
    // create a list of 3 abilities and randomize them, we need to press their buttons in the correct order
    public class PerformTheRotation : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject frostboltButtonObj = null;

        [SerializeField]
        GameObject glacialSpileButtonObj = null;

        [SerializeField]
        GameObject iceBlastButtonObj = null;

        private bool failed = false;

        private enum RotationMapping
        {
            Frostbolt = 0,
            GlacialSpike = 1,
            IceBlast = 2
        }

        private List<RotationMapping> rotation = new List<RotationMapping>();

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

            // get a random, 3 button rotation
            rotation.Add((RotationMapping)0);
            rotation.Add((RotationMapping)1);
            rotation.Add((RotationMapping)2);
            rotation = rotation.OrderBy(i => Random.value).ToList();

            instructionsText.text = rotation[0].ToString() + " -> " + rotation[1].ToString() + " -> " + rotation[2];
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
                    if (rotation.Count > 0 && !failed)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Bad DPS";
                    }

                    HandleMicrogameEnd(rotation.Count == 0);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                }
            }
        }

        // 0
        public void FrostboltButtonPressed()
        {
            if(rotation[0] == RotationMapping.Frostbolt)
            {
                rotation.RemoveAt(0);
                frostboltButtonObj.SetActive(false);
            }
            else
            {
                HandleLoseGame();
            }

            CheckEndCondition();
        }

        // 1
        public void GlacialSpikeButtonPressed()
        {
            if (rotation[0] == RotationMapping.GlacialSpike)
            {
                rotation.RemoveAt(0);
                glacialSpileButtonObj.SetActive(false);
            }
            else
            {
                HandleLoseGame();
            }

            CheckEndCondition();
        }

        // 2
        public void IceBlastButtonPressed()
        {
            if (rotation[0] == RotationMapping.IceBlast)
            {
                rotation.RemoveAt(0);
                iceBlastButtonObj.SetActive(false);
            }
            else
            {
                HandleLoseGame();
            }

            CheckEndCondition();
        }

        private void CheckEndCondition()
        {
            if(rotation.Count == 0)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "PEW PEW PEW";
            }
        }

        private void HandleLoseGame()
        {
            frostboltButtonObj.SetActive(false);
            glacialSpileButtonObj.SetActive(false);
            iceBlastButtonObj.SetActive(false);

            instructionsText.gameObject.SetActive(true);
            instructionsText.text = "Wrong rotation";

            failed = true;
        }
    }
}