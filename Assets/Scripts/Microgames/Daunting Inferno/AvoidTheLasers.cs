using UnityEngine;

namespace ShrugWare
{
    public class AvoidTheLasers : Microgame
    {
        [SerializeField]
        GameObject laserParent1;

        const float ROTATESPEED = 30.0f;
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
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            laserParent1.transform.Rotate(0, 0, -ROTATESPEED * Time.deltaTime);
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