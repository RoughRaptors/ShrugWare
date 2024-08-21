using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class NavigateTheMaze : Microgame
    {
        [SerializeField]
        GameObject mazeObj;

        [SerializeField]
        GameObject allyObj;

        [SerializeField]
        GameObject playerRendererObj;

        [SerializeField]
        List<Vector2> spawnPoints = new List<Vector2>();

        bool reachedTank = false;

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

            SpawnPlayer();

            playerRendererObj.SetActive(true);
            mazeObj.SetActive(true);
            allyObj.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
        }

        protected override bool VictoryCheck()
        {
            return reachedTank;
        }

        private void OnTriggerEnter(Collider other)
        {
            reachedTank = true;
            SetMicrogameEndText(true);
        }

        private void SpawnPlayer()
        {
            int randIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            this.transform.position = spawnPoints[randIndex];
        }
    }
}