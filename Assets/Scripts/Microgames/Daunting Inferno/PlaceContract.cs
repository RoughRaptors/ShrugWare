using UnityEngine;
using System.Collections.Generic;

namespace ShrugWare
{
    public class PlaceContract : Microgame
    {        
        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        GameObject contractObj;

        private bool failedPlacement = false;

        private const float SPAWN_INTERVAL = 2.25f; //how often a contract spawns
        private const float DROP_TIMER = 1.35f; // how long we have before the contract drops
        private float lastPlacedTime = 1.5f; // start with this already set to give the player a bit of time to orient themselves

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            microGameTime *= 3.0f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (Time.time > lastPlacedTime + SPAWN_INTERVAL)
            {
                lastPlacedTime = Time.time;
                SpawnContract();
            }
        }

        protected override bool VictoryCheck()
        {
            return !failedPlacement;
        }

        private void SpawnContract()
        {
            Vector2 spawnPos = this.transform.position;
            spawnPos = new Vector2(spawnPos.x, spawnPos.y + 3.0f);
            Quaternion rotPos = contractObj.transform.rotation;
            GameObject newContract = Instantiate(contractObj, spawnPos, rotPos);
            newContract.SetActive(true);
        }
    }
}