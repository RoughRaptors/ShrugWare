using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class StackUpForMeteor : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject[] groupMembers = new GameObject[0];

        [SerializeField]
        List<GameObject> hitVFXList;

        private const float X_MIN = -65.0f;
        private const float X_MAX = 65.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;
        private const float DISTANCE_FOR_VALID_STACK = 10.0f;
        private const float MEMBER_MOVE_SPEED = 22.5f;
        private bool stacked = true;

        private float timeRatio = 0;
        private Vector3 meteorStartPos;

        private List<GameObject> overlapObjects = new List<GameObject>();

        protected override void Start()
        {
            base.Start();
            SetupPlayerObject();
            meteorStartPos = meteorObject.transform.position;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnAnyCollision += OverlapObject;
            PlayerCollider.OnGoodExit += LeftAura;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnAnyCollision -= OverlapObject;
            PlayerCollider.OnGoodExit -= LeftAura;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            if (!meteorObject.activeInHierarchy) return;

            timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
            meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, timeRatio);

            foreach(GameObject groupMember in groupMembers)
            {
                groupMember.transform.position =
                    Vector3.MoveTowards(groupMember.transform.position, playerObject.transform.position, MEMBER_MOVE_SPEED * Time.deltaTime);
            }
        }

        protected override bool VictoryCheck()
        {
            return overlapObjects.Count == groupMembers.Length && overlapObjects.Count > 0;
        }

        private void SetupPlayerObject()
        {
            float xPos = Random.Range(X_MIN, X_MAX);
            float yPos = Random.Range(Y_MIN, Y_MAX);
            playerObject.transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        private void OverlapObject(GameObject go)
        {
            if (!overlapObjects.Contains(go) && go.name != "Meteor")
            {
                overlapObjects.Add(go);
            }
            else if (go.name == "Meteor")
            {
                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], playerObject.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
                go.SetActive(false);

                if (overlapObjects.Count != groupMembers.Length || overlapObjects.Count == 0)
                {
                    SetMicrogameEndText(false);
                    Destroy(playerObject);
                }

                SetMicrogameEndText(true);
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