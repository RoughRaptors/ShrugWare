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
        Text timerText = null;

        private Enemy enemy1;
        private Enemy enemy2;
        bool won = false;

        private const float ENEMY_MOVE_SPEED = 10f;

        private const float X_MIN = -55.0f;
        private const float X_MAX = 55.0f;
        private const float Y_MIN = -38.0f;
        private const float Y_MAX = 33;

        private struct Enemy
        {
            public bool taunted;
            public GameObject enemyObj;
        }

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

            SetupEnemies();

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

                HandleInput();

                if (microgameDurationRemaining <= 0.0f)
                {
                    // out of time
                    HandleMicrogameEnd(won);
                }
                else
                {
                    // move towards the middle - arbitrary point
                    enemy1.enemyObj.transform.position = 
                        Vector3.MoveTowards(enemy1.enemyObj.transform.position,
                        new Vector3(0, 0, 100), ENEMY_MOVE_SPEED * Time.deltaTime);

                    enemy2.enemyObj.transform.position =
                        Vector3.MoveTowards(enemy2.enemyObj.transform.position, 
                        new Vector3(0, 0, 100), ENEMY_MOVE_SPEED * Time.deltaTime);
                }
            }
        }

        private void SetupEnemies()
        {
            float enemy1XPos = Random.Range(X_MIN, X_MAX);
            float enemy1YPos = Random.Range(X_MIN, X_MAX);
            Vector3 enemy1Pos = new Vector3(enemy1XPos, enemy1YPos, 100.0f);
            enemy1.enemyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy1.enemyObj.transform.position = enemy1Pos;
            enemy1.enemyObj.transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
            enemy1.enemyObj.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            enemy1.taunted = false;

            float enemy2XPos = Random.Range(Y_MIN, Y_MAX);
            float enemy2YPos = Random.Range(Y_MIN, Y_MAX);
            Vector3 enemy2Pos = new Vector3(enemy2XPos, enemy2YPos, 100.0f);
            enemy2.enemyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemy2.enemyObj.transform.position = enemy2Pos;
            enemy2.enemyObj.transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
            enemy2.enemyObj.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            enemy2.taunted = false;
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
                    if (hit.transform.gameObject == enemy1.enemyObj)
                    {
                        enemy1.taunted = true;
                        enemy1.enemyObj.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                    }
                    else if(hit.transform.gameObject == enemy2.enemyObj)
                    {
                        enemy2.taunted = true;
                        enemy2.enemyObj.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                    }
                }
               
                if(enemy1.taunted && enemy2.taunted)
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
        }
    }
}