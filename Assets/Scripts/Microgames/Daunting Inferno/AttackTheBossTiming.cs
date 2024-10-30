using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class AttackTheBossTiming : Microgame
    {
        [SerializeField]
        Slider timingSlider;

        [SerializeField]
        GameObject boss;

        private bool attackPressed = false;
        private bool attackedSucceeded = false;
        private bool goingLeft = false;
        private float sliderSpeed = 100.0f;

        protected override void Start()
        {
            base.Start();

            // randomize it a bit
            sliderSpeed = Random.Range(sliderSpeed, sliderSpeed * 2);
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            boss.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (!attackPressed)
            {
                if (goingLeft && timingSlider.value >= 0)
                {
                    timingSlider.value -= sliderSpeed * Time.deltaTime;
                }
                else if (!goingLeft && timingSlider.value <= 100)
                {
                    timingSlider.value += sliderSpeed * Time.deltaTime;
                }

                // swap directions
                if (timingSlider.value == 0 || timingSlider.value == 100)
                {
                    goingLeft = !goingLeft;
                }
            }
        }

        protected override bool VictoryCheck() => attackedSucceeded;

        // success if we're in the right 80% of the slider
        public void AttackButtonPressed()
        {
            AudioManager.Instance.PlayAudioClip(DataManager.AudioType.ButtonClick);

            if (!attackPressed)
            {
                attackPressed = true;

                float attackValue = timingSlider.value;
                if (attackValue >= 70)
                {
                    attackedSucceeded = true;
                    SetMicrogameEndText(true, "Yay timing");
                }
                else
                {
                    SetMicrogameEndText(false, "Not quite");
                }
            }
        }
    }
}