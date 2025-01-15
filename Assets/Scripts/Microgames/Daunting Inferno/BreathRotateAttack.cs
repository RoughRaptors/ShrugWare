using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShrugWare
{
    public class BreathRotateAttack : Microgame
    {
        [Serializable]
        public struct BreathAttack
        {
            public Quaternion breathRot;
            public Quaternion bossRot;
        } 

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject bossObject = null;

        [SerializeField]
        List<Vector2> playerPosList;

        [SerializeField]
        GameObject breathObj;

        [SerializeField]
        List<BreathAttack> breathAttacks = new List<BreathAttack>();

        [SerializeField]
        AudioClipData breathSound;

        private const float ROTATE_TIME = 1.00f;
        private const float ROTATE_SPEED = 5.0f;

        private bool safe = true;
        private Quaternion targetRotation;
        private int playerPos = 1; // 0 left, 1 mid, 2 right

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnBadCollision += BreathHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnBadCollision -= BreathHit;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            bossObject.SetActive(true);
            RotateBoss();
            Invoke("RotateBoss", ROTATE_TIME);
            Invoke("Breathe", microGameTime - 0.5f);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!gameOver)
            {
                if (bossObject.transform.rotation != targetRotation)
                {
                    bossObject.transform.rotation = Quaternion.Slerp(bossObject.transform.rotation, targetRotation, ROTATE_SPEED * Time.deltaTime);
                }

                HandlePlayerMovement();
            }
        }

        protected override bool VictoryCheck()
        {
            return safe;
        }

        private void Breathe()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(breathSound);
            }

            breathObj.SetActive(true);
        }

        private void HandlePlayerMovement()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (playerPos == 0)
                {
                    // nothing
                }
                else if (playerPos == 1)
                {
                    // move left
                    playerPos = 0;
                }
                else if (playerPos == 2)
                {
                    playerPos = 1;
                }

                playerObject.transform.position = playerPosList[playerPos];
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (playerPos == 0)
                {
                    playerPos = 1;

                }
                else if (playerPos == 1)
                {
                    playerPos = 2;

                }
                else if (playerPos == 2)
                {
                    // nothing
                }

                playerObject.transform.position = playerPosList[playerPos];
            }
        }

        private void RotateBoss()
        {
            if(gameOver || timeLeft < ROTATE_TIME)
            {
                return;
            }

            int direction = UnityEngine.Random.Range(0, 3);
            if(direction == 0)
            {
                targetRotation = breathAttacks[0].bossRot;
                breathObj.transform.rotation = breathAttacks[0].breathRot;
            }
            else if (direction == 1)
            {
                targetRotation = breathAttacks[1].bossRot;
                breathObj.transform.rotation = breathAttacks[1].breathRot;
            }
            else if(direction == 2)
            {
                targetRotation = breathAttacks[2].bossRot;
                breathObj.transform.rotation = breathAttacks[2].breathRot;
            }

            Invoke("RotateBoss", ROTATE_TIME);
        }

        private void BreathHit(GameObject obj)
        {
            safe = false;
            SetMicrogameEndText(false);
        }
    }
}