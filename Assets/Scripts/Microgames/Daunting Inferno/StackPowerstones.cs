using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class StackPowerstones : Microgame
    {
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

        protected override void Start()
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

            SpawnPowerstones();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += CollectStone;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= CollectStone;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            HandleInput();
        }

        protected override bool VictoryCheck()
        {
            playerObject.transform.GetChild(0).gameObject.SetActive(false);
            bool victory = numPowerstonesCollected == NUM_POWERSTONES_TOTAL;
            if (!victory)
            {
                foreach(GameObject go in powerstones)
                {
                    go.SetActive(false);
                }
            }
            return victory;
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

        private void CollectStone(GameObject stone)
        {
            stone.SetActive(false);
            if (++numPowerstonesCollected == NUM_POWERSTONES_TOTAL)
            {
                SetMicrogameEndText(true);
            }
        }
    }
}