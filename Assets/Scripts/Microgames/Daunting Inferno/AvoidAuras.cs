using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShrugWare
{
    // to make things more fun and change things up. randomly pick two who needs to line of sight
    public class AvoidAuras : Microgame
    {
        [SerializeField]
        GameObject ally1Obj;

        [SerializeField]
        GameObject ally2Obj;

        [SerializeField]
        GameObject ally3Obj;

        [SerializeField]
        GameObject tankObj;

        [SerializeField]
        GameObject bossObj;

        [SerializeField]
        GameObject bloodVFX;

        [SerializeField]
        List<ListWrapper> allyPositionLayoutListWrapper = new List<ListWrapper>();

        [SerializeField]
        List<Vector2> tankPositions = new List<Vector2>();

        [SerializeField]
        List<Vector2> bossPositions = new List<Vector2>();

        [SerializeField]
        GameObject playerAura;

        [SerializeField]
        GameObject allyAura;

        [SerializeField]
        List<ListWrapper> targetPosSequenceList = new List<ListWrapper>();

        [SerializeField]
        List<Vector2> playerSpawnPosList = new List<Vector2>();

        private int layoutIndex;
        private GameObject chosenAlly;
        private Vector2 allyTargetPos;

        // also treat as invulm
        private const float AURA_EXPAND_DELAY = 1.25f;

        [Serializable]
        public class ListWrapper
        {
            public List<Vector2> positionList;
        }

        private bool collided = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            microGameTime *= 1.25f;
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            SetupPositions();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!gameOver)
            {
                if(timeElapsed >= AURA_EXPAND_DELAY)
                {
                    float newXScale = playerAura.transform.localScale.x + Time.deltaTime;
                    float newYScale = playerAura.transform.localScale.y + Time.deltaTime;
                    float newZScale = playerAura.transform.localScale.z + Time.deltaTime;

                    playerAura.transform.localScale = new Vector3(newXScale, newYScale, newZScale);
                    allyAura.transform.localScale = new Vector3(newXScale, newYScale, newZScale);
                }

                if (targetPosSequenceList[layoutIndex].positionList.Count > 0 && Vector2.Distance(chosenAlly.transform.position, allyTargetPos) < 1.0f)
                {
                    allyTargetPos = targetPosSequenceList[layoutIndex].positionList[0];
                    targetPosSequenceList[layoutIndex].positionList.RemoveAt(0);
                }

                chosenAlly.transform.position = Vector3.MoveTowards(chosenAlly.transform.position, allyTargetPos, 45.0f * Time.deltaTime);
            }
        }

        protected override bool VictoryCheck()
        {
            return !collided;
        }

        private void SetupPositions()
        {
            SetupTankAndBoss();

            layoutIndex = UnityEngine.Random.Range(0, allyPositionLayoutListWrapper.Count);
            List<Vector2> posList = allyPositionLayoutListWrapper[layoutIndex].positionList;

            SetupAlly(ref ally1Obj, ref posList);
            SetupAlly(ref ally2Obj, ref posList);
            SetupAlly(ref ally3Obj, ref posList);

            allyTargetPos = targetPosSequenceList[layoutIndex].positionList[0];
            transform.position = playerSpawnPosList[layoutIndex];
            targetPosSequenceList[layoutIndex].positionList.RemoveAt(0);

            int allyToDebuff = UnityEngine.Random.Range(1, 3);
            if(allyToDebuff == 1)
            {
                chosenAlly = ally1Obj;
                allyAura.transform.parent = ally1Obj.transform;
            }
            else if (allyToDebuff == 2)
            {
                chosenAlly = ally2Obj;
                allyAura.transform.parent = ally2Obj.transform;
            }

            allyAura.transform.position = chosenAlly.transform.position;
            allyAura.SetActive(true);
        }

        private void SetupTankAndBoss()
        {
            int tankAndBossIndex = UnityEngine.Random.Range(0, tankPositions.Count);
            tankObj.transform.position = tankPositions[tankAndBossIndex];
            bossObj.transform.position = bossPositions[tankAndBossIndex];
        }

        private void SetupAlly(ref GameObject goToModify, ref List<Vector2> posList)
        {
            int posIndex = UnityEngine.Random.Range(0, posList.Count);
            Vector2 spawnPos = posList[posIndex];
            goToModify.transform.position = spawnPos;
            posList.RemoveAt(posIndex);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Ally" && timeElapsed >= AURA_EXPAND_DELAY)
            {
                GameObject bloodObj = Instantiate(bloodVFX, collision.gameObject.transform.position, Quaternion.identity);
                bloodObj.SetActive(true);
                collision.gameObject.SetActive(false);

                collided = false;
                SetMicrogameEndText(false);
            }
        }
    }
}