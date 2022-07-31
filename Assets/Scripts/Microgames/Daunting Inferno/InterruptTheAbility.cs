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

            DataManager.StatEffect damagePlayerEffect = new DataManager.StatEffect();
            damagePlayerEffect.effectType = DataManager.StatModifierType.PlayerCurHealth;
            damagePlayerEffect.amount = 34.0f;
            damagePlayerEffect.asPercentage = false;

            DataManager.StatEffect damageBossEffect = new DataManager.StatEffect();
            damageBossEffect.effectType = DataManager.StatModifierType.BossCurHealth;
            damageBossEffect.amount = 20.0f; 
            damageBossEffect.asPercentage = false;

            DataManager.StatEffect timeScaleEffect = new DataManager.StatEffect();
            timeScaleEffect.effectType = DataManager.StatModifierType.Timescale;
            timeScaleEffect.amount = 0.05f;
            timeScaleEffect.asPercentage = false;

            winEffects.Add(damageBossEffect);
            winEffects.Add(timeScaleEffect);

            lossEffects.Add(damagePlayerEffect);
            lossEffects.Add(timeScaleEffect);

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