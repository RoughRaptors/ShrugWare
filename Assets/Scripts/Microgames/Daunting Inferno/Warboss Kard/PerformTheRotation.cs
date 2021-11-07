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
        Text timerText = null;

        [SerializeField]
        GameObject frostboltButtonObj = null;

        [SerializeField]
        GameObject glacialSpileButtonObj = null;

        [SerializeField]
        GameObject iceBlastButtonObj = null;

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
                    if (rotation.Count > 0)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Bad DPS";
                    }

                    HandleMicrogameEnd(rotation.Count == 0);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";
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
    }
}