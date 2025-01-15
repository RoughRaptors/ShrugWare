using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShrugWare
{
    public class FireWalls : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        List<GameObject> bottomFireWalls = new List<GameObject>();

        [SerializeField]
        List<GameObject> leftFireWalls = new List<GameObject>();

        [SerializeField]
        AudioClipData fireSound;

        private List<GameObject> objsToEnable = new List<GameObject>();

        private bool hitFire = false;

        private const int NUM_LEFT_TO_SPAWN = 2;
        private const int NUM_BOTTOM_TO_SPAWN = 4;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PlayerCollider.OnBadCollision += FireHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PlayerCollider.OnBadCollision -= FireHit;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            for (int i = 0; i < NUM_BOTTOM_TO_SPAWN; ++i)
            {
                int index = UnityEngine.Random.Range(0, bottomFireWalls.Count);
                bottomFireWalls[index].SetActive(true);
                objsToEnable.Add(bottomFireWalls[index]);
                bottomFireWalls.RemoveAt(index);
            }

            for (int j = 0; j < NUM_LEFT_TO_SPAWN; ++j)
            {
                int index = UnityEngine.Random.Range(0, leftFireWalls.Count);
                leftFireWalls[index].SetActive(true);
                objsToEnable.Add(leftFireWalls[index]);
                leftFireWalls.RemoveAt(index);
            }

            Invoke("SpawnFire", microGameTime);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return !hitFire;
        }

        private void FireHit(GameObject obj)
        {
            hitFire = true;
            SetMicrogameEndText(hitFire);
        }

        private void SpawnFire()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(fireSound);
            }

            foreach (GameObject fireWall in objsToEnable)
            {
                foreach (Transform child in fireWall.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}