using UnityEngine;

namespace ShrugWare
{
    public class EqualizeTwoGroups : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject groupOfTwoObj;

        [SerializeField]
        GameObject groupOfThreeObj;

        private const float PLAYER_MOVE_SPEED = 30.0f;

        private bool stackedEqually = false;

        private Vector3 meteorStartPos;


        protected override void Start()
        {
            base.Start();
            meteorStartPos = meteorObject.transform.position;
            SetupGroupMembers();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += MakeEqual;
            PlayerCollider.OnBadCollision += EqualCheck;
            PlayerCollider.OnGoodExit += BreakEqual;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= MakeEqual;
            PlayerCollider.OnBadCollision -= EqualCheck;
            PlayerCollider.OnGoodExit -= BreakEqual;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            groupOfTwoObj.SetActive(true);
            groupOfThreeObj.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, 1 - timePercentLeft);
            HandleInput();
        }

        protected override bool VictoryCheck()
        {
            if (meteorObject.activeInHierarchy)
            {
                EqualCheck(meteorObject);
            }

            return stackedEqually;
        }

        private void HandleInput()
        {
            Vector3 newPos = playerObject.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                newPos.y -= PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                newPos.x -= PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                newPos.x += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            playerObject.transform.position = newPos;
        }

        // 50/50 chance to spawn on left or right
        private void SetupGroupMembers()
        {
            groupOfTwoObj.SetActive(false);
            groupOfThreeObj.SetActive(false);

            if (Random.Range(0, 2) == 0)
            {
                Vector3 tempObjPos = groupOfTwoObj.transform.position;
                groupOfTwoObj.transform.position = groupOfThreeObj.transform.position;
                groupOfThreeObj.transform.position = tempObjPos;
            }
        }

        private void MakeEqual(GameObject members)
        {
            stackedEqually = true;
        }

        private void BreakEqual(GameObject members)
        {
            stackedEqually = false;
        }

        private void EqualCheck(GameObject meteor)
        {
            SetMicrogameEndText(stackedEqually);
            meteorObject.SetActive(false);
        }
    }
}