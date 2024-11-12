using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.Animations.Rigging;

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

        [SerializeField]
        List<PositionLayout> positionLayoutList = new List<PositionLayout>();

        [Serializable]
        public struct PositionLayout
        {
            public Vector2 playerPos;
            public Vector2 enemyPos;
        }

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

        protected override void Awake()
        {
            base.Awake();

            SetupPositions();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
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

        private void SetupPositions()
        {
            int posIndex = UnityEngine.Random.Range(0, positionLayoutList.Count);
            PositionLayout layout = positionLayoutList[posIndex];
            playerObj.transform.position = layout.playerPos;
            enemyObj.transform.position = layout.enemyPos;

            // the "sideways" oriented setups put you and the boss at a longer distance
            bossSpeed = UnityEngine.Random.Range(BOSS_SPEED_MIN, BOSS_SPEED_MAX);
            if (posIndex == 1 || posIndex == 3)
            {
                bossSpeed *= 2.25f;
            }
        }
    }
}