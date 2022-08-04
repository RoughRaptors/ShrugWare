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

        private const float X_MIN = -65.0f;
        private const float X_MAX = 65.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;
        private const float DISTANCE_FOR_VALID_STACK = 10.0f;
        private const float MEMBER_MOVE_SPEED = 22.5f;
        private bool stacked = true;

        private float timeRatio = 0;
        private Vector3 meteorStartPos;

        protected override void Start()
        {
            base.Start();
            SetupPlayerObject();
            meteorStartPos = meteorObject.transform.position;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += MeteorCheck;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= MeteorCheck;
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
            if (meteorObject.activeInHierarchy)
            {
                MeteorCheck(meteorObject);
            }

            return stacked;
        }

        private void SetupPlayerObject()
        {
            float xPos = Random.Range(X_MIN, X_MAX);
            float yPos = Random.Range(Y_MIN, Y_MAX);
            playerObject.transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        private void MeteorCheck(GameObject meteor)
        {
            // be lazy, check member distance from player instead of if the meteor is colliding with all 3 objects
            foreach(GameObject member in groupMembers)
            {
                float memberDistance = Vector3.Distance(member.transform.position, playerObject.transform.position);
                if (memberDistance < DISTANCE_FOR_VALID_STACK) continue;
                stacked = false;
                break;
            }
            SetMicrogameEndText(stacked);
            meteorObject.SetActive(false);
        }
    }
}