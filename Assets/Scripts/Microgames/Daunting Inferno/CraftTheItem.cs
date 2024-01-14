using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

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

        GameObject selectedObject;
        private bool item1InSlot = false;
        private bool item2InSlot = false;
        private bool item3InSlot = false;

        const float X_MIN = -3500;
        const float X_MAX = -350;
        const float Y_MIN = -1500;
        const float Y_MAX = -500;

        new private void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            microGameTime = 200;
            SpawnResources();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            // drag our items
            if (Input.GetMouseButton(0))
            {
                RaycastHit raycastHit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (raycastHit.transform != null)
                    {
                        // move our object to the hit point of the raycast
                        raycastHit.transform.position = raycastHit.point;
                        selectedObject = raycastHit.transform.gameObject;
                    }
                }
                else if (selectedObject != null)
                {
                    // don't unselect the object if we are still dragging but we drag off of the item too quickly
                    selectedObject.transform.position = ray.GetPoint(100);
                }
            }

            // check distance from borders on mouse up
            if(Input.GetMouseButtonUp(0))
            {
                CheckDistance();

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

        private void CheckDistance()
        {
            // when we drop an object, determine its distance from the three borders
            // if it's less than an amount, it's "in"
            if(selectedObject != null)
            {
                float offset = 5;
                float acceptableDistance = 7.0f;
                float distanceFromBorder1 = Vector3.Distance(selectedObject.transform.position, border1Center.transform.position) - offset;
                if(distanceFromBorder1 < acceptableDistance)
                {
                    item1InSlot = true;
                }

                float distanceFromBorder2 = Vector3.Distance(selectedObject.transform.position, border2Center.transform.position) - offset;
                if (distanceFromBorder2 < acceptableDistance)
                {
                    item2InSlot = true;
                }

                float distanceFromBorder3 = Vector3.Distance(selectedObject.transform.position, border3Center.transform.position) - offset;
                if (distanceFromBorder3 < acceptableDistance)
                {
                    item3InSlot = true;
                }
            }

            // unselect our item
            selectedObject = null;
        }

        private void SpawnResources()
        {
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
                Vector2 spawnPos = new Vector3(xPos, yPos, 0);

                float acceptableDistance = 30.0f;
                float distanceFromBorder1 = Vector2.Distance(resourceObj.transform.position, border1Center.transform.position);
                float distanceFromBorder2 = Vector2.Distance(resourceObj.transform.position, border2Center.transform.position);
                float distanceFromBorder3 = Vector2.Distance(resourceObj.transform.position, border3Center.transform.position);
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