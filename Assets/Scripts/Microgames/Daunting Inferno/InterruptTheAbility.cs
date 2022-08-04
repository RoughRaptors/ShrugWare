using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class InterruptTheAbility : Microgame
    {
        [SerializeField]
        Slider abilityBar;

        [SerializeField]
        Button interruptButton;

        // take some time to spawn to make it challenging
        private bool interrupted = false;
        private float castDelay = 0.0f;
        private float castTimePercent = 1;

        new private void Start()
        {
            base.Start();
            interruptButton.gameObject.SetActive(false);
            interruptButton.onClick.AddListener(InterruptButtonPressed);
            abilityBar.gameObject.SetActive(false);
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            interruptButton.gameObject.SetActive(true);
            castDelay = Random.Range(0.5f, 1.5f);
            castTimePercent -= castDelay / microGameTime;
            Invoke(nameof(CastAbility), castDelay);
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

        private void InterruptButtonPressed()
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

            interruptButton.gameObject.SetActive(false);
        }
    }
}