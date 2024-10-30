using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class SidestepTheFireball : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject[] fireballObjects = null;

        [SerializeField]
        List<GameObject> hitVFXList;

        private const float FIREBALL_MOVE_SPEED = 30.0f;
        private const float FIREBALL_X_MIN = -100.0f;
        private const float FIREBALL_X_MAX = 100.0f;

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
                fireballObj.transform.position = new Vector3(xSpawnPos, 75);
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

        protected override bool VictoryCheck()
        {
            return playerObject.activeInHierarchy;
        }

        private void FireballHit(GameObject fireball)
        {
            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            Instantiate(hitVFXList[index], playerObject.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
            fireball.SetActive(false);

            playerObject.SetActive(false);
            SetMicrogameEndText(false);
        }
    }
}