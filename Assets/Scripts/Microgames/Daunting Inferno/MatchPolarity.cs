using UnityEngine;

namespace ShrugWare
{
    public class MatchPolarity : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject eletricityObj = null;

        [SerializeField]
        GameObject negativeGroupObj;

        [SerializeField]
        GameObject positiveGroupObj;

        [SerializeField]
        GameObject playerNegativeObj;

        [SerializeField]
        GameObject playerPositiveObj;

        private const float PLAYER_MOVE_SPEED = 40.0f;

        private bool polarityMatched = false;

        private Vector3 electricityStartPos;

        bool playerPositive = false;

        new private void Start()
        {
            base.Start();
            electricityStartPos = eletricityObj.transform.position;
            playerNegativeObj.SetActive(false);
            playerPositiveObj.SetActive(false);
            SetupGroupMembers();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += EnterField;
            PlayerCollider.OnBadCollision += ElectricHit;
            PlayerCollider.OnGoodExit += ExitField;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= EnterField;
            PlayerCollider.OnBadCollision -= ElectricHit;
            PlayerCollider.OnGoodExit -= ExitField;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            SetupPlayer();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            eletricityObj.transform.position = Vector3.Lerp(electricityStartPos, playerObject.transform.position, 1 - timePercentLeft);
            HandleInput();
        }

        protected override bool VictoryCheck()
        {
            if (eletricityObj.activeInHierarchy)
            {
                ElectricHit(eletricityObj);
            }

            return polarityMatched;
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

        private void SetupPlayer()
        {
            playerPositive = Random.Range(0, 2) == 0;
            if(playerPositive)
            {
                playerPositiveObj.SetActive(true);
            }
            else
            {
                playerNegativeObj.SetActive(true);
            }
        }

        // 50/50 chance for polarity to be on one side
        private void SetupGroupMembers()
        {
            if (Random.Range(0, 2) == 0)
            {
                Vector3 tempObjPos = negativeGroupObj.transform.position;
                negativeGroupObj.transform.position = positiveGroupObj.transform.position;
                positiveGroupObj.transform.position = tempObjPos;
            }
        }

        private void EnterField(GameObject electricField)
        {
            bool isNegative = electricField != positiveGroupObj;

            if(isNegative ^ playerPositive)
            {
                polarityMatched = true;
            }
        }

        private void ExitField(GameObject electricField)
        {
            polarityMatched = false;
        }

        private void ElectricHit(GameObject electric)
        {
            SetMicrogameEndText(polarityMatched);
            eletricityObj.SetActive(false);
        }
    }
}