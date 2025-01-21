using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class StandBehindTank : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject tankObj = null;

        [SerializeField]
        List<GameObject> hitVFXList;

        private const float TANK_X_MIN = -85.0f;
        private const float TANK_X_MAX = 85.0f;
        private const float TANK_Y_MIN = -17.5f;
        private const float TANK_Y_MAX = 15.0f;

        private const float MIN_TANK_SPEED = 25.0f;
        private const float MAX_TANK_SPEED = 55.0f;

        // start invuln just to orient the player a bit easier
        private const float INVULN_TIME = 0.75f;

        // it takes time to start the microgame before we can move
        private const float INVULN_BONUS_TIME = 2.0f;
        private float invulnExpireTime;

        private bool protectedByTank = false;

        private Vector3 targetPos;
        private float tankSpeed = 0.0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            invulnExpireTime = Time.time + INVULN_TIME + INVULN_BONUS_TIME;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += FireEntered;
            PlayerCollider.OnGoodCollision += TankAuraEntered;
            PlayerCollider.OnGoodExit += TankAuraLeft;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= FireEntered;
            PlayerCollider.OnGoodCollision -= TankAuraEntered;
            PlayerCollider.OnGoodExit -= TankAuraLeft;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            MoveTank();
        }

        protected override bool VictoryCheck()
        {
            if(!protectedByTank)
            {
                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], playerObject.transform.position, Quaternion.identity);
            }

            return protectedByTank;
        }

        private void MoveTank()
        {
            tankObj.transform.position = Vector3.MoveTowards(tankObj.transform.position, targetPos, tankSpeed * Time.deltaTime);
            if (Vector3.Distance(tankObj.transform.position, targetPos) <= 0.5f || tankSpeed == 0)
            {
                float newXPos = UnityEngine.Random.Range(TANK_X_MIN, TANK_X_MAX);
                float newYPos = UnityEngine.Random.Range(TANK_Y_MIN, TANK_Y_MAX);
                tankSpeed = UnityEngine.Random.Range(MIN_TANK_SPEED, MAX_TANK_SPEED);
                targetPos = new Vector2(newXPos, newYPos);
            }
        }

        private void TankAuraEntered(GameObject tankAura)
        {
            protectedByTank = true;
        }

        private void TankAuraLeft(GameObject tankAura)
        {
            protectedByTank = false;
        }

        private void FireEntered(GameObject fire)
        {
            if(!protectedByTank && Time.time > invulnExpireTime)
            {
                SetMicrogameEndText(false);
            }
        }
    }
}