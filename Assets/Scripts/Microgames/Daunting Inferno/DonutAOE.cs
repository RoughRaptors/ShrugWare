using UnityEngine;
using System.Collections.Generic;
using System;
using CartoonFX;

namespace ShrugWare
{
    // to make things more fun and change things up. randomly pick two who get chosen for the aura
    public class DonutAOE : Microgame
    {
        [SerializeField]
        GameObject storm;

        [SerializeField]
        GameObject greenAura;

        [SerializeField]
        GameObject redAura;

        private const float STORM_X_MIN = -80.0f;
        private const float STORM_X_MAX = 80.0f;
        private const float STORM_Y_MIN = -25.0f;
        private const float STORM_Y_MAX = 25;
        private const float MIN_STORM_DISTANCE = 30.0f;
        private const float STORM_SPEED_MIN = 25.0f;
        private const float STORM_SPEED_MAX = 40.0f;

        private Vector2 stormTargetPos;
        private float stormSpeed;

        private bool safe = true;

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

            float newXPos = UnityEngine.Random.Range(STORM_X_MIN, STORM_X_MAX);
            float newYPos = UnityEngine.Random.Range(STORM_Y_MIN, STORM_Y_MAX);
            stormTargetPos = new Vector2(newXPos, newYPos);
            stormSpeed = UnityEngine.Random.Range(STORM_SPEED_MIN, STORM_SPEED_MAX);
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!gameOver)
            {
                MoveEye();
            }
        }

        protected override bool VictoryCheck()
        {
            return safe;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Friendly Collider"))
            {
                safe = false;
                SetMicrogameEndText(false);
            }
        }

        private void MoveEye()
        {
            storm.transform.position = Vector2.MoveTowards(storm.transform.position, stormTargetPos, stormSpeed * Time.deltaTime);

            float dist = Vector2.Distance(storm.transform.position, stormTargetPos);
            if (dist <= 1.0f)
            { 
                int numAttempts = 0;
                while (numAttempts++ < 100)
                {
                    float newXPos = UnityEngine.Random.Range(STORM_X_MIN, STORM_X_MAX);
                    float newYPos = UnityEngine.Random.Range(STORM_Y_MIN, STORM_Y_MAX);
                    Vector2 newPos = new Vector2(newXPos, newYPos);
                    if (Vector2.Distance(storm.transform.position, newPos) >= MIN_STORM_DISTANCE)
                    {
                        stormSpeed = UnityEngine.Random.Range(STORM_SPEED_MIN, STORM_SPEED_MAX);
                        stormTargetPos = newPos;
                        return;
                    }
                }
            }
        }
    }
}