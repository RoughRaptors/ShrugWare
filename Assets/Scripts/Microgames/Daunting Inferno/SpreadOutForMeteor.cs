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

        [SerializeField]
        List<GameObject> hitVFXList;

        private const float X_MIN = -80.0f;
        private const float X_MAX = 80.0f;
        private const float Y_MIN = -35.0f;
        private const float Y_MAX = 30.0f;

        private const float PLAYER_MOVE_SPEED = 20.0f;

        private Vector3 member0TargetPos;
        private Vector3 member1TargetPos;

        private bool gameEnded = false;

        private List<GameObject> overlapObjects = new List<GameObject>();

        protected override void Start()
        {
            base.Start();
            SetupGroupMembers();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += OverlapObject;
            PlayerCollider.OnBadExit += LeftAura;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= OverlapObject;
            PlayerCollider.OnBadExit -= LeftAura;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!meteorObject.activeInHierarchy)
            {
                return;
            }

            // would be nice if this wasn't hard coded and turned into a struct
            groupMembers[0].transform.position =
                    Vector2.MoveTowards(groupMembers[0].transform.position, member0TargetPos, PLAYER_MOVE_SPEED * Time.deltaTime);

            groupMembers[1].transform.position =
                Vector2.MoveTowards(groupMembers[1].transform.position, member1TargetPos, PLAYER_MOVE_SPEED * Time.deltaTime);

            float time = Vector2.Distance(meteorObject.transform.position, playerObject.transform.position) / (microGameTime - timeElapsed) * Time.deltaTime;
            meteorObject.transform.position = Vector2.MoveTowards(meteorObject.transform.position, playerObject.transform.position, time);
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
            member0TargetPos = new Vector2(member1TargetXPos, member1TargetYPos);

            float member2TargetXPos = Random.Range(X_MIN, X_MAX);
            float member2TargetYPos = Random.Range(Y_MIN, Y_MAX);
            member1TargetPos = new Vector2(member2TargetXPos, member2TargetYPos);
        }

        private void OverlapObject(GameObject go)
        {
            if(!overlapObjects.Contains(go) && go.name != "Meteor")
            {
                overlapObjects.Add(go);
            }
            else if(go.name == "Meteor")
            {
                gameEnded = true;

                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], playerObject.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
                go.SetActive(false);

                if(overlapObjects.Count > 0)
                {
                    SetMicrogameEndText(false);
                    Destroy(playerObject);
                }

                SetMicrogameEndText(true);
            }
        }

        private void LeftAura(GameObject go)
        {
            if (overlapObjects.Contains(go) && !gameEnded)
            {
                overlapObjects.Remove(go);
            }
        }
    }
}