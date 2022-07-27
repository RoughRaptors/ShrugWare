using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class SpreadOutForMeteor : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject meteorObject = null;

        [SerializeField]
        GameObject[] groupMembers = new GameObject[0];

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;
        private const float DISTANCE_FOR_VALID_STACK = 20.0f;

        private const float PLAYER_MOVE_SPEED = 20.0f;

        private bool stacked = true;

        private Vector3 member1TargetPos;
        private Vector3 member2TargetPos;

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

            SetupGroupMembers();
            meteorStartPos = meteorObject.transform.position;

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
                        MeteorHit(meteorObject);
                    }

                    HandleMicrogameEnd(!stacked);
                }
                else
                {
                    if (meteorObject.activeInHierarchy)
                    {
                        timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
                        foreach(GameObject member in groupMembers)
                        {
                            member.transform.position = 
                                Vector3.MoveTowards(member.transform.position, member1TargetPos, PLAYER_MOVE_SPEED * Time.deltaTime);
                        }

                        meteorObject.transform.position = Vector3.Lerp(meteorStartPos, playerObject.transform.position, timeRatio);
                        HandleInput();
                    }

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnBadCollision += MeteorHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnBadCollision -= MeteorHit;
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

        private void SetupGroupMembers()
        {
            float member1TargetXPos = Random.Range(X_MIN, X_MAX);
            float member1TargetYPos = Random.Range(Y_MIN,Y_MAX);
            member1TargetPos = new Vector3(member1TargetXPos, member1TargetYPos, 0.0f);

            float member2TargetXPos = Random.Range(X_MIN, X_MAX);
            float member2TargetYPos = Random.Range(Y_MIN, Y_MAX);
            member2TargetPos = new Vector3(member2TargetXPos, member2TargetYPos, 0.0f);
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
        }

        private void MeteorHit(GameObject meteor)
        {
            bool fail = false;
            // be lazy, check member distance from player instead of if the meteor is colliding with all 3 objects
            foreach(GameObject member in groupMembers)
            {
                float memberDistance = Vector3.Distance(member.transform.position, playerObject.transform.position);
                if (memberDistance > DISTANCE_FOR_VALID_STACK) continue;
                fail = true;
                break;
            }

            if(fail)
            {
                instructionsText.text = "Boom";
            }
            else
            {
                stacked = false;
                instructionsText.text = "On point";
            }

            instructionsText.gameObject.SetActive(true);
            meteorObject.SetActive(false);
        }
    }
}