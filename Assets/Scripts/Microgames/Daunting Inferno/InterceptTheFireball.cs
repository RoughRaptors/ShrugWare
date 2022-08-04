using UnityEngine;

namespace ShrugWare
{
    public class InterceptTheFireball : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject fireballObject = null;

        [SerializeField]
        GameObject healerObject = null;

        private bool intercepted = false;
        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;

        private const float FIREBALL_MOVE_SPEED = 15.0f;

        protected override void Start()
        {
            base.Start();
            SetupPlayerObject();
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
                Vector3.MoveTowards(fireballObject.transform.position, healerObject.transform.position, FIREBALL_MOVE_SPEED * Time.deltaTime);
        }

        protected override bool VictoryCheck() => intercepted;

        private void SetupPlayerObject()
        {
            float xPos = Random.Range(X_MIN, X_MAX);
            float yPos = Random.Range(Y_MIN, Y_MAX);
            playerObject.transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        private void FireballHit(GameObject fireball)
        {
            SetMicrogameEndText(true);
            intercepted = true;
            fireballObject.SetActive(false);
        }
    }
}