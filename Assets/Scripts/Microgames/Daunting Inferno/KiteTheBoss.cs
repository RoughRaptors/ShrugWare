using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ShrugWare
{
    public class KiteTheBoss : Microgame
    {
        [SerializeField]
        GameObject enemyObj;
        
        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        GameObject playerObj;

        private bool hit = false;

        private const float BOSS_SPEED_MIN = 15.0f;
        private const float BOSS_SPEED_MAX = 27.5f;
        private float bossSpeed;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnBadCollision += BossHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnBadCollision -= BossHit;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            bossSpeed = UnityEngine.Random.Range(BOSS_SPEED_MIN, BOSS_SPEED_MAX);
            Debug.Log(bossSpeed);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            enemyObj.transform.position = Vector3.MoveTowards(enemyObj.transform.position, playerObj.transform.position, bossSpeed * Time.deltaTime);
        }

        protected override bool VictoryCheck()
        {
            return !hit;
        }

        private void BossHit(GameObject obj)
        {
            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            GameObject bloodObj = Instantiate(hitVFXList[index], playerObj.transform.position, Quaternion.identity);
            bloodObj.SetActive(true);
            playerObj.SetActive(false);

            hit = true;
            SetMicrogameEndText(false);
        }
    }
}