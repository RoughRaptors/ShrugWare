using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class StackUpForMeteor : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject groupMember1 = null;

        [SerializeField]
        GameObject groupMember2 = null;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;
        private const float DISTANCE_FOR_VALID_STACK = 6.0f;

        private const float PLAYER_MOVE_SPEED = 15.0f;

        private bool stacked = false;

        private float timeRatio = 0;
        private Vector3 meteorStartPos;

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

            SetupPlayerObject();
            meteorStartPos = meteorObject.transform.position;

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
                    // out of time - we should have collided already, but maybe not
                    if (meteorObject.activeInHierarchy)
                    {
                        HandleCollision();
                    }

                    HandleMicrogameEnd(stacked);
                }
                else
                {
                    timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
                    meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, timeRatio);

                    groupMember1.transform.position =
                        Vector3.MoveTowards(groupMember1.transform.position, playerObject.transform.position, PLAYER_MOVE_SPEED * Time.deltaTime);

                    groupMember2.transform.position =
                        Vector3.MoveTowards(groupMember2.transform.position, playerObject.transform.position, PLAYER_MOVE_SPEED * Time.deltaTime);

                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";
                    HandleInput();
                }
            }
        }

        private void HandleInput()
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

        private void SetupPlayerObject()
        {
            float xPos = Random.Range(X_MIN, X_MAX);
            float yPos = Random.Range(Y_MIN, Y_MAX);
            playerObject.transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == meteorObject)
            {
                HandleCollision();
            }
        }

        private void HandleCollision()
        {
            // be lazy, check member distance from player instead of if the meteor is colliding with all 3 objects
            float member1DistanceFromPlayer = Vector3.Distance(groupMember1.transform.position, playerObject.transform.position);
            float member2DistanceFromPlayer = Vector3.Distance(groupMember2.transform.position, playerObject.transform.position);
            if (member1DistanceFromPlayer < DISTANCE_FOR_VALID_STACK && member2DistanceFromPlayer < DISTANCE_FOR_VALID_STACK)
            {
                stacked = true;
                instructionsText.text = "On point";
            }
            else
            {
                instructionsText.text = "Boom";
            }

            instructionsText.gameObject.SetActive(true);

            meteorObject.SetActive(false);
        }
    }
}