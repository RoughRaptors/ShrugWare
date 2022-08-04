using UnityEngine;

namespace ShrugWare
{
    public class SidestepTheFireball : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject fireballObject = null;

        private const float FIREBALL_MOVE_SPEED = 30.0f;


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
        }

        protected override bool VictoryCheck() => playerObject.activeInHierarchy;

        private void FireballHit(GameObject fireball)
        {
            playerObject.SetActive(false);
            SetMicrogameEndText(false);
        }
    }
}