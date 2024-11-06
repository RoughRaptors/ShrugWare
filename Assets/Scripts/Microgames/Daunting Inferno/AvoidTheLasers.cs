using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class AvoidTheLasers : Microgame
    {
        [SerializeField]
        GameObject laserParent;

        [SerializeField]
        List<Vector3> playerSpawnPosList = new List<Vector3>();

        [SerializeField]
        GameObject playerObj;

        [SerializeField]
        GameObject playerSpriteParent;

        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        GameObject rotationObj;

        float delay = 0.75f;
        float rotateSpeed = 30.0f;
        bool hasBeenHit = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += LaserHit;

            int randIndex = UnityEngine.Random.Range(0, playerSpawnPosList.Count);
            playerObj.transform.position = playerSpawnPosList[randIndex];
            
            playerSpriteParent.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= LaserHit;
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            // 50/50 to rotate either direction
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                rotateSpeed *= -1;
            }
            else
            {
                rotationObj.transform.Rotate(new Vector2(0, 180));
            }

            Invoke("HideRotationArrow", 1.0f);
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (timeElapsed >= delay && !hasBeenHit)
            {
                laserParent.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            }
        }

        protected override bool VictoryCheck()
        {
            return !hasBeenHit;
        }

        private void LaserHit(GameObject gameObj)
        {
            if (!hasBeenHit)
            {
                int index = UnityEngine.Random.Range(0, hitVFXList.Count);
                Instantiate(hitVFXList[index], playerObj.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
                Destroy(playerObj);
            }

            SetMicrogameEndText(false);
            hasBeenHit = true;
        }

        private void HideRotationArrow()
        {
            rotationObj.SetActive(false);
        }
    }
}