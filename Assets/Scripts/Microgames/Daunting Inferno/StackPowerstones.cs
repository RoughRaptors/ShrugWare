using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class StackPowerstones : Microgame
    {
        [SerializeField]
        Text instructionsText = null;
        
        [SerializeField]
        Text timerText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject powerstoneObj = null;

        private const float PLAYER_MOVE_SPEED = 75.0f;

        private const float X_MIN = -65.0f;
        private const float X_MAX = 65.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 14.0f;

        private const float NUM_POWERSTONES_TOTAL = 4;
        private float numPowerstonesCollected = 0;

        private List<GameObject> powerstones = new List<GameObject>();

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

            SpawnPowerstones();

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
                    playerObject.SetActive(false);

                    if (numPowerstonesCollected != NUM_POWERSTONES_TOTAL)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Casual, no kill for you";

                        foreach(GameObject go in powerstones)
                        {
                            go.SetActive(false);
                        }
                    }

                    HandleMicrogameEnd(numPowerstonesCollected == NUM_POWERSTONES_TOTAL);
                }
                else
                {
                    microgameDurationRemaining -= Time.deltaTime;
                    timerText.text = microgameDurationRemaining.ToString("F2") + "s";
                    HandleInput();
                }
            }
        }

        private void SpawnPowerstones()
        {
            for (int i = 0; i < NUM_POWERSTONES_TOTAL; ++i)
            {
                int numTries = 0;

                while (numTries < 100)
                {
                    ++numTries;

                    float xPos = Random.Range(X_MIN, X_MAX);
                    float yPos = Random.Range(Y_MIN, Y_MAX);
                    Vector3 powerstonePos = new Vector3(xPos, yPos, 75.0f);
                    if (Vector3.Distance(playerObject.transform.position, powerstonePos) > 15.0f)
                    {
                        GameObject powerstone = Instantiate(powerstoneObj);
                        powerstone.transform.position = powerstonePos;
                        powerstone.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);

                        powerstones.Add(powerstone);
                        break;
                    }
                }
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
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

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.SetActive(false);

            if (++numPowerstonesCollected == NUM_POWERSTONES_TOTAL)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Oops banned \n#WORTHIT #GOTLOOT";
            }
        }
    }
}