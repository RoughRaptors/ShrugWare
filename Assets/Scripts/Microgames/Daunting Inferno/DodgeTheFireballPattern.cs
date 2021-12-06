using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class DodgeTheFireballPattern : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject fireballObject1 = null;

        [SerializeField]
        GameObject fireballObject2 = null;

        [SerializeField]
        GameObject fireballObject3 = null;

        private bool intercepted = false;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;

        private const float FIREBALL_MOVE_SPEED = 60.0f;
        private const float PLAYER_MOVE_SPEED = 10.0f;

        new private void Start()
        {
            base.Start();

            DataManager.StatEffect damagePlayerEffect;
            damagePlayerEffect.effectType = DataManager.StatModifierType.PlayerHealth;
            damagePlayerEffect.amount = 34.0f;

            DataManager.StatEffect damageBossEffect;
            damageBossEffect.effectType = DataManager.StatModifierType.BossHealth;
            damageBossEffect.amount = 20.0f;

            DataManager.StatEffect timeScaleEffect;
            timeScaleEffect.effectType = DataManager.StatModifierType.Timescale;
            timeScaleEffect.amount = 0.05f;

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
                    if (playerObject.GetComponent<MeshRenderer>().enabled)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Threaded";
                    }

                    HandleMicrogameEnd(playerObject.GetComponent<MeshRenderer>().enabled);
                }
                else
                {
                    fireballObject1.transform.position = 
                        Vector3.MoveTowards(fireballObject1.transform.position, new Vector3(-100, fireballObject1.transform.position.y, 0),
                        FIREBALL_MOVE_SPEED * (Random.Range(1, 1.5f) * Time.deltaTime));

                    fireballObject2.transform.position =
                        Vector3.MoveTowards(fireballObject2.transform.position, new Vector3(-100, fireballObject2.transform.position.y, 0),
                        FIREBALL_MOVE_SPEED * (Random.Range(1, 1.5f) * Time.deltaTime));

                    fireballObject3.transform.position =
                        Vector3.MoveTowards(fireballObject3.transform.position, new Vector3(-100, fireballObject3.transform.position.y, 0),
                        FIREBALL_MOVE_SPEED * (Random.Range(1, 1.5f) * Time.deltaTime));

                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";
                    HandleInput();
                }
            }
        }

        private void HandleInput()
        {
            if (!intercepted)
            {
                Vector3 newPos = playerObject.transform.position;
                if (Input.GetKey(KeyCode.W))
                {
                    newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    newPos.y -= PLAYER_MOVE_SPEED * Time.deltaTime;
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
            playerObject.GetComponent<MeshRenderer>().enabled = false;

            instructionsText.gameObject.SetActive(true);
            instructionsText.text = "Like a leaf on the wind";
        }
    }
}