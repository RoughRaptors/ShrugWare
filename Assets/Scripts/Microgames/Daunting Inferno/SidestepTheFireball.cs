using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class SidestepTheFireball : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject fireballObject = null;

        private bool intercepted = false;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;

        private const float FIREBALL_MOVE_SPEED = 30.0f;
        private const float PLAYER_MOVE_SPEED = 11.0f;

        new private void Start()
        {
            base.Start();

            playerObject.transform.GetChild(0).gameObject.SetActive(true);

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
                    if (playerObject.activeInHierarchy)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Slightly singed";
                    }

                    HandleMicrogameEnd(playerObject.activeInHierarchy);
                }
                else
                {
                    fireballObject.transform.position = 
                        Vector3.MoveTowards(fireballObject.transform.position, new Vector3(0, -60, 0), FIREBALL_MOVE_SPEED * Time.deltaTime);

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                    HandleInput();
                }
            }
        }

        private void HandleInput()
        {
            if (!intercepted)
            {
                Vector3 newPos = playerObject.transform.position;
                if (Input.GetKey(KeyCode.A))
                {
                    newPos.x -= PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    newPos.x += PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                playerObject.transform.position = newPos;
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == fireballObject)
            {
                playerObject.transform.GetChild(0).gameObject.SetActive(false);

                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Be faster";
            }
        }
    }
}