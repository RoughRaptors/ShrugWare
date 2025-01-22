using System.Collections;
using UnityEngine;

namespace ShrugWare
{
    public class FollowTheRaidLag : Microgame
    {
        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        GameObject raidGroupObj;

        private const float RAID_X_MIN = -65.0f;
        private const float RAID_X_MAX = 95.0f;
        private const float RAID_Y_MIN = -40.0f;
        private const float RAID_Y_MAX = 35.0f;

        private const float MIN_RAID_SPEED = 40.0f;
        private const float MAX_RAID_SPEED = 70.0f;

        private float raidSpeed = 60.0f;
        private Vector2 nextMovementTarget;
        private bool isWithRaid = false;
        private float initialTimescale = 1.0f;

        // each of these chances are going to be calculated on tick and their respective percentage is their chance (percentages are 1-1)
        private const float CHANCE_TO_FREEZE = 1.5f;
        private const float MIN_FREEZE_TIME = 0.07f;
        private const float MAX_FREEZE_TIME = 0.9f;

        private const float CHANCE_TO_CHANGE_TIMESCALE = 2.0f;
        private const float MIN_TIMESCALE_MULTIPLIER = 0.25f;
        private const float MAX_TIMESCALE_MULTIPLIER = 2.75f;
        private const float TIMESCALE_CHANGE_DURATION_MIN = 0.25f;
        private const float TIMESCALE_CHANGE_DURATION_MAX = 0.75f;

        private Vector2 rubberBandReversePos; // the position we return to. we save this off when we trigger the chance
        private const float CHANCE_TO_RUBBER_BAND = 1.0f;
        private const float MIN_RUBBER_BAND_DELAY_TIME = 0.25f;
        private const float MAX_RUBBER_BAND_DELAY_TIME = 0.75f;

        // don't do two things at once
        float timeToNextAcceptableLagEvent = float.MinValue;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnGoodCollision += RaidEntered;
            PlayerCollider.OnGoodExit += RaidLeft;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnGoodCollision -= RaidEntered;
            PlayerCollider.OnGoodExit -= RaidLeft;
        }

        protected override void Awake()
        {
            base.Awake();

            microGameTime *= 1.2f;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            if (BossGameManager.Instance != null)
            {
                initialTimescale = BossGameManager.Instance.GetCurTimeScale();
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            HandleRaidMovement();
            
            if (Time.time >= timeToNextAcceptableLagEvent)
            {
                HandleChanceToFreeze();
                HandleChanceToChangeTimescale();
                HandleChanceToRubberBand();
            }
        }

        protected override bool VictoryCheck()
        {
            // set this back once we're done
            if (BossGameManager.Instance != null)
            {
                Time.timeScale = BossGameManager.Instance.GetCurTimeScale();
            }

            return isWithRaid;
        }

        private void RaidEntered(GameObject raidObject)
        {
            isWithRaid = true;
        }

        private void RaidLeft(GameObject raidObject)
        {
            isWithRaid = false;
        }

        private void HandleRaidMovement()
        {
            if(Vector2.Distance(raidGroupObj.transform.position, nextMovementTarget) < 0.5f)
            {
                float newXPos = UnityEngine.Random.Range(RAID_X_MIN, RAID_X_MAX);
                float newYPos = UnityEngine.Random.Range(RAID_Y_MIN, RAID_Y_MAX);
                nextMovementTarget = new Vector2(newXPos, newYPos);

                raidSpeed = UnityEngine.Random.Range(MIN_RAID_SPEED, MAX_RAID_SPEED);
            }

            raidGroupObj.transform.position = Vector3.MoveTowards(raidGroupObj.transform.position, nextMovementTarget, raidSpeed * Time.deltaTime);
        }

        // completely freeze the game
        private void HandleChanceToFreeze()
        {
            float roll = UnityEngine.Random.Range(0, 100);
            if(roll < CHANCE_TO_FREEZE)
            {
                //Time.timeScale = 0.0f;
                playerObj.GetComponent<PlayerMover>().DisableMovement();

                float freezeTime = UnityEngine.Random.Range(MIN_FREEZE_TIME, MAX_FREEZE_TIME);
                timeToNextAcceptableLagEvent = Time.time + freezeTime;
                StartCoroutine(ResetTimescale(freezeTime));
            }
        }

        // change the timescale
        private void HandleChanceToChangeTimescale()
        {
            float roll = UnityEngine.Random.Range(0, 100);
            if (roll < CHANCE_TO_CHANGE_TIMESCALE)
            {
                float timescaleMultiplier = UnityEngine.Random.Range(MIN_TIMESCALE_MULTIPLIER, MAX_TIMESCALE_MULTIPLIER);
                float newTempTimescale = initialTimescale * timescaleMultiplier;
                Time.timeScale = newTempTimescale;

                float timescaleChangeDuration = UnityEngine.Random.Range(TIMESCALE_CHANGE_DURATION_MIN, TIMESCALE_CHANGE_DURATION_MAX);
                timeToNextAcceptableLagEvent = Time.time + timescaleChangeDuration;
                StartCoroutine(ResetTimescale(timescaleChangeDuration));
            }
        }

        // store the position and in a delay, move the player back to there
        private void HandleChanceToRubberBand()
        {
            float roll = UnityEngine.Random.Range(0, 100);
            if (roll < CHANCE_TO_RUBBER_BAND)
            {
                rubberBandReversePos = playerObj.transform.position;
                float rubberBandDelay = UnityEngine.Random.Range(MIN_RUBBER_BAND_DELAY_TIME, MAX_RUBBER_BAND_DELAY_TIME);
                timeToNextAcceptableLagEvent = Time.time + rubberBandDelay;
                StartCoroutine(RubberBand(rubberBandDelay));
            }
        }

        private IEnumerator RubberBand(float rubberBandDelay)
        {
            yield return new WaitForSecondsRealtime(rubberBandDelay);

            playerObj.transform.position = rubberBandReversePos;
        }

        IEnumerator ResetTimescale(float freezeTime)
        {
            yield return new WaitForSecondsRealtime(freezeTime);

            Time.timeScale = initialTimescale;
            playerObj.GetComponent<PlayerMover>().EnableMovement();
        }
    }
}