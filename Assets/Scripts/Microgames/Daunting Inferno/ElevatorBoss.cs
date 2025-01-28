using System.Collections;
using UnityEngine;

namespace ShrugWare
{
    public class ElevatorBoss : Microgame
    {
        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        GameObject raidGroupObj;

        [SerializeField]
        GameObject elevatorParent;

        private bool onElevator = false;

        private const float PLAYER_SPAWN1_X = -70.0f;
        private const float PLAYER_SPAWN2_X = 70.0f;

        private const float RAID_SPAWN_POINT1_X = -5.0f;
        private const float RAID_SPAWN_POINT2_X = 15.0f;

        private float time = 0;
        private Vector3 elevatorMovePos = new Vector2(0, 60);
        private Vector3 elevatorStartPos = new Vector2(0, -30);

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnGoodCollision += ElevatorEntered;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnGoodCollision -= ElevatorEntered;
        }

        protected override void Awake()
        {
            base.Awake();

            SpawnPlayer();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            elevatorParent.SetActive(true);
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                raidGroupObj.transform.position = new Vector2(RAID_SPAWN_POINT1_X, -25);
            }
            else
            {

                raidGroupObj.transform.position = new Vector2(RAID_SPAWN_POINT2_X, -25);
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            time += Time.deltaTime / microGameTime;
            elevatorParent.transform.position = Vector3.Lerp(elevatorStartPos, elevatorMovePos, time);
        }

        protected override bool VictoryCheck()
        {
            return onElevator;
        }

        private void SpawnPlayer()
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                playerObj.transform.position = new Vector2(PLAYER_SPAWN1_X, 0);
            }
            else
            {
                playerObj.transform.position = new Vector2(PLAYER_SPAWN2_X, 0);
            }

            foreach (SpriteRenderer renderer in playerObj.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.enabled = true;
            }
        }

        private void ElevatorEntered(GameObject go)
        {
            onElevator = true;
            playerObj.transform.parent = elevatorParent.transform;
            SetMicrogameEndText(onElevator);
        }
    }
}