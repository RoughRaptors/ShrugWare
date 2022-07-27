using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class DodgeTheFireballPattern : Microgame
    {
        [SerializeField]
        Text instructionsText = null;

        [SerializeField]
        GameObject playerObject = null;

        [SerializeField]
        GameObject[] fireballs = new GameObject[0];

        private bool intercepted = false;

        private const float X_MIN = -50.0f;
        private const float X_MAX = 50.0f;
        private const float Y_MIN = -30.0f;
        private const float Y_MAX = 0.0f;

        [SerializeField] private float FIREBALL_MOVE_SPEED = 60.0f;
        [SerializeField] private float PLAYER_MOVE_SPEED = 10.0f;
        [SerializeField] private float minPlayerStartPos = -40f;
        [SerializeField] private float maxPlayerStartPos = 15f;


        protected override void Awake()
        {
            base.Awake();
            playerObject.transform.position = new Vector2(playerObject.transform.position.x, Random.Range(minPlayerStartPos, maxPlayerStartPos));
        }

        protected override void Start()
        {
            base.Start();

            playerObject.SetActive(true);

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
                    // out of time
                    if (playerObject.activeInHierarchy)
                    {
                        instructionsText.gameObject.SetActive(true);
                        instructionsText.text = "Threaded";
                    }

                    HandleMicrogameEnd(playerObject.activeInHierarchy);
                }
                else
                {
                    foreach(GameObject fireball in fireballs)
                    {
                        fireball.transform.position = 
                        Vector3.MoveTowards(fireball.transform.position, new Vector3(-100, fireball.transform.position.y, 0),
                        FIREBALL_MOVE_SPEED * (Random.Range(1, 1.5f) * Time.deltaTime));
                    }

                    microgameDurationRemaining -= Time.deltaTime;
                    base.Update();
                    HandleInput();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCollider.OnAnyCollision += PlayerCollision;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCollider.OnAnyCollision -= PlayerCollision;
        }

        private void HandleInput()
        {
            if (!intercepted)
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
                
                newPos.y = Mathf.Clamp(newPos.y, minPlayerStartPos, maxPlayerStartPos);
                playerObject.transform.position = newPos;
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            instructionsText.gameObject.SetActive(false);
        }

        private void PlayerCollision(GameObject fireball)
        {
            playerObject.SetActive(false);
            instructionsText.gameObject.SetActive(true);
            instructionsText.text = "Like a leaf on the wind";
        }
    }
}