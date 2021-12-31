using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class StealTheChest : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject chestObj = null;

        [SerializeField]
        GameObject groupMember1Obj = null;

        [SerializeField]
        GameObject groupMember2Obj = null;

        [SerializeField]
        GameObject groupMember3Obj = null;

        private Vector2 groupMember1StartPos = new Vector2(-80, -40);
        private Vector2 groupMember2StartPos = new Vector2(0, -25);
        private Vector2 groupMember3StartPos = new Vector2(80, -40);

        private const float X_MIN = -75.0f;
        private const float X_MAX = 75.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 10.0f;

        private const float PLAYER_MOVE_SPEED = 25.0f;
        private const float REQUIRED_DISTANCE_FROM_PLAYER = 45.0f;

        private bool lootStolen = false;

        private int numLootSpawnRetries = 0;

        private float timeRatio = 0;

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

            SetupChest();

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
                    if(!lootStolen)
                    {
                        instructionsText.text = "No loot for you";
                        instructionsText.gameObject.SetActive(true);
                    }

                    HandleMicrogameEnd(lootStolen);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";

                    if (!lootStolen)
                    {
                        MoveGroupMembers();
                        HandleInput();
                    }
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
        }

        private void SetupChest()
        {
            Vector2 spawnPos = new Vector3(0, 0);
            float xPos = 0.0f;
            float yPos = 0.0f;

            // make sure it spawns a certain distance away from the player - if we fail enough times then oh well it doesn't need to be perfect :-D
            while (numLootSpawnRetries < 100)
            {
                xPos = Random.Range(X_MIN, X_MAX);
                yPos = Random.Range(Y_MIN, Y_MAX);
                ++numLootSpawnRetries;

                spawnPos = new Vector3(xPos, yPos);
                if(Vector2.Distance(playerObject.transform.position, spawnPos) > REQUIRED_DISTANCE_FROM_PLAYER)
                {
                    break;
                }
            }

            chestObj.transform.position = spawnPos;
        }

        private void MoveGroupMembers()
        {
            timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;

            groupMember1Obj.transform.position = Vector3.Lerp(groupMember1StartPos, chestObj.transform.position, timeRatio);
            groupMember2Obj.transform.position = Vector3.Lerp(groupMember2StartPos, chestObj.transform.position, timeRatio);
            groupMember3Obj.transform.position = Vector3.Lerp(groupMember3StartPos, chestObj.transform.position, timeRatio);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject == chestObj)
            {
                lootStolen = true;

                instructionsText.text = "NINJAED!";
                instructionsText.gameObject.SetActive(true);

                groupMember1Obj.SetActive(false);
                groupMember2Obj.SetActive(false);
                groupMember3Obj.SetActive(false);
            }
        }
    }
}