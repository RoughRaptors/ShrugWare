using UnityEngine;
using System.Collections.Generic;
using System;
using static UnityEngine.GraphicsBuffer;

namespace ShrugWare
{
    public class TurnTheBoss : Microgame
    {
        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        GameObject ally1;

        [SerializeField]
        GameObject ally2;

        [SerializeField]
        GameObject bloodVFX;

        [SerializeField]
        List<PositionLayout> positionLayoutList = new List<PositionLayout>();

        [SerializeField]
        GameObject breathInitialObj;

        [SerializeField]
        AudioClipData fireSound;

        [Serializable]
        public struct PositionLayout
        {
            public Vector3 playerPos;
            public Vector3 enemyPos;
            public Vector3 ally1Pos;
            public Vector3 ally2Pos;
        }

        private const float BOSS_SPEED_MIN = 30.0f;
        private const float BOSS_SPEED_MAX = 45.0f;
        private float bossSpeed;

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

            microGameTime *= 1.25f;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            SetupPositions();

            Vector3 lookPos = playerObj.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!gameOver)
            {
                Vector2 targetPos;
                if (transform.position.x < playerObj.transform.position.x)
                {
                    targetPos = new Vector2(playerObj.transform.position.x - 15, playerObj.transform.position.y);
                }
                else
                {
                    targetPos = new Vector2(playerObj.transform.position.x + 15, playerObj.transform.position.y);
                }

                transform.position = Vector2.MoveTowards(transform.position, targetPos, bossSpeed * Time.deltaTime);

                Vector3 lookPos = playerObj.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = rotation;
            }
        }

        protected override bool VictoryCheck()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(fireSound);
            }

            GameObject breathObj1 = Instantiate(breathInitialObj, transform);
            bool lookingAtAlly1 = IsLookingAtAlly(ref ally1, ref breathObj1);

            GameObject breathObj2 = Instantiate(breathInitialObj, transform);
            bool lookingAtAlly2 = IsLookingAtAlly(ref ally2, ref breathObj2);

            return !lookingAtAlly1 && !lookingAtAlly2;
        }

        private void SetupPositions()
        {
            int posIndex = UnityEngine.Random.Range(0, positionLayoutList.Count);
            PositionLayout layout = positionLayoutList[posIndex];
            playerObj.transform.position = layout.playerPos;
            transform.position = layout.enemyPos;
            ally1.transform.position = layout.ally1Pos;
            ally2.transform.position = layout.ally2Pos;

            bossSpeed = UnityEngine.Random.Range(BOSS_SPEED_MIN, BOSS_SPEED_MAX);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if(collision.gameObject.tag == "Ally")
            {
                GameObject bloodObj = Instantiate(bloodVFX, collision.gameObject.transform.position, Quaternion.identity);
                bloodObj.SetActive(true);

                SetMicrogameEndText(false);
                collision.gameObject.SetActive(false);
            }
        }

        private bool IsLookingAtAlly(ref GameObject ally, ref GameObject breathObj)
        {
            float dot = Vector3.Dot(transform.forward, (ally.transform.position - transform.position).normalized);
            Debug.Log(ally.name + " " + dot);
            if ((dot >= 0.85f || Mathf.Abs(dot) <= 0.1f) && dot != 0)
            {
                // draw a breath of fire from the boss to the ally
                Vector3 target = breathObj.transform.position;
                target.z = 0f;

                Vector3 objectPos = ally.transform.position;
                target.x -= objectPos.x;
                target.y -= objectPos.y;

                float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
                breathObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
                breathObj.SetActive(true);

                ally.gameObject.SetActive(false);
                GameObject bloodObj = Instantiate(bloodVFX, ally.gameObject.transform.position, Quaternion.identity);
                bloodObj.SetActive(true);

                return true;
            }

            return false;
        }
    }
}