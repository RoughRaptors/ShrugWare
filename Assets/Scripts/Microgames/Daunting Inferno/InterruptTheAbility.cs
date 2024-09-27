using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class InterruptTheAbility : Microgame
    {
        [SerializeField]
        Slider abilityBar;

        [SerializeField]
        GameObject interruptButton;

        [SerializeField]
        GameObject bossObj;

        [SerializeField]
        GameObject attackButton1;

        [SerializeField]
        GameObject attackButton2;

        // take some time to spawn to make it challenging
        private bool interrupted = false;
        private float castDelay = 0.0f;
        private float castTimePercent = 1;

        protected override void Start()
        {
            base.Start();
            interruptButton.SetActive(false);
            attackButton1.SetActive(false);
            attackButton2.SetActive(false);
            abilityBar.gameObject.SetActive(false);
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            bossObj.SetActive(true);

            SetupButtonLocations();

            interruptButton.SetActive(true);
            attackButton1.SetActive(true);
            attackButton2.SetActive(true);

            castDelay = Random.Range(0.5f, 1.5f);
            castTimePercent -= castDelay / microGameTime;
            Invoke("CastAbility", castDelay);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            if (abilityBar.isActiveAndEnabled && !interrupted)
            {
                abilityBar.value = Mathf.Lerp(abilityBar.minValue, abilityBar.maxValue, castTimePercent - timePercentLeft);
            }
        }

        protected override bool VictoryCheck() => interrupted;

        private void CastAbility()
        {
            abilityBar.gameObject.SetActive(true);
        }

        private void SetupButtonLocations()
        {
            List<GameObject> buttons = new List<GameObject> { attackButton1, attackButton2, interruptButton };
            List<Vector3> locations = new List<Vector3> { attackButton1.transform.position, attackButton2.transform.position, interruptButton.transform.position };
            foreach(GameObject button in buttons)
            {
                // pick a random location
                int randIndex = UnityEngine.Random.Range(0, locations.Count);
                Vector3 location = locations[randIndex];
                button.transform.position = location;
                locations.RemoveAt(randIndex);
            }
        }

        public void InterruptButtonPressed()
        {
            if (!abilityBar.isActiveAndEnabled)
            {
                SetMicrogameEndText(false, "Too soon");
            }
            else
            {
                interrupted = true;
                SetMicrogameEndText(true);
            }

            interruptButton.SetActive(false);
        }

        public void OnAttackPressed(GameObject obj)
        {
            // increase the cast percentage
            if (abilityBar.isActiveAndEnabled)
            {
                castTimePercent += .125f;
                obj.SetActive(false);

                timeElapsed += 1.0f;
                microGameTime += 1.0f;
            }
        }
    }
}