using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class KnockbackedSafely : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject arrowObj = null;

        [SerializeField]
        GameObject safeZoneObj = null;

        private bool inSafeZone = false;
        private bool knockbacked = false;

        private const float X_MIN = -30.0f;
        private const float X_MAX = 30.0f;

        private const float PLAYER_MOVE_SPEED = 15.0f;

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

            SetupSafeZone();

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
                    if (!knockbacked)
                    {
                        ApplyKnockback();
                        Invoke(nameof(EndMicrogame), 1.5f);
                    }
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";
                    HandleInput();

                    float angleRad = Mathf.Atan2(playerObject.transform.position.y - arrowObj.transform.position.y,
                        playerObject.transform.position.x - arrowObj.transform.position.x);

                    float angleDeg = (180 / Mathf.PI) * angleRad;
                    arrowObj.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
                }
            }
        }

        private void SetupSafeZone()
        {
            safeZoneObj.SetActive(false);
            float xPos = Random.Range(X_MIN, X_MAX);
            safeZoneObj.transform.position = new Vector3(xPos, -12, 0);
        }

        // this is a bit of a special case. the microgame ends with a knockback, and we need to wait for the collision with a safe zone to end it
        private void EndMicrogame()
        {
            if(!inSafeZone)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Fell off";
            }

            HandleMicrogameEnd(inSafeZone);
        }

        private void HandleInput()
        {
            if (!inSafeZone)
            {
                Vector3 newPos = playerObject.transform.position;
                if (Input.GetKey(KeyCode.W))
                {
                    newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.S) && playerObject.transform.position.y > -5.0f)
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

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
            safeZoneObj.SetActive(true);
        }

        private void ApplyKnockback()
        {
            knockbacked = true;

            Vector3 dir = playerObject.transform.position - arrowObj.transform.position;
            dir.z = 0;
            dir = dir.normalized;
            playerObject.GetComponent<Rigidbody>().AddForce(dir * 2500);

            arrowObj.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == safeZoneObj)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Wheeeee!";

                inSafeZone = true;

                playerObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }
}