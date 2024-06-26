using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class StealTheChest : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject chestObj = null;

        [SerializeField]
        GameObject[] groupMembers = new GameObject[0];
        Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();

        private const float X_MIN = -75.0f;
        private const float X_MAX = 75.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 10.0f;

        private const float REQUIRED_DISTANCE_FROM_PLAYER = 45.0f;

        private bool lootStolen = false;

        private int numLootSpawnRetries = 0;

        private float timeRatio = 0;

        protected override void Start()
        {
            base.Start();
            foreach(GameObject member in groupMembers)
            {
                startPositions.Add(member, member.transform.position);
            }
            SetupChest();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += ChestHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= ChestHit;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            MoveGroupMembers();
        }

        protected override bool VictoryCheck() => lootStolen;

        private void SetupChest()
        {
            Vector2 spawnPos = new Vector3(0, 0);
            float xPos = 0.0f;
            float yPos = 0.0f;

            // make sure it spawns a certain distance away from the player - if we fail enough times then oh well it doesn't need to be perfect :-D
            while (numLootSpawnRetries < 100)
            {
                xPos = Random.Range(X_MIN, X_MAX);
                yPos = Random.Range(Y_MIN, Y_MAX);
                ++numLootSpawnRetries;

                spawnPos = new Vector3(xPos, yPos);
                if(Vector2.Distance(playerObject.transform.position, spawnPos) > REQUIRED_DISTANCE_FROM_PLAYER)
                {
                    break;
                }
            }

            chestObj.transform.position = spawnPos;
        }

        private void MoveGroupMembers()
        {
            timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
            foreach(GameObject member in groupMembers)
            {
                member.transform.position = Vector3.Lerp(startPositions[member], chestObj.transform.position, timeRatio);
            }
        }

        private void ChestHit(GameObject chest)
        {
            lootStolen = true;
            SetMicrogameEndText(true);
            foreach(GameObject member in groupMembers)
            {
                member.SetActive(false);
            }
        }
    }
}