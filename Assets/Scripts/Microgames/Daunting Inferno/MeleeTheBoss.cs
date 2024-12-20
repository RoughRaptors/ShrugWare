using UnityEngine;
using System.Collections.Generic;

namespace ShrugWare
{
    public class MeleeTheBoss : Microgame
    {
        [SerializeField]
        GameObject enemyObj;
        
        [SerializeField]
        List<GameObject> hitVFXList;

        private bool enemyHit = false;

        private float time = 0;

        private const int LEFT_ENEMY_LIMIT = -85;
        private const int RIGHT_ENEMY_LIMIT = 85;
        private Vector3 enemyMovePos = new Vector3(0, 0, 0);
        private Vector3 enemyStartPos = new Vector3(0, 0, 0);

        protected override void Start()
        {
            base.Start();

            enemyObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

            PickEnemyMovePosition();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            enemyObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!enemyHit)
            {
                time += Time.deltaTime / microGameTime;
                enemyObj.transform.position = Vector3.Lerp(enemyStartPos, enemyMovePos, time);
            }
        }

        protected override bool VictoryCheck()
        {
            return enemyHit;
        }

        private void OnTriggerEnter(Collider other)
        {
            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            Instantiate(hitVFXList[index], enemyObj.transform.position, Quaternion.identity);
            enemyObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

            enemyHit = true;
            SetMicrogameEndText(true);
        }

        private void PickEnemyMovePosition()
        {
            // pick left or right and move there
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                enemyMovePos = new Vector2(LEFT_ENEMY_LIMIT, 40);
            }
            else
            {
                enemyMovePos = new Vector2(RIGHT_ENEMY_LIMIT, 40);
            }

            enemyStartPos = enemyObj.transform.position;
        }
    }
}