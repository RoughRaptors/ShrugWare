using UnityEngine;

namespace ShrugWare
{
    public class SidestepTheFireball : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject fireballObject = null;

        private bool intercepted = false;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;

        private const float FIREBALL_MOVE_SPEED = 30.0f;
        private const float PLAYER_MOVE_SPEED = 11.0f;

        protected override void Start()
        {
            base.Start();

            playerObject.SetActive(true);

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
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += FireballHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= FireballHit;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            fireballObject.transform.position = 
                Vector3.MoveTowards(fireballObject.transform.position, new Vector3(0, -60, 0), FIREBALL_MOVE_SPEED * Time.deltaTime);

            HandleInput();
        }

        protected override bool VictoryCheck() => playerObject.activeInHierarchy;

        private void HandleInput()
        {
            if (!intercepted)
            {
                Vector3 newPos = playerObject.transform.position;
                if (Input.GetKey(KeyCode.A))
                {
                    newPos.x -= PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    newPos.x += PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                playerObject.transform.position = newPos;
            }
        }

        private void FireballHit(GameObject fireball)
        {
            playerObject.SetActive(false);
            SetMicrogameEndText(false);
        }
    }
}