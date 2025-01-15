using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShrugWare
{
    public class LaserLineOfSightAlternative : Microgame
    {
        [SerializeField]
        GameObject[] laserObjs;

        [SerializeField]
        PlayerMover playerMover = null;

        [SerializeField]
        GameObject barrierObj;

        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        AudioClipData laserAudio;

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
                playerMover.DisableMovement();
            }

            float xDir = 0.5f;
            if(movingLeft)
            {
                xDir *= -1.0f;
            }

            barrierObj.transform.position = new Vector3(barrierObj.transform.position.x + xDir * 0.9f, barrierObj.transform.position.y, barrierObj.transform.position.z);
        }
        
        protected override bool VictoryCheck()
        {
            return !hasBeenHit;
        }

        private void EnableLasers()
        {
            playerMover.DisableMovement();
            foreach (GameObject laserObj in laserObjs)
            {
                laserObj.SetActive(true);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(laserAudio);
            }
        }
        
        public void LaserHit(GameObject gameObj)
        {
            if (!hasBeenHit)
            {
                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], playerObj.transform.position, Quaternion.identity);
                Destroy(playerObj);
            }

            hasBeenHit = true;
            SetMicrogameEndText(false);
        }
    }
}