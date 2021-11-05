using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class InterceptTheFireball : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject fireballObject = null;

        [SerializeField]
        GameObject healerObject = null;

        private bool intercepted = false;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f; 
        private const float Y_MIN = -25.0f;
        private const float Y_MAX = 10.0f;

        private const float FIREBALL_MOVE_SPEED = 10.0f;
        private const float PLAYER_MOVE_SPEED = 15.0f;

        new private void Start()
        {
            base.Start();

            DataManager.StatEffect damagePlayerEffect;
            damagePlayerEffect.effectType = DataManager.StatEffectType.PlayerHealth;
            damagePlayerEffect.amount = 20.0f;

            DataManager.StatEffect damageBossEffect;
            damageBossEffect.effectType = DataManager.StatEffectType.BossHealth;
            damageBossEffect.amount = 10.0f;

            DataManager.StatEffect timeScaleEffect;
            timeScaleEffect.effectType = DataManager.StatEffectType.TimeScale;
            timeScaleEffect.amount = 0.05f;

            winEffects.Add(damageBossEffect);
            winEffects.Add(timeScaleEffect);

            lossEffects.Add(damagePlayerEffect);
            lossEffects.Add(timeScaleEffect);

            SetupPlayerObject();

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
                    if(!intercepted)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "I smell healer";
                    }

                    HandleMicrogameEnd(intercepted);
                }
                else
                {
                    fireballObject.transform.position = 
                        Vector2.MoveTowards(fireballObject.transform.position, healerObject.transform.position, PLAYER_MOVE_SPEED * Time.deltaTime);

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

        private void SetupPlayerObject()
        {
            float xPos = Random.Range(X_MIN, X_MAX);
            float yPos = Random.Range(Y_MIN, Y_MAX);
            playerObject.transform.position = new Vector3(xPos, yPos, 95.0f);
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
                intercepted = true;

                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Good tank";

                fireballObject.SetActive(false);
            }
        }
    }
}