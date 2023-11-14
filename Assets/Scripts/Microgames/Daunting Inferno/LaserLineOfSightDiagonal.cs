using UnityEngine;
using UnityEngine.Events;

namespace ShrugWare
{
    public class LaserLineOfSightDiagonal : Microgame
    {
        [SerializeField]
        GameObject[] laserObjs;

        [SerializeField]
        PlayerMover playerObject = null;

        public static bool hasBeenHit = false;
        private float timeRunning = 0.0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            // don't enable the lasers until the end of the microgame
            timeRunning += Time.deltaTime;
            if(timeRunning >= microGameTime)
            {
                EnableLasers();
            }
            else if(hasBeenHit || timeRunning > microGameTime)
            {
                // stop moving if we hit a laser
                playerObject.DisableMovement();
            }
        }

        public static void LaserHit()
        {
            hasBeenHit = true;
        }

        protected override bool VictoryCheck()
        {
            return !hasBeenHit;
        }

        private void EnableLasers()
        {
            foreach(GameObject laserObj in laserObjs)
            {
                laserObj.GetComponent<ShootLaser>().enabled = true;
            }
        }
    }
}