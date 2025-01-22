using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShrugWare
{
    public class ManyWhelps : Microgame
    {
        [SerializeField]
        GameObject whelpInitialObj;

        [SerializeField]
        AudioClipData tauntAudio;

        [SerializeField]
        List<GameObject> whelps;

        private List<Vector2> enemyTargetPositions = new List<Vector2>();

        private const float ENEMY_MOVE_SPEED = 15.0f;
        private const float MIN_SPAWN_DISTANCE = 20.0f;
        private const float X_MIN = -90.0f;
        private const float X_MAX = 90.0f;
        private const float Y_MIN = -35;
        private const float Y_MAX = 20;

        protected override void Awake()
        {
            base.Awake();

            microGameTime *= 1.25f;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            for(int i = 0; i < whelps.Count; ++i)
            {
                GameObject whelp = whelps[i];
                SetupEnemy(ref whelp);
            }

            foreach (GameObject enemy in whelps)
            {
                enemy.GetComponent<Clickable>().Clicked += Taunted;
                enemy.gameObject.SetActive(true);
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            MoveEnemies();
        }

        protected override bool VictoryCheck()
        {
            bool allTaunted = true;
            foreach(GameObject obj in whelps)
            {
                if(obj.activeInHierarchy)
                {
                    allTaunted = false;
                    break;
                }
            }

            return allTaunted;
        }

        private void SetupEnemy(ref GameObject enemy)
        {
            // try 100 times to get a far enough position away from its spawn
            int numTries = 0;

            // enemy 1
            while (numTries < 100)
            {
                ++numTries;

                float enemyXPos = Random.Range(X_MIN, X_MAX);
                float enemyYPos = Random.Range(Y_MIN, Y_MAX);
                Vector2 enemyPos = new Vector2(enemyXPos, enemyYPos);

                float enemyTargetXPos = Random.Range(X_MIN, X_MAX);
                float enemyTargetYPos = Random.Range(Y_MIN, Y_MAX);
                Vector2 targetPos = new Vector2(enemyTargetXPos, enemyTargetYPos);

                if (Vector2.Distance(enemyPos, targetPos) > MIN_SPAWN_DISTANCE)
                {
                    //GameObject enemy = Instantiate(whelpInitialObj, targetPos, Quaternion.identity);
                    //GameObject enemy = whelps[index];
                    enemy.GetComponent<Clickable>().Clicked += Taunted;
                    //enemy.transform.parent = whelpParent.transform;
                    enemy.transform.position = enemyPos;
                    //enemy.name = "Whelp " + index.ToString();
                    enemy.SetActive(true);

                    //enemies.Add(enemy);
                    enemyTargetPositions.Add(targetPos);

                    break;
                }
            }
        }

        private void MoveEnemies()
        {
            for (int i = 0; i < whelps.Count; ++i)
            {
                GameObject enemy = whelps[i];
                enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemyTargetPositions[i], ENEMY_MOVE_SPEED * Time.deltaTime);

                enemy.GetComponentInChildren<Transform>().LookAt(enemyTargetPositions[i]);
            }
        }

        private void Taunted(Clickable enemy)
        {
            if(AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(tauntAudio);
            }

            enemy.gameObject.SetActive(false);

            if (VictoryCheck())
            {
                SetMicrogameEndText(true);
            }
        }
    }
}