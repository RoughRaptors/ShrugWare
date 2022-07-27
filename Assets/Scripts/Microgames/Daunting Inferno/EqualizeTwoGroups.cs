using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class EqualizeTwoGroups : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject groupOfTwoObj;

        [SerializeField]
        GameObject groupOfThreeObj;

        private const float PLAYER_MOVE_SPEED = 30.0f;

        private bool stackedEqually = false;

        private float timeRatio = 0;
        private Vector3 meteorStartPos;


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

            meteorStartPos = meteorObject.transform.position;

            SetupGroupMembers();

            StartCoroutine(DisableInstructionsText());
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;

            // don't "start" the microgame until we can orient the player to the microgame
            if (timeElapsed >= DataManager.SECONDS_TO_START_MICROGAME)
            {
                if (microgameDurationRemaining <= 0.0f)
                {
                    // out of time - we should have collided already, but maybe not
                    if (meteorObject.activeInHierarchy)
                    {
                        EqualCheck(meteorObject);
                    }

                    HandleMicrogameEnd(stackedEqually);
                }
                else
                {
                    timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
                    meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, timeRatio);

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                    HandleInput();
                }
            }
        }

        private void HandleInput()
        {
            Vector3 newPos = playerObject.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
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

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += MakeEqual;
            PlayerCollider.OnBadCollision += EqualCheck;
            PlayerCollider.OnGoodExit += BreakEqual;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= MakeEqual;
            PlayerCollider.OnBadCollision -= EqualCheck;
            PlayerCollider.OnGoodExit -= BreakEqual;
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);

            groupOfTwoObj.SetActive(true);
            groupOfThreeObj.SetActive(true);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == meteorObject)
            {
                EqualCheck(meteorObject);
            }
            else if(other.gameObject == groupOfTwoObj)
            {
                stackedEqually = true;
            }
        }

        private void MakeEqual(GameObject members)
        {
            stackedEqually = true;
        }

        private void BreakEqual(GameObject members)
        {
            stackedEqually = false;
        }

        private void EqualCheck(GameObject meteor)
        {
            if (stackedEqually)
            {
                stackedEqually = true;
                instructionsText.text = "Equality!";
            }
            else
            {
                instructionsText.text = "Count better";
            }

            instructionsText.gameObject.SetActive(true);
            meteorObject.SetActive(false);
        }
    }
}