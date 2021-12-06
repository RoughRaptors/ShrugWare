using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class EqualizeTwoGroups : Microgame
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
        GameObject groupOfTwoObj;

        [SerializeField]
        GameObject groupOfThreeObj;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;
        private const float DISTANCE_FOR_VALID_STACK = 6.0f;

        private const float PLAYER_MOVE_SPEED = 30.0f;

        private bool stackedEqually = false;

        private float timeRatio = 0;
        private Vector3 meteorStartPos;

        // TODO - put the groups on literal scales to make it clear what the objective is
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

            meteorStartPos = meteorObject.transform.position;

            SetupGroupMembers();

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

                    HandleMicrogameEnd(stackedEqually);
                }
                else
                {
                    timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
                    meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, timeRatio);

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

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);

            groupOfTwoObj.SetActive(true);
            groupOfThreeObj.SetActive(true);
        }

        // 50/50 chance to spawn on left or right
        private void SetupGroupMembers()
        {
            groupOfTwoObj.SetActive(false);
            groupOfThreeObj.SetActive(false);

            if (Random.Range(0, 2) == 0)
            {
                Vector3 tempObjPos = groupOfTwoObj.transform.position;
                groupOfTwoObj.transform.position = groupOfThreeObj.transform.position;
                groupOfThreeObj.transform.position = tempObjPos;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == meteorObject)
            {
                HandleCollision();
            }
            else if(other.gameObject == groupOfTwoObj)
            {
                stackedEqually = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject == groupOfTwoObj)
            {
                stackedEqually = false;
            }
        }

        private void HandleCollision()
        {
            if (stackedEqually)
            {
                stackedEqually = true;
                instructionsText.text = "Equality!";
            }
            else
            {
                instructionsText.text = "Count better";
            }

            instructionsText.gameObject.SetActive(true);

            meteorObject.SetActive(false);
        }
    }
}