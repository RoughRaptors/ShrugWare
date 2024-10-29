using UnityEngine;
using System.Collections.Generic;

namespace ShrugWare
{
    public class SpreadOutForMeteor : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject[] groupMembers = new GameObject[0];

        private const float X_MIN = -80.0f;
        private const float X_MAX = 80.0f;
        private const float Y_MIN = -35.0f;
        private const float Y_MAX = 30.0f;

        private const float PLAYER_MOVE_SPEED = 20.0f;

        private Vector3 member0TargetPos;
        private Vector3 member1TargetPos;

        private float timeRatio = 0;
        private Vector3 meteorStartPos;

        private List<GameObject> overlapObjects = new List<GameObject>();

        protected override void Start()
        {
            base.Start();
            SetupGroupMembers();
            meteorStartPos = meteorObject.transform.position;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += EnteredAura;
            PlayerCollider.OnBadExit += LeftAura;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= EnteredAura;
            PlayerCollider.OnBadExit -= LeftAura;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!meteorObject.activeInHierarchy)
            {
                return;
            }

            timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;

            // would be nice if this wasn't hard coded and turned into a struct
            groupMembers[0].transform.position =
                    Vector3.MoveTowards(groupMembers[0].transform.position, member0TargetPos, PLAYER_MOVE_SPEED * Time.deltaTime);

            groupMembers[1].transform.position =
                Vector3.MoveTowards(groupMembers[1].transform.position, member1TargetPos, PLAYER_MOVE_SPEED * Time.deltaTime);


            meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, timeRatio);
        }

        protected override bool VictoryCheck()
        {
            return overlapObjects.Count == 0;
        }

        private void SetupGroupMembers()
        {
            // pick a location to move to and go there
            float member1TargetXPos = Random.Range(X_MIN, X_MAX);
            float member1TargetYPos = Random.Range(Y_MIN, Y_MAX);
            member0TargetPos = new Vector3(member1TargetXPos, member1TargetYPos, 0.0f);

            float member2TargetXPos = Random.Range(X_MIN, X_MAX);
            float member2TargetYPos = Random.Range(Y_MIN, Y_MAX);
            member1TargetPos = new Vector3(member2TargetXPos, member2TargetYPos, 0.0f);
        }

        private void EnteredAura(GameObject go)
        {
            if(!overlapObjects.Contains(go) && go.name != "Meteor")
            {
                overlapObjects.Add(go);
            }
        }

        private void LeftAura(GameObject go)
        {
            if (overlapObjects.Contains(go))
            {
                overlapObjects.Remove(go);
            }
        }
    }
}