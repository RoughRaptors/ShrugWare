using UnityEngine;

namespace ShrugWare
{
    public class AvoidTheLasers : Microgame
    {
        [SerializeField]
        GameObject laserParent;

        float rotateSpeed = 30.0f;
        bool hasBeenHit = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += LaserHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= LaserHit;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            // 50/50 to rotate either direction
            if (Random.Range(0, 2) == 0)
            {
                rotateSpeed *= -1;
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            
            laserParent.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }

        protected override bool VictoryCheck()
        {
            return !hasBeenHit;
        }

        private void LaserHit(GameObject gameObj)
        {
            SetMicrogameEndText(false);
            hasBeenHit = true;
        }
    }
}