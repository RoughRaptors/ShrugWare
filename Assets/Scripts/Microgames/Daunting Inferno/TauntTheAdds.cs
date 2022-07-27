using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class TauntTheAdds : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject[] enemies = new GameObject[0];
        Dictionary<GameObject, Vector3> enemyTargetPositions = new Dictionary<GameObject, Vector3>();
        Dictionary<GameObject, bool> enemiesTaunted = new Dictionary<GameObject, bool>();

        bool won = false;

        private const float ENEMY_MOVE_SPEED = 1.5f;
        
        private const float X_MIN = -3.20f;
        private const float X_MAX = 3.20f;
        private const float Y_MIN = 0.70f;
        private const float Y_MAX = 1.70f;

        protected override void Awake()
        {
            base.Awake();

            for(int ii = 0; ii < enemies.Length; ii++)
            {
                SetupEnemy(ii);
            }
        }

        protected override void Start()
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
                    if (!won)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "RIP healers";
                    }

                    HandleMicrogameEnd(won);
                }
                else
                {
                    foreach(GameObject enemy in enemies)
                    {
                        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemyTargetPositions[enemy], ENEMY_MOVE_SPEED * Time.deltaTime);
                    }

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                    HandleInput();
                }
            }
        }

        private void SetupEnemy(int index)
        {
            // try 100 times to get a far enough position away from its spawn
            int numTries = 0;
            GameObject enemy = enemies[index];
            enemiesTaunted.Add(enemy, false);

            // enemy 1
            while (numTries < 100)
            {
                ++numTries;

                float enemyXPos = Random.Range(X_MIN, X_MAX);
                float enemyYPos = Random.Range(Y_MIN, Y_MAX);
                Vector3 enemyPos = new Vector3(enemyXPos, enemyYPos, 4.0f);

                float enemyTargetXPos = Random.Range(X_MIN, X_MAX);
                float enemyTargetYPos = Random.Range(Y_MIN, Y_MAX);
                Vector3 targetPos = new Vector3(enemyTargetXPos, enemyTargetYPos, 4.0f);

                if (Vector3.Distance(enemyPos, targetPos) < 3.0f)
                {
                    enemy.transform.position = enemyPos;
                    enemyTargetPositions.Add(enemy, targetPos);
                    break;
                }

                enemy.SetActive(false);
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    foreach(GameObject enemy in enemies)
                    {
                        if(hit.transform.gameObject == enemy)
                        {
                            enemiesTaunted[enemy] = true;
                            enemy.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
                        }
                    }
                }
               
                if(!enemiesTaunted.ContainsValue(false))
                {
                    won = true;

                    instructionsText.gameObject.SetActive(true);
                    instructionsText.text = "Healer Says Thanks";
                }
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
            foreach(GameObject enemy in enemies)
                enemy.SetActive(true);
        }
    }
}