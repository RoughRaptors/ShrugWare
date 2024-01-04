using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace ShrugWare
{
    public class CraftTheItem : Microgame
    {
        [SerializeField]
        GameObject item1;

        [SerializeField]
        GameObject item2;

        [SerializeField]
        GameObject item3;

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

        new private void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
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
    }
}