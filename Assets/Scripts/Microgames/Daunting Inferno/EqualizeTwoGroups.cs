using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class EqualizeTwoGroups : Microgame
    {
        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject groupOfTwoObj;

        [SerializeField]
        GameObject groupOfThreeObj;

        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        AudioClipData fireballSound;

        private const float BALL_SPEED = 30.0f;

        private bool stackedEqually = false;

        private Vector3 meteorStartPos;

        protected override void Start()
        {
            base.Start();
            meteorStartPos = meteorObject.transform.position;
            SetupGroupMembers();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += CheckMakeEqual;
            PlayerCollider.OnBadCollision += EqualCheck;
            PlayerCollider.OnGoodExit += BreakEqual;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= CheckMakeEqual;
            PlayerCollider.OnBadCollision -= EqualCheck;
            PlayerCollider.OnGoodExit -= BreakEqual;
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
            groupOfTwoObj.SetActive(true);
            groupOfThreeObj.SetActive(true);
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);
            float time = Vector3.Distance(meteorObject.transform.position, playerObject.transform.position) / (microGameTime - timeElapsed) * Time.deltaTime;
            meteorObject.transform.position = Vector3.MoveTowards(meteorObject.transform.position, playerObject.transform.position, time);
        }

        protected override bool VictoryCheck()
        {
            if (meteorObject.activeInHierarchy)
            {
                EqualCheck(meteorObject);
            }

            return stackedEqually;
        }

        // 50/50 chance to spawn on left or right
        private void SetupGroupMembers()
        {
            groupOfTwoObj.SetActive(false);
            groupOfThreeObj.SetActive(false);

            if (Random.Range(0, 2) == 0)
            {
                Vector3 tempObjPos = groupOfTwoObj.transform.position;
                groupOfTwoObj.transform.position = groupOfThreeObj.transform.position;
                groupOfThreeObj.transform.position = tempObjPos;
            }
        }

        private void CheckMakeEqual(GameObject members)
        {
            if(members == groupOfTwoObj)
            {
                stackedEqually = true;
            }
        }

        private void BreakEqual(GameObject members)
        {
            stackedEqually = false;
        }

        private void EqualCheck(GameObject meteor)
        {
            if(!stackedEqually)
            {
                playerObject.SetActive(false);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(fireballSound);
            }

            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            Instantiate(hitVFXList[index], playerObject.transform.position, Quaternion.identity);

            SetMicrogameEndText(stackedEqually);
            meteorObject.SetActive(false);
        }
    }
}