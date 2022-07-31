using UnityEngine;

namespace ShrugWare
{
    public class KnockbackedSafely : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject arrowObj = null;

        [SerializeField]
        GameObject safeZoneObj = null;

        private bool inSafeZone = false;

        private const float X_MIN = -30.0f;
        private const float X_MAX = 30.0f;

        private const float PLAYER_MOVE_SPEED = 15.0f;

        protected override void Start()
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

            SetupSafeZone();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += EnterSafeZone;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= EnterSafeZone;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            safeZoneObj.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            HandleInput();

            float angleRad = Mathf.Atan2(playerObject.transform.position.y - arrowObj.transform.position.y,
                playerObject.transform.position.x - arrowObj.transform.position.x);

            float angleDeg = (180 / Mathf.PI) * angleRad;
            arrowObj.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }

        protected override void TimeOut()
        {
            ApplyKnockback();
        }

        protected override bool VictoryCheck() => inSafeZone;

        private void SetupSafeZone()
        {
            safeZoneObj.SetActive(false);
            float xPos = Random.Range(X_MIN, X_MAX);
            safeZoneObj.transform.position = new Vector3(xPos, -12, 0);
        }

        private void HandleInput()
        {
            if (!inSafeZone)
            {
                Vector3 newPos = playerObject.transform.position;
                if (Input.GetKey(KeyCode.W))
                {
                    newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.S) && playerObject.transform.position.y > -5.0f)
                {
                    newPos.y -= PLAYER_MOVE_SPEED * Time.deltaTime;
                }

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

        private void ApplyKnockback()
        {
            Vector3 dir = playerObject.transform.position - arrowObj.transform.position;
            dir.z = 0;
            dir = dir.normalized;
            playerObject.GetComponent<Rigidbody>().AddForce(dir * 2500);

            arrowObj.SetActive(false);
        }

        private void EnterSafeZone(GameObject safeZone)
        {
            inSafeZone = true;
            SetMicrogameEndText(true);
            playerObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            playerObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}