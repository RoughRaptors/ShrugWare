using UnityEngine;
using UnityEngine.Events;

namespace ShrugWare
{
    public class LaserLineOfSightAlternative : Microgame
    {
        [SerializeField]
        GameObject[] laserObjs;

        [SerializeField]
        PlayerMover playerObject = null;

        [SerializeField]
        GameObject barrierObj;

        private bool movingLeft = false;

        public static bool hasBeenHit = false;
        private float timeRunning = 0.0f;

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

            movingLeft = UnityEngine.Random.Range(0, 2) == 0;
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

            float xDir = 0.5f;
            if(movingLeft)
            {
                xDir *= -1.0f;
            }

            barrierObj.transform.position = new Vector3(barrierObj.transform.position.x + xDir * 0.15f, barrierObj.transform.position.y, barrierObj.transform.position.z);
        }
        
        protected override bool VictoryCheck()
        {
            return !hasBeenHit;
        }

        private void EnableLasers()
        {
            playerObject.DisableMovement();
            foreach (GameObject laserObj in laserObjs)
            {
                laserObj.SetActive(true);
            }
        }
        
        public void LaserHit(GameObject gameObj)
        {
            hasBeenHit = true;
            SetMicrogameEndText(false);
        }
    }
}