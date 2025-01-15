using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace ShrugWare
{
    public class LaserLineOfSightHorizontal : Microgame
    {
        [SerializeField]
        GameObject[] laserObjs;

        [SerializeField]
        PlayerMover playerMover = null;

        [SerializeField]
        GameObject[] walls;

        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        AudioClipData laserAudio;

        private static bool hasBeenHit = false;
        private float timeRunning = 0.0f;
        private int negative = 1;

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

            // 50/50 chance to go up or down
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                negative = -1;
            }

            foreach(GameObject obj in walls)
            {
                obj.SetActive(true);
            }
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
            else if (timeRunning < microGameTime)
            {
                MoveWalls();
            }
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

        private void LaserHit(GameObject gameObj)
        {
            if (!hasBeenHit)
            {
                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], playerObj.transform.position, Quaternion.identity);
                Destroy(playerObj);

                hasBeenHit = true;
            }
        }

        private void MoveWalls()
        {
            foreach (GameObject wall in walls)
            {
                wall.transform.position = new Vector3(wall.transform.position.x, Mathf.PingPong(Time.time * 20, 40) * negative, wall.transform.position.z);
            }
        }
    }
}