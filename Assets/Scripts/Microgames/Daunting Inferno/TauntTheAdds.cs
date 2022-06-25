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
        GameObject enemy1 = null;

        [SerializeField]
        GameObject enemy2 = null;

        bool won = false;

        private const float ENEMY_MOVE_SPEED = 1.5f;
        
        private const float X_MIN = -3.20f;
        private const float X_MAX = 3.20f;
        private const float Y_MIN = 0.70f;
        private const float Y_MAX = 1.70f;

        private bool enemy1Taunted = false;
        private bool enemy2Taunted = false;

        private Vector3 enemy1TargetPos;
        private Vector3 enemy2TargetPos;

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

            SetupEnemies();

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
                    // move towards the target point
                    enemy1.transform.position = Vector3.MoveTowards(enemy1.transform.position, enemy1TargetPos, ENEMY_MOVE_SPEED * Time.deltaTime);

                    enemy2.transform.position = Vector3.MoveTowards(enemy2.transform.position, enemy2TargetPos, ENEMY_MOVE_SPEED * Time.deltaTime);

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                    HandleInput();
                }
            }
        }

        private void SetupEnemies()
        {
            // try 100 times to get a far enough position away from its spawn
            int numTries = 0;

            // enemy 1
            while (numTries < 100)
            {
                ++numTries;

                float enemy1XPos = Random.Range(X_MIN, X_MAX);
                float enemy1YPos = Random.Range(Y_MIN, Y_MAX);
                Vector3 enemy1Pos = new Vector3(enemy1XPos, enemy1YPos, 4.0f);

                float enemy1TargetXPos = Random.Range(X_MIN, X_MAX);
                float enemy1TargetYPos = Random.Range(Y_MIN, Y_MAX);
                enemy1TargetPos = new Vector3(enemy1TargetXPos, enemy1TargetYPos, 4.0f);

                if (Vector3.Distance(enemy1Pos, enemy1TargetPos) < 3.0f)
                {
                    enemy1.transform.position = enemy1Pos;
                    break;
                }
            }

            // enemy 2
            numTries = 0;
            while (numTries < 100)
            {
                ++numTries;

                float enemy2XPos = Random.Range(X_MIN, X_MAX);
                float enemy2YPos = Random.Range(Y_MIN, Y_MAX);
                Vector3 enemy2Pos = new Vector3(enemy2XPos, enemy2YPos, 4.0f);
                enemy2.transform.position = enemy2Pos;

                float enemy2TargetXPos = Random.Range(X_MIN, X_MAX);
                float enemy2TargetYPos = Random.Range(Y_MIN, Y_MAX);
                enemy2TargetPos = new Vector3(enemy2TargetXPos, enemy2TargetYPos, 4.0f);

                if (Vector3.Distance(enemy2Pos, enemy2TargetPos) > 3.0f)
                {
                    enemy2.transform.position = enemy2Pos;
                    break;
                }
            }

            enemy1.SetActive(false);
            enemy2.SetActive(false);
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Select stage    
                    if (hit.transform.gameObject == enemy1)
                    {
                        enemy1Taunted = true;
                        //enemy1.GetComponent<MeshRenderer>().material.color = Color.green;
                        enemy1.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
                    }
                    else if(hit.transform.gameObject == enemy2)
                    {
                        enemy2Taunted = true;
                        //enemy2.GetComponent<MeshRenderer>().material.color = Color.green;
                        enemy2.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
                    }
                }
               
                if(enemy1Taunted && enemy2Taunted)
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
            enemy1.SetActive(true);
            enemy2.SetActive(true);
        }
    }
}