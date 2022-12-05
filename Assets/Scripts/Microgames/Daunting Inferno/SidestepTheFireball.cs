using UnityEngine;

namespace ShrugWare
{
    public class SidestepTheFireball : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject[] fireballObjects = null;

        private const float FIREBALL_MOVE_SPEED = 30.0f;
        private const float FIREBALL_X_MIN = -70.0f;
        private const float FIREBALL_X_MAX = 100.0f;
        private const float FIREBALL_Y_MIN = 25.0f;
        private const float FIREBALL_Y_MAX = 45.0f;

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

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();
            foreach (GameObject fireballObj in fireballObjects)
            {
                float xSpawnPos = Random.Range(FIREBALL_X_MIN, FIREBALL_X_MAX);
                float ySpawnPos = Random.Range(FIREBALL_Y_MIN, FIREBALL_Y_MAX);
                fireballObj.transform.position = new Vector3(xSpawnPos, ySpawnPos, 0);
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            foreach (GameObject fireballObj in fireballObjects)
            {
                fireballObj.transform.position =
                    Vector3.MoveTowards(fireballObj.transform.position, 
                    new Vector3(fireballObj.transform.position.x, -60, fireballObj.transform.position.z), FIREBALL_MOVE_SPEED * Time.deltaTime);
            }
        }

        protected override bool VictoryCheck() => playerObject.activeInHierarchy;

        private void FireballHit(GameObject fireball)
        {
            playerObject.SetActive(false);
            SetMicrogameEndText(false);
        }
    }
}