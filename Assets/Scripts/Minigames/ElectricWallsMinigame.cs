using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace ShrugWare
{
    // need to collect x collectibles
    // 0-10s - come from right
    // 10s-20s come from top + right
    // 20s-30s come from top + right + bottom

    public class ElectricWallsMinigame : Minigame
    {
        [SerializeField]
        GameObject lightningBallInitialObj;

        [SerializeField]
        GameObject oscillatorInitialObj;

        [SerializeField]
        GameObject collectibleInitialObj;

        [SerializeField]
        TextMeshProUGUI playerHealthText;

        [SerializeField]
        GameObject continueButton;

        [SerializeField]
        TextMeshProUGUI endGameText;

        [SerializeField]
        TextMeshProUGUI enemyHealthText;

        // just to have some variation have some different sprites that spawn in
        [SerializeField]
        List<Sprite> collectibleObjectSprites = new List<Sprite>();

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        List<GameObject> collectVFXList;

        [SerializeField]
        AudioClipData lightningImpactSound;

        [SerializeField]
        AudioClipData collectiblePickupSound;

        [SerializeField]
        AudioClipData deathSound;

        [SerializeField]
        TextMeshProUGUI instructionsText;

        private const int ENEMY_START_HEALTH = 100;
        private float enemyHealth = ENEMY_START_HEALTH;

        // if we've been hit within INVULN_TIME, don't do damage
        private float invulnExpireTime = 0.0f;
        private const float INVULN_DURATION = 1.05f;

        private const float COLLECTIBLE_SPAWN_DISTANCE = 300;
#if UNITY_EDITOR
        private const int COLLECTIBLE_DAMAGE = 20;
#else
        private const int COLLECTIBLE_DAMAGE = 10;
#endif

        private const float COLLECTIBLE_X_MIN = -825.0f;
        private const float COLLECTIBLE_X_MAX = 825.0f;
        private const float COLLECTIBLE_Y_MIN = -250.0f;
        private const float COLLECTIBLE_Y_MAX = 275.0f;

        private float mitigation = 0.0f;

        private float timeInGame = 0.0f;
        private const float PLAYER_SPEED = 50.0f;

        // float so we can take partial damage via damage mitigation
        private const float START_HEALTH = 100;
        private float healthRemaining = 100;
        private bool gameRunning = false;

        private const float SPAWN_INTERVAL = 2.25f;
        private float lastSpawnTime = float.MinValue;

        private const float DIST_MIN = 15.0f;
        private const float DIST_MAX = 45.0f;
        private const float LEFT_ORB_X_SPAWN_POS_MIN = 0.0f;
        private const float LEFT_ORB_X_SPAWN_POS_MAX = 75.0f;
        private const float RIGHT_ORB_X_SPAWN_POS_MIN = 15.0f;
        private const float RIGHT_ORB_X_SPAWN_POS_MAX = 100.0f;
        private const float WALL_X_MIN_SPAWN_POS = 0.0f;
        private const float WALL_X_MAX_SPAWN_POS = 0.0f;

        private const float ELECTRICITY_SPEED = .3f;

        List<GameObject> electricObjs = new List<GameObject>();

        private void Awake()
        {
            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.ReadyScene(false);
            }

            continueButton.SetActive(false);
        }

        private void OnEnable()
        {
            PlayerCollider.OnBadCollision += CollideElectricity;
            PlayerCollider.OnGoodCollision += CollideCollectible;
        }

        private void OnDisable()
        {
            PlayerCollider.OnBadCollision -= CollideElectricity;
            PlayerCollider.OnGoodCollision -= CollideCollectible;
        }

        private new void Start()
        {
            base.Start();
            healthRemaining = START_HEALTH + healthToAdd;

            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.PlayerInventory.RecalculateStats();
                mitigation = OverworldManager.Instance.PlayerInventory.GetMitigation();
            }

            Invoke("RunGame", START_DELAY);
            playerHealthText.enabled = false;
            enemyHealthText.enabled = false;
        }

        private void FixedUpdate()
        {
            if (gameRunning)
            {
                if(Time.time > lastSpawnTime + SPAWN_INTERVAL)
                {
                    lastSpawnTime = Time.time;
                    SpawnElectricity();
                }

                HandlePlayerMovement();
                HandleLightningMovement();
                timeInGame += Time.deltaTime;
            }
        }

        private void RunGame()
        {
            instructionsText.gameObject.SetActive(false);

            playerHealthText.text = "HP: " + healthRemaining.ToString("F2");
            playerHealthText.enabled = true;

            enemyHealthText.text = "Enemy Health: " + enemyHealth;
            enemyHealthText.enabled = true;

            SpawnCollectible();

            gameRunning = true;
        }

        private void SpawnElectricity()
        {
            GameObject parentObj = new GameObject();
            parentObj.name = "Lightning Wall";

            float leftOrbPosX = UnityEngine.Random.Range(LEFT_ORB_X_SPAWN_POS_MIN, LEFT_ORB_X_SPAWN_POS_MAX);
            GameObject leftOrb = Instantiate(lightningBallInitialObj, new Vector2(leftOrbPosX, 55.0f), Quaternion.identity);
            leftOrb.transform.parent = parentObj.transform;
            leftOrb.name = "Left Orb";
            leftOrb.SetActive(true);

            float dist = UnityEngine.Random.Range(DIST_MIN, DIST_MAX);
            float rightOrbPosX = leftOrbPosX + dist;
            GameObject rightOrb = Instantiate(lightningBallInitialObj, new Vector2(rightOrbPosX, 55.0f), Quaternion.identity);
            rightOrb.transform.parent = parentObj.transform;
            rightOrb.name = "Right Orb";
            rightOrb.SetActive(true);

            GameObject leftOscillator = Instantiate(oscillatorInitialObj, new Vector2(leftOrbPosX - 51.0f, 45.0f), Quaternion.identity);
            leftOscillator.transform.parent = parentObj.transform;
            leftOscillator.name = "Left Oscillar";
            leftOscillator.SetActive(true);

            GameObject rightOscillator = Instantiate(oscillatorInitialObj, new Vector2(rightOrbPosX + 52.5f, 45.0f), Quaternion.identity);
            rightOscillator.transform.parent = parentObj.transform;
            rightOscillator.name = "Right Oscillator";
            rightOscillator.SetActive(true);

            electricObjs.Add(parentObj);
        }

        private void HandleLightningMovement()
        {
            List<GameObject> objsToDelete = new List<GameObject>();
            foreach(GameObject lightningObj in electricObjs)
            {
                lightningObj.transform.position -= new Vector3(0, ELECTRICITY_SPEED, 0);
                if(lightningObj.transform.position.y < -100)
                {
                    objsToDelete.Add(lightningObj);
                }
            }

            foreach(GameObject objToDelete in objsToDelete)
            {
                electricObjs.Remove(objToDelete);
                Destroy(objToDelete);
            }
        }

        private void HandlePlayerMovement()
        {
            Vector3 newPos = this.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPos.y += PLAYER_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                newPos.y -= PLAYER_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                newPos.x -= PLAYER_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                newPos.x += PLAYER_SPEED * Time.deltaTime;
            }

            this.transform.position = newPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 7)
            {
                // enemy attack
                CollideElectricity(other.gameObject);
            }
            else if (other.gameObject.layer == 8)
            {
                // friendly collider
                CollideCollectible(other.gameObject);
            }
        }

        public void OnContinueButtonClicked()
        {
            OverworldManager overworldManager = OverworldManager.Instance;
            if (!overworldManager)
            {
                return;
            }

            if (healthRemaining >= 0)
            {
                overworldManager.CompleteLevel(overworldManager.CurLevel.LevelID);
            }

            overworldManager.StopMusic();
            overworldManager.ReadyScene(true);
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        private void SpawnCollectible()
        {
            int numTries = 0;
            while (numTries < 10)
            {
                float xPos = UnityEngine.Random.Range(COLLECTIBLE_X_MIN, COLLECTIBLE_X_MAX);
                float yPos = UnityEngine.Random.Range(COLLECTIBLE_Y_MIN, COLLECTIBLE_Y_MAX);
                Vector3 locationVec = new Vector3(xPos, yPos, 0);

                float distance = Vector3.Distance(this.transform.position, locationVec);
                if (distance > COLLECTIBLE_SPAWN_DISTANCE)
                {
                    GameObject newCollectible = Instantiate(collectibleInitialObj, locationVec, Quaternion.identity);

                    int spriteIndex = UnityEngine.Random.Range(0, collectibleObjectSprites.Count);
                    newCollectible.GetComponentInChildren<Image>().sprite = collectibleObjectSprites[spriteIndex];

                    newCollectible.transform.SetParent(canvas.transform);
                    newCollectible.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    newCollectible.transform.localPosition = locationVec;

                    newCollectible.SetActive(true);
                    break;
                }

                ++numTries;
            }
        }

        private void CollideElectricity(GameObject otherGO)
        {
            // don't get hit multiple times repeatedly from a big cluster
            bool invuln = invulnExpireTime > Time.time;
            if (Time.time > invulnExpireTime)
            {
                invulnExpireTime = Time.time + INVULN_DURATION;
            }

            if(invuln)
            {
                return;
            }

            // maybe damage the player
            float baseDamage = 20.0f;
            float damageReduction = baseDamage * (mitigation / 100);
            float damageTaken = baseDamage - damageReduction;
            if (damageTaken > 0)
            {
                if (!invuln)
                {
                    healthRemaining -= damageTaken;
                }
            }

            playerHealthText.text = "Player Health: " + healthRemaining.ToString();
            if (healthRemaining < 0)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAudioClip(deathSound);
                }

                playerHealthText.text = "YOU ARE DED";
                gameRunning = false;
                continueButton.SetActive(true);
            }

            int index = UnityEngine.Random.Range(0, hitVFXList.Count);
            Instantiate(hitVFXList[index], gameObject.transform.position, Quaternion.identity);
            FlashColor();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(lightningImpactSound);
            }
        }

        private void CollideCollectible(GameObject otherGO)
        {
            enemyHealth -= COLLECTIBLE_DAMAGE;
            enemyHealthText.text = "Enemy Health: " + enemyHealth;

            if (enemyHealth <= 0)
            {
                gameRunning = false;

                int lootAmount = 500;
                endGameText.text = "Boss time!\nReceived " + lootAmount + " gold";

                if (OverworldManager.Instance != null)
                {
                    OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Gold, lootAmount);
                }

                enemyHealthText.text = "ded";
                continueButton.SetActive(true);
            }
            else
            {
                SpawnCollectible();
            }

            int index = UnityEngine.Random.Range(0, collectVFXList.Count);
            Instantiate(collectVFXList[index], otherGO.GetComponent<BoxCollider2D>().transform.position, Quaternion.identity);
            Destroy(otherGO);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(collectiblePickupSound);
            }
        }

        // flash colors when we get hit
        private void FlashColor()
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.color == Color.red)
                {
                    renderer.color = Color.white;
                }
                else
                {
                    renderer.color = Color.red;
                }
            }

            if (invulnExpireTime > Time.time)
            {
                Invoke("FlashColor", 0.2f);
            }
            else
            {
                // make sure we end up back to normal
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.color = Color.white;
                }
            }
        }
    }
}