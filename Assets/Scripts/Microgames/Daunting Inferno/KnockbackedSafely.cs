using UnityEngine;

namespace ShrugWare
{
    public class KnockbackedSafely : Microgame
    {
        [SerializeField]
        PlayerMover playerObject = null;

        [SerializeField]
        GameObject arrowObj = null;

        [SerializeField]
        GameObject safeZoneObj = null;

        private bool inSafeZone = false;

        private const float X_MIN = -30.0f;
        private const float X_MAX = 30.0f;

        protected override void Start()
        {
            base.Start();
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
            float angle = Mathf.Atan2(playerObject.transform.position.y - arrowObj.transform.position.y,
                playerObject.transform.position.x - arrowObj.transform.position.x);

            arrowObj.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }

        protected override void TimeOut()
        {
            playerObject.DisableMovement();
            ApplyKnockback();
        }

        protected override bool VictoryCheck() => inSafeZone;

        private void SetupSafeZone()
        {
            safeZoneObj.SetActive(false);
            float xPos = Random.Range(X_MIN, X_MAX);
            safeZoneObj.transform.position = new Vector3(xPos, -12, 0);
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