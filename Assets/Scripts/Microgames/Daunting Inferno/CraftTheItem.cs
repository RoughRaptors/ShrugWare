using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShrugWare
{
    public class CraftTheItem : Microgame
    {
        [SerializeField]
        GameObject resource1;

        [SerializeField]
        GameObject resource2;

        [SerializeField]
        GameObject resource3;

        [SerializeField]
        GameObject border1Center;

        [SerializeField]
        GameObject border2Center;

        [SerializeField]
        GameObject border3Center;

        private bool item1InSlot = false;
        private bool item2InSlot = false;
        private bool item3InSlot = false;

        const float X_MIN = -1700;
        const float X_MAX = 1700;
        const float Y_MIN = -600;
        const float Y_MAX = 900;

        new private void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            SpawnResources();

            // this is a hard microgame when too fast and, there's not enough time, give it a time boost the faster the timescale is
            if (BossGameManager.Instance != null)
            {
                microGameTime += BossGameManager.Instance.GetCurTimeScale() * 0.75f;
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            // check distance from borders on mouse up
            if(Input.GetMouseButtonUp(0))
            {
                CheckDistance(resource1);
                CheckDistance(resource2);
                CheckDistance(resource3);

                if (item1InSlot && item2InSlot && item3InSlot)
                {
                    SetMicrogameEndText(true);
                }
            }
        }

        protected override bool VictoryCheck()
        {
            return item1InSlot && item2InSlot && item3InSlot;
        }

        public void CheckDistance(GameObject resourceObj)
        {
            // when we drop an object, determine its distance from the three borders
            // if it's less than an amount, it's "in"

            float offset = 5;
            float acceptableDistance = 7.0f;
            float distanceFromBorder1 = Vector2.Distance(resourceObj.transform.position, border1Center.transform.position) - offset;
            if (distanceFromBorder1 < acceptableDistance)
            {
                item1InSlot = true;
            }

            float distanceFromBorder2 = Vector2.Distance(resourceObj.transform.position, border2Center.transform.position) - offset;
            if (distanceFromBorder2 < acceptableDistance)
            {
                item2InSlot = true;
            }

            float distanceFromBorder3 = Vector2.Distance(resourceObj.transform.position, border3Center.transform.position) - offset;
            if (distanceFromBorder3 < acceptableDistance)
            {
                item3InSlot = true;
            }
        }

        private void SpawnResources()
        {
            // don't let them exploit by setting one in all 3 slots sequentially
            item1InSlot = false;
            item2InSlot = false;
            item3InSlot = false;

            TrySpawnResource(resource1);
            TrySpawnResource(resource2);
            TrySpawnResource(resource3);

            resource1.gameObject.SetActive(true);
            resource2.gameObject.SetActive(true);
            resource3.gameObject.SetActive(true);
        }

        private void TrySpawnResource(GameObject resourceObj)
        {
            // try to spawn greater than acceptableDistance away
            // give up after 50 tries
            int numTries = 0;
            while (numTries < 50)
            {
                float xPos = Random.Range(X_MIN, X_MAX);
                float yPos = Random.Range(Y_MIN, Y_MAX);
                Vector2 spawnPos = new Vector2(xPos, yPos);

                float acceptableDistance = 25.0f;
                float distanceFromBorder1 = Vector2.Distance(spawnPos, border1Center.transform.position);
                float distanceFromBorder2 = Vector2.Distance(spawnPos, border2Center.transform.position);
                float distanceFromBorder3 = Vector2.Distance(spawnPos, border3Center.transform.position);
                if (distanceFromBorder1 > acceptableDistance && distanceFromBorder2 > acceptableDistance && distanceFromBorder3 > acceptableDistance)
                {
                    resourceObj.transform.localPosition = spawnPos;
                    break;
                }

                ++numTries;
            }
        }
    }
}