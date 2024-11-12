using System.Collections.Generic;
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

        [SerializeField]
        List<GameObject> hitVFXList;

        private bool intercepted = false;
        private const float HEALER_X_MIN = -50;
        private const float HEALER_X_MAX = 50;
        private const float PLAYER_X_MIN = -50.0f;
        private const float PLAYER_X_MAX = 50.0f;
        private const float PLAYER_Y_MIN = -40.0f;
        private const float PLAYER_Y_MAX = 18;
        private const float FIREBALL_X_MIN = -100;
        private const float FIREBALL_X_MAX = 100;

        private const float FIREBALL_MOVE_SPEED = 5.0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            SetupFireball();
            SetupPlayerObject();
            SetupHealer();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
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

            float time = Vector3.Distance(fireballObject.transform.position, healerObject.transform.position) / (microGameTime - timeElapsed) * Time.deltaTime;
            fireballObject.transform.position = Vector3.MoveTowards(fireballObject.transform.position, healerObject.transform.position, time);
        }

        protected override bool VictoryCheck()
        {
            if(!intercepted)
            {
                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], healerObject.transform.position, Quaternion.identity);
                healerObject.SetActive(false);
            }

            return intercepted;
        }

        private void SetupFireball()
        {
            float xPos = Random.Range(FIREBALL_X_MIN, FIREBALL_X_MAX);
            float yPos = 75.0f;
            fireballObject.transform.position = new Vector2(xPos, yPos);
        }

        private void SetupPlayerObject()
        {
            float xPos = Random.Range(PLAYER_X_MIN, PLAYER_X_MAX);
            float yPos = Random.Range(PLAYER_Y_MIN, PLAYER_Y_MAX);
            playerObject.transform.position = new Vector3(xPos, yPos, 0.0f);
        }

        private void SetupHealer()
        {
            float xPos = Random.Range(HEALER_X_MIN, HEALER_X_MAX);
            healerObject.transform.position = new Vector2(xPos, -35);
        }

        private void FireballHit(GameObject fireball)
        {
            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            Instantiate(hitVFXList[index], playerObject.transform.position, Quaternion.identity);

            SetMicrogameEndText(true);
            intercepted = true;
            fireballObject.SetActive(false);
        }
    }
}