using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShrugWare
{
    public class DragToPoison : Microgame
    {
        [SerializeField]
        GameObject enemyObj;

        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        List<PositionLayout> positionLayoutList = new List<PositionLayout>();

        [SerializeField]
        GameObject poisonObj;

        [SerializeField]
        List<Vector2> poisonPosList = new List<Vector2>();

        [SerializeField]
        GameObject enemyDeathVFX;

        [SerializeField]
        GameObject ally1;

        [SerializeField]
        GameObject ally2;

        [SerializeField]
        GameObject bloodVFX;

        [Serializable]
        public struct PositionLayout
        {
            public Vector2 playerPos;
            public Vector2 enemyPos;
        }

        private bool bossPoisoned = false;

        private Vector2 targetAlly1Pos = new Vector2();
        private Vector2 targetAlly2Pos = new Vector2();

        private const float ALLY_MIN_X = -80.0f;
        private const float ALLY_MAX_X = 80.0f;
        private const float ALLY_MIN_Y = -30.0f;
        private const float ALLY_MAX_Y = 30.0f;

        private const float BOSS_SPEED_MIN = 30.0f;
        private const float BOSS_SPEED_MAX = 45.0f;
        private float bossSpeed;

        // our allies can spawn right by the boss, this isn't really a big deal as positioning can be whatever. make them immune for a bit at the start
        private const float ALLY_INVULN_TIME = 1.25f;
        private const float ALLY_SPEED_MIN = 15.0f;
        private const float ALLY_SPEED_MAX = 30.0f;
        private float allySpeed;

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

            SetupPositions();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            microGameTime *= 1.25f;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!bossPoisoned && !gameOver)
            {
                enemyObj.transform.position = Vector2.MoveTowards(enemyObj.transform.position, playerObj.transform.position, bossSpeed * Time.deltaTime);
                ally1.transform.position = Vector2.MoveTowards(ally1.transform.position, targetAlly1Pos, allySpeed * Time.deltaTime);
                ally2.transform.position = Vector2.MoveTowards(ally2.transform.position, targetAlly2Pos, allySpeed * Time.deltaTime);
            }
        }

        protected override bool VictoryCheck()
        {
            return bossPoisoned;
        }

        private void SetupPositions()
        {
            int posIndex = UnityEngine.Random.Range(0, positionLayoutList.Count);
            PositionLayout layout = positionLayoutList[posIndex];
            playerObj.transform.position = layout.playerPos;
            enemyObj.transform.position = layout.enemyPos;

            int poisonIndex = UnityEngine.Random.Range(0, poisonPosList.Count);
            poisonObj.transform.position = poisonPosList[poisonIndex];

            bossSpeed = UnityEngine.Random.Range(BOSS_SPEED_MIN, BOSS_SPEED_MAX);
            allySpeed = UnityEngine.Random.Range(ALLY_SPEED_MIN, ALLY_SPEED_MAX);

            SetupAllies();
        }

        private void SetupAllies()
        {
            float xPos = UnityEngine.Random.Range(ALLY_MIN_X, ALLY_MAX_X);
            float yPos = UnityEngine.Random.Range(ALLY_MIN_Y, ALLY_MAX_Y);
            ally1.transform.position = new Vector2(xPos, yPos);

            float targetXPos = UnityEngine.Random.Range(ALLY_MIN_X, ALLY_MAX_X);
            float targetYPos = UnityEngine.Random.Range(ALLY_MIN_Y, ALLY_MAX_Y);
            targetAlly1Pos = new Vector2(targetXPos, targetYPos);

            xPos = UnityEngine.Random.Range(ALLY_MIN_X, ALLY_MAX_X);
            yPos = UnityEngine.Random.Range(ALLY_MIN_Y, ALLY_MAX_Y);
            ally2.transform.position = new Vector2(xPos, yPos);

            targetXPos = UnityEngine.Random.Range(ALLY_MIN_X, ALLY_MAX_X);
            targetYPos = UnityEngine.Random.Range(ALLY_MIN_Y, ALLY_MAX_Y);
            targetAlly2Pos = new Vector2(targetXPos, targetYPos);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Field")
            {
                GameObject deathObj = Instantiate(enemyDeathVFX, this.transform.position, Quaternion.identity);
                deathObj.transform.localScale = new Vector2(10, 10);
                this.transform.position = new Vector2(100, 100);

                bossPoisoned = true;
                SetMicrogameEndText(bossPoisoned);
            }
            else if(collision.tag == "Ally" && timeElapsed >= ALLY_INVULN_TIME)
            {
                gameOver = true;

                GameObject bloodObj = Instantiate(bloodVFX, collision.gameObject.transform.position, Quaternion.identity);
                bloodObj.SetActive(true);
                collision.gameObject.SetActive(false);
                SetMicrogameEndText(false);
            }
        }
    }
}