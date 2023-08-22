using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class TauntTheAdds : Microgame
    {
        [SerializeField]
        List<Clickable> enemies = new List<Clickable>();
        Dictionary<Clickable, Vector3> enemyTargetPositions = new Dictionary<Clickable, Vector3>();

        [SerializeField]
        GameObject portalObj;

        private const float ENEMY_MOVE_SPEED = 1.5f;
        private const float X_MIN = -3.20f;
        private const float X_MAX = 3.20f;
        private const float Y_MIN = 0.70f;
        private const float Y_MAX = 1.70f;

        protected override void Start()
        {
            base.Start();

            for(int ii = 0; ii < enemies.Count; ii++)
            {
                SetupEnemy(ii);
            }
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            foreach(Clickable enemy in enemies)
            {
                enemy.Clicked += Taunted;
                enemy.gameObject.SetActive(true);
                Instantiate(portalObj);
                portalObj.transform.position = enemy.transform.position;
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            foreach(Clickable enemy in enemies)
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemyTargetPositions[enemy], ENEMY_MOVE_SPEED * Time.deltaTime);
            }
        }

        protected override bool VictoryCheck() => enemies.Count == 0;

        private void SetupEnemy(int index)
        {
            // try 100 times to get a far enough position away from its spawn
            int numTries = 0;
            Clickable enemy = enemies[index];

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

                enemy.gameObject.SetActive(false);
            }
        }

        private void Taunted(Clickable enemy)
        {
            enemies.Remove(enemy);
            enemy.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
            enemy.Clicked -= Taunted;
            
            if(!VictoryCheck()) return;
            SetMicrogameEndText(true);
        }
    }
}