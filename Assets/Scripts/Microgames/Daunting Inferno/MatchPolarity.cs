using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class MatchPolarity : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject eletricityObj = null;

        [SerializeField]
        GameObject negativeGroupObj;

        [SerializeField]
        GameObject positiveGroupObj;

        [SerializeField]
        GameObject playerNegativeObj;

        [SerializeField]
        GameObject playerPositiveObj;

        private const float PLAYER_MOVE_SPEED = 40.0f;

        private bool polarityMatched = false;

        private float timeRatio = 0;
        private Vector3 electricityStartPos;

        bool playerPositive = false;

        new private void Start()
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

            electricityStartPos = eletricityObj.transform.position;

            playerNegativeObj.SetActive(false);
            playerPositiveObj.SetActive(false);
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
                    if (eletricityObj.activeInHierarchy)
                    {
                        ElectricHit(eletricityObj);
                    }

                    HandleMicrogameEnd(polarityMatched);
                }
                else
                {
                    timeRatio += Time.deltaTime / DataManager.MICROGAME_DURATION_SECONDS;
                    eletricityObj.transform.position = Vector3.Lerp(electricityStartPos, playerObject.transform.position, timeRatio);

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                    HandleInput();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnGoodCollision += EnterField;
            PlayerCollider.OnBadCollision += ElectricHit;
            PlayerCollider.OnGoodExit += ExitField;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnGoodCollision -= EnterField;
            PlayerCollider.OnBadCollision -= ElectricHit;
            PlayerCollider.OnGoodExit -= ExitField;
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

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);

            // do this here so they don't see their own polarity until it starts
            SetupPlayer();
        }

        private void SetupPlayer()
        {
            playerPositive = Random.Range(0, 2) == 0;
            if(playerPositive)
            {
                playerPositiveObj.SetActive(true);
            }
            else
            {
                playerNegativeObj.SetActive(true);
            }
        }

        // 50/50 chance for polarity to be on one side
        private void SetupGroupMembers()
        {
            if (Random.Range(0, 2) == 0)
            {
                Vector3 tempObjPos = negativeGroupObj.transform.position;
                negativeGroupObj.transform.position = positiveGroupObj.transform.position;
                positiveGroupObj.transform.position = tempObjPos;
            }
        }

        private void EnterField(GameObject electricField)
        {
            bool isNegative = electricField != positiveGroupObj;

            if(isNegative ^ playerPositive)
            {
                polarityMatched = true;
            }
        }

        private void ExitField(GameObject electricField)
        {
            polarityMatched = false;
        }

        private void ElectricHit(GameObject electric)
        {
            if (polarityMatched)
            {
                polarityMatched = true;
                instructionsText.text = "Shocking Performance";
            }
            else
            {
                instructionsText.text = "BZZZT DED";
            }

            instructionsText.gameObject.SetActive(true);
            eletricityObj.SetActive(false);
        }
    }
}