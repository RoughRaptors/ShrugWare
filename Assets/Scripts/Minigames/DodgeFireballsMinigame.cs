using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;

namespace ShrugWare
{
    // need to collect x collectibles
    // 0-10s - come from right
    // 10s-20s come from top + right
    // 20s-30s come from top + right + bottom

    public class DodgeFireballsMinigame : Minigame
    {
        [SerializeField]
        GameObject fireballInitObj;

        [SerializeField]
        GameObject collectibleInitObj;

        [SerializeField]
        Text playerHealthText;

        [SerializeField]
        GameObject continueButton;

        [SerializeField]
        Text endGameText;

        [SerializeField]
        GameObject topIndicatorObj;

        [SerializeField]
        GameObject bottomIndicatorObj;

        [SerializeField]
        Text enemyHealthText;

        // just to have some variation have some different sprites that spawn in
        [SerializeField]
        List<Sprite> collectibleObjectSprites = new List<Sprite>();

        private const float FIREBALL_X_MIN = -25;
        private const float FIREBALL_X_MAX = 125;
        private const float FIREBALL_Y_MIN = -40;
        private const float FIREBALL_Y_MAX = 50;
        private const float FIREBALL_SPEED_MIN = 45;
        private const float FIREBALL_SPEED_MAX = 75;

        private const float PLAYER_X_MIN = -13;
        private const float PLAYER_X_MAX = 115;
        private const float PLAYER_Y_MIN = -30;
        private const float PLAYER_Y_MAX = 35;

        private const int ENEMY_START_HEALTH = 100;
        private float enemyHealth = ENEMY_START_HEALTH;

        private const float COLLECTIBLE_SPAWN_DISTANCE = 35;
        private const int COLLECTIBLE_DAMAGE = 10;
        private const float COLLECTIBLE_X_MIN = -5;
        private const float COLLECTIBLE_X_MAX = 110;
        private const float COLLECTIBLE_Y_MIN = -25;
        private const float COLLECTIBLE_Y_MAX = 30;

        private const float FIREBALL_SPAWN_INTERVAL = 0.9f;
        private float timeSinceLastSpawn = 0.0f;
        private bool hasSpawnedTopIndicator = false;
        private bool hasSpawnedBottomIndicator = false;

        private float timeInGame = 0.0f;
        private const float PLAYER_SPEED = 50.0f;

        // float so we can take partial damage via damage mitigation
        private const float START_HEALTH = 5;
        private float healthRemaining = 5;
        private bool gameRunning = false;

        private const int TOP_PATTERN_SPAWN_TIME = 5;
        private const int BOTTOM_PATTERN_SPAWN_TIME = 10;

        private enum FromDirection
        {
            FromRight = 0,
            FromTop,
            FromBottom
        }

        protected struct Fireball
        {
            public GameObject fireballObj;
            public Vector3 targetPos;
            public float speed;
        }

        private List<Fireball> fireballsList = new List<Fireball>();

        private void Awake()
        {
            continueButton.SetActive(false);
        }

        private new void Start()
        {
            base.Start();
            healthRemaining = START_HEALTH + healthToAdd;

            // initial wave
            for (int i = 0; i < 10; ++i)
            {
                SpawnFireball(FromDirection.FromRight);
            }

            SpawnCollectible();

            gameRunning = true;
            playerHealthText.text = "HP: " + healthRemaining.ToString("F2");
        }

        private void FixedUpdate()
        {
            if(gameRunning)
            {
                HandlePlayerMovement();
                HandleFireballs();
                timeInGame += Time.deltaTime;

                if (!hasSpawnedBottomIndicator && timeInGame >= BOTTOM_PATTERN_SPAWN_TIME)
                {
                    bottomIndicatorObj.SetActive(true);
                    hasSpawnedBottomIndicator = true;
                    Invoke("DeactivateBottomIndicator", 2.5f);
                }
                else if (!hasSpawnedTopIndicator && timeInGame >= TOP_PATTERN_SPAWN_TIME)
                {
                    topIndicatorObj.SetActive(true);
                    hasSpawnedTopIndicator = true;
                    Invoke("DeactivateTopIndicator", 2.5f);
                }
            }
        }

        private void HandleFireballs()
        {
            if (timeSinceLastSpawn >= FIREBALL_SPAWN_INTERVAL)
            {
                timeSinceLastSpawn = 0;

                // spawn more at 20s and 10s
                if ((int)timeInGame != 0 && (int)timeInGame % BOTTOM_PATTERN_SPAWN_TIME == 0)
                {
                    // spawn bottom
                    for (int i = 0; i < 10; ++i)
                    {
                        SpawnFireball(FromDirection.FromBottom);
                    }
                }
                else if ((int)timeInGame != 0 && (int)timeInGame % TOP_PATTERN_SPAWN_TIME == 0)
                {
                    // spawn top
                    for (int i = 0; i < 10; ++i)
                    {
                        SpawnFireball(FromDirection.FromTop);
                    }
                }

                // spawn if enough time has been spent
                if (timeInGame >= BOTTOM_PATTERN_SPAWN_TIME)
                {
                    SpawnFireball(FromDirection.FromBottom);
                }

                if (timeInGame >= TOP_PATTERN_SPAWN_TIME)
                {
                    SpawnFireball(FromDirection.FromTop);
                }

                // always spawn from here
                SpawnFireball(FromDirection.FromRight);
            }

            UpdateFireballs();
            timeSinceLastSpawn += Time.deltaTime;
        }

        private void SpawnFireball(FromDirection fromDir)
        {
            Fireball newFireball = new Fireball();
            GameObject newFireballObj = Instantiate(fireballInitObj);
            newFireball.fireballObj = newFireballObj;
            newFireballObj.gameObject.SetActive(true);

            float speed = UnityEngine.Random.Range(FIREBALL_SPEED_MIN, FIREBALL_SPEED_MAX);
            newFireball.speed = speed;

            float xPos = 0.0f;
            float yPos = 0.0f;
            if (fromDir == FromDirection.FromRight)
            {
                xPos = FIREBALL_X_MAX;
                yPos = UnityEngine.Random.Range(FIREBALL_Y_MIN, FIREBALL_Y_MAX);
                newFireballObj.transform.position = new Vector3(xPos, yPos, 0);

                Vector3 targetPos = new Vector3(-50, yPos, 0);
                newFireball.targetPos = targetPos;
            }
            else if (fromDir == FromDirection.FromTop)
            {
                xPos = UnityEngine.Random.Range(FIREBALL_X_MIN, FIREBALL_X_MAX);
                yPos = 50;
                newFireballObj.transform.position = new Vector3(xPos, yPos, 0);

                Vector3 targetPos = new Vector3(xPos, -40, 0);
                newFireball.targetPos = targetPos;
            }
            else if (fromDir == FromDirection.FromBottom)
            {
                xPos = UnityEngine.Random.Range(FIREBALL_X_MIN, FIREBALL_X_MAX);
                yPos = -40;
                newFireballObj.transform.position = new Vector3(xPos, yPos, 0);

                Vector3 targetPos = new Vector3(xPos, 50, 0);
                newFireball.targetPos = targetPos;
            }

            fireballsList.Add(newFireball);
        }

        private void UpdateFireballs()
        {
            for(int i = 0; i < fireballsList.Count; ++i)
            {
                Fireball fireball = fireballsList[i];

                // not sure why but pressing 'z' triggers this to be null and break
                if(fireball.fireballObj == null)
                {
                    break;
                }

                // done with it
                if (fireball.fireballObj.transform.position == fireball.targetPos)
                {
                    fireballsList.RemoveAt(i);
                    Destroy(fireball.fireballObj);
                    continue;
                }

                fireball.fireballObj.transform.position =
                    Vector3.MoveTowards(fireball.fireballObj.transform.position, fireball.targetPos, fireball.speed * Time.deltaTime);
            }
        }

        private void HandlePlayerMovement()
        {
            Vector3 newPos = this.transform.position;
            if (Input.GetKey(KeyCode.W) && transform.position.y < PLAYER_Y_MAX)
            {
                newPos.y += PLAYER_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S) && transform.position.y > PLAYER_Y_MIN)
            {
                newPos.y -= PLAYER_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A) && transform.position.x > PLAYER_X_MIN)
            {
                newPos.x -= PLAYER_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D) && transform.position.x < PLAYER_X_MAX)
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
                CollideFireball(other);
            }
            else if(other.gameObject.layer == 8)
            {
                // friendly collider
                CollideCollectible(other);
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
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        private void DeactivateTopIndicator()
        {
            topIndicatorObj.SetActive(false);
        }

        private void DeactivateBottomIndicator()
        {
            bottomIndicatorObj.SetActive(false);
        }

        private void SpawnCollectible()
        {
            int numTries = 0;
            while(numTries < 10)
            {
                float xPos = UnityEngine.Random.Range(COLLECTIBLE_X_MIN, COLLECTIBLE_X_MAX);
                float yPos = UnityEngine.Random.Range(COLLECTIBLE_Y_MIN, COLLECTIBLE_Y_MAX);
                Vector3 locationVec = new Vector3(xPos, yPos, 0);

                float distance = Vector3.Distance(this.transform.position, locationVec);
                if (distance > COLLECTIBLE_SPAWN_DISTANCE)
                {
                    GameObject newCollectible = Instantiate(collectibleInitObj);
                    newCollectible.transform.position = locationVec;

                    int spriteIndex = UnityEngine.Random.Range(0, collectibleObjectSprites.Count);
                    newCollectible.GetComponent<SpriteRenderer>().sprite = collectibleObjectSprites[spriteIndex];

                    newCollectible.SetActive(true);
                    break;
                }

                ++numTries;
            }
        }

        private void CollideFireball(Collider other)
        {
            float mitigation = 0;
            if (OverworldManager.Instance != null)
            {
                mitigation = OverworldManager.Instance.PlayerInventory.GetMitigation();
            }

            // damage the player
            float damageTaken = 1.0f - (mitigation / 100);
            healthRemaining -= damageTaken;
            playerHealthText.text = "Player Health: " + healthRemaining.ToString();
            if (healthRemaining < 0)
            {
                playerHealthText.text = "YOU ARE DED";
                gameRunning = false;
                continueButton.SetActive(true);
            }

            // destroy the fireball and remove it from the list
            for (int i = 0; i < fireballsList.Count; ++i)
            {
                Fireball fireball = fireballsList[i];
                if (fireball.fireballObj == other.gameObject)
                {
                    fireballsList.RemoveAt(i);
                    Destroy(fireball.fireballObj);
                    break;
                }
            }
        }

        private void CollideCollectible(Collider other)
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
                    OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Generic, lootAmount);
                }

                enemyHealthText.text = "ded";

                continueButton.SetActive(true);
            }
            else
            {
                SpawnCollectible();
            }

            Destroy(other.gameObject);
        }
    }
}