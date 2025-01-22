using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShrugWare
{
    public class SimonSays : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject bossObject = null;

        [SerializeField]
        List<Vector2> playerPosList;

        [SerializeField]
        List<Vector2> bossPosList;

        [SerializeField]
        GameObject breathObj;

        [SerializeField]
        MicrogameUI microgameUI;

        [SerializeField]
        AudioClipData fireSound;

        private const float HIDE_BOSS_DELAY = .9f;
        private const float HIDE_WARNING_DELAY = 1.5f;
        private const float NEXT_MOVE_TIME_DELAY = 1.125f;
        private const float NEXT_MOVE_AND_BREATHE_TIME_DELAY = 1.9f;
        private const float SEQUENCE_DURATION = 3;

        private bool safe = true;
        private int playerPos = 1; // 0 left, 1 mid, 2 right

        // we need to first pick our movement spots, then display them as bad areas (move the boss to them), then start from the beginning and move him with breath
        private Queue<int> bossPosQueueMove = new Queue<int>();
        private Queue<int> bossPosQueueMoveAndBreathe = new Queue<int>();

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

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            microGameTime = 11.5f;

            ParticleSystem ps = breathObj.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = ps.main;
            SetupBossPositionQueue();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            Invoke("HandleBossMovement", NEXT_MOVE_TIME_DELAY);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            HandlePlayerMovement();
        }

        protected override bool VictoryCheck()
        {
            return safe;
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

        private void HandleBossMovement()
        {
            bossObject.SetActive(true);
            if (bossPosQueueMove.Count > 0 && !gameOver)
            {
                int posIndex = bossPosQueueMove.Dequeue();
                Vector2 nextPos = bossPosList[posIndex];
                bossObject.transform.position = nextPos;

                Invoke("HandleBossMovement", NEXT_MOVE_TIME_DELAY);
            }
            else if(bossPosQueueMove.Count == 0)
            {
                microgameUI.ShowInstructionText("INCOMING!");
                Invoke("HideWarning", HIDE_WARNING_DELAY);
            }
        }

        private void HideWarning()
        {
            bossObject.SetActive(false);
            playerObject.SetActive(true);

            microgameUI.DisableInstructionsText();
            Invoke("HandleBossMovementAndBreath", NEXT_MOVE_TIME_DELAY);
        }

        private void HandleBossMovementAndBreath()
        {
            if (bossPosQueueMoveAndBreathe.Count > 0 && !gameOver)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAudioClip(fireSound);
                }

                bossObject.SetActive(true);
                breathObj.SetActive(true);

                int posIndex = bossPosQueueMoveAndBreathe.Dequeue();
                Vector2 nextPos = bossPosList[posIndex];
                bossObject.transform.position = nextPos;

                ParticleSystem ps = breathObj.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule mainModule = ps.main;
                if (posIndex == 0)
                {
                    mainModule.startColor = Color.blue;
                }
                else if (posIndex == 1)
                {
                    mainModule.startColor = Color.red;
                }
                else if (posIndex == 2)
                {
                    mainModule.startColor = Color.green;
                }

                Invoke("HideBoss", HIDE_BOSS_DELAY);
                Invoke("HandleBossMovementAndBreath", NEXT_MOVE_AND_BREATHE_TIME_DELAY);

                microgameUI.DisableInstructionsText();
            }
        }

        private void HideBoss()
        {
            bossObject.SetActive(false);
        }

        private void BreathHit(GameObject obj)
        {
            safe = false;
            playerObject.SetActive(false);
            SetMicrogameEndText(safe);
        }

        // fill out our queue with the positions, then use the index to get the position out of bossPosList
        // make each movement a new spot, so it's clear where the movement is and it's not unnoticably stationary
        private void SetupBossPositionQueue()
        {
            int prevSpot = -1;
            for (int i = 0; i < SEQUENCE_DURATION; ++i)
            {
                bool newSpot = false;
                while (!newSpot)
                {
                    int bossPos = UnityEngine.Random.Range(0, 3);
                    if (prevSpot != bossPos)
                    {
                        bossPosQueueMove.Enqueue(bossPos);
                        bossPosQueueMoveAndBreathe.Enqueue(bossPos);
                        prevSpot = bossPos;
                        newSpot = true;
                    }
                }
            }
        }
    }
}