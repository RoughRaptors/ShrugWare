using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    public class HeiganDanceMinigame : Minigame
    {
        [SerializeField]
        Text statusText;

        [SerializeField]
        Text enemyHealthText;

        [SerializeField]
        GameObject continueButton;

        [SerializeField]
        Text endGameText;

        [SerializeField]
        Material greenMaterial;

        [SerializeField]
        Material yellowMaterial;

        [SerializeField]
        Material redMaterial;

        [SerializeField]
        GameObject[] tileObjs;

        // use this to make sure the player spawns on a green tile
        [SerializeField]
        GameObject startingTileObj;

        // just to have some variation have some different sprites that spawn in
        [SerializeField]
        List<Sprite> collectibleObjectSprites = new List<Sprite>();

        [SerializeField]
        GameObject collectibleObj;

        private const float TIME_TO_SWITCH_COLORS = 3.5f;
        private const float PLAYER_SPEED = 50.0f;
        private float timeSinceLastColorSwitch = 0.0f;
        private float totalTimeInGame = 0.0f;

        // float so we can take partial damage via damage mitigation
        private const float START_HEALTH = 5;

        private const float COLLECTIBLE_SPAWN_DISTANCE = 30;
        private const int COLLECTIBLE_DAMAGE = 10;
        private const int ENEMY_START_HEALTH = 100;
        private float enemyHealth = ENEMY_START_HEALTH;

        private const int NUM_EACH_COLOR = 6;

        private List<GameObject> greenTiles = new List<GameObject>();
        private List<GameObject> yellowTiles = new List<GameObject>();
        private List<GameObject> redTiles = new List<GameObject>();

        private float healthRemaining = 5;
        private bool gameRunning = false;

        private float laserInvulnExpireTime = 0.0f;
        private const float LASER_INVULN_DURATION = 0.85f;

        private float mitigation = 0.0f;

        private void Awake()
        {
            continueButton.SetActive(false);
        }
        
        protected void OnEnable()
        {
            PlayerCollider.OnBadCollision += PlayerHit;
        }

        protected void OnDisable()
        {
            PlayerCollider.OnBadCollision -= PlayerHit;
        }

        private new void Start()
        {
            base.Start();
            healthRemaining = START_HEALTH + healthToAdd;

            gameRunning = true;
            statusText.text = "HP: " + healthRemaining.ToString("F2");

            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.PlayerInventory.RecalculateStats();
                mitigation = OverworldManager.Instance.PlayerInventory.GetMitigation();
            }

            // just start everything as green
            // then randomly pick tiles to change
            foreach (GameObject tileObj in tileObjs)
            {
                MakeTileGreen(tileObj);
                greenTiles.Add(tileObj);
            }

            // turn NUM_EACH_COLOR to yellow
            int numIterations = 0;
            while (numIterations < NUM_EACH_COLOR)
            {
                ++numIterations;
                
                int randomTileIndex = Random.Range(0, greenTiles.Count);
                GameObject randomTile = greenTiles[randomTileIndex];
                MakeTileYellow(randomTile);
                greenTiles.Remove(randomTile);
                yellowTiles.Add(randomTile);
            }

            // turn NUM_EACH_COLOR to red
            numIterations = 0;
            while (numIterations < NUM_EACH_COLOR)
            {
                ++numIterations;

                int randomTileIndex = Random.Range(0, greenTiles.Count);
                GameObject randomTile = greenTiles[randomTileIndex];
                MakeTileRed(randomTile);
                greenTiles.Remove(randomTile);
                redTiles.Add(randomTile);
            }

            // change the spawn tile to green and remove it if it's yellow/red
            MakeTileGreen(startingTileObj);
            if (!greenTiles.Contains(startingTileObj))
            {
                greenTiles.Add(startingTileObj);
                yellowTiles.Remove(startingTileObj);
                redTiles.Remove(startingTileObj);
            }

            SpawnCollectible();
        }

        private void FixedUpdate()
        {
            if(gameRunning)
            {
                HandlePlayerMovement();

                // this doesn't reset
                totalTimeInGame += Time.fixedDeltaTime;

                // this does reset
                timeSinceLastColorSwitch += Time.fixedDeltaTime;
                if(timeSinceLastColorSwitch >= TIME_TO_SWITCH_COLORS)
                {
                    timeSinceLastColorSwitch = 0;
                    RotateTileColors();
                }
            }
        }

        private void WinGame()
        {
            gameRunning = false;

            int lootAmount = 500;
            endGameText.text = "Boss time!\nReceived " + lootAmount + " gold";

            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Gold, lootAmount);
            }
            
            continueButton.SetActive(true);
        }

        private void HandlePlayerMovement()
        {
            Vector3 newPos = this.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPos.y += PLAYER_SPEED * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                newPos.y -= PLAYER_SPEED * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                newPos.x -= PLAYER_SPEED * Time.fixedDeltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                newPos.x += PLAYER_SPEED * Time.fixedDeltaTime;
            }

            bool invuln = laserInvulnExpireTime > Time.time;
            if (!invuln)
            {
                transform.position = newPos;
            }
        }

        // funny exploit if you go around the lasers. there's barely enough room
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Collectible")
            {
                Destroy(other.gameObject);

                enemyHealth -= COLLECTIBLE_DAMAGE;
                enemyHealthText.text = "Enemy Health: " + enemyHealth.ToString();
                if (enemyHealth <= 0)
                {
                    gameRunning = false;
                    WinGame();
                }
                else
                {                    
                    SpawnCollectible();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (gameRunning)
            {
                MeshRenderer meshRenderer = other.gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.material.color == Color.red)
                {
                    DamagePlayer(other.gameObject);
                }
            }
        }

        private void DamagePlayer(GameObject collideObj)
        {
            float damageTaken = 0.0f;
            bool invuln = laserInvulnExpireTime > Time.time;
            if (collideObj.tag == "Laser")
            {
                // don't get hit multiple times from a laser if you keep moving towards it after the knockback
                if (Time.time > laserInvulnExpireTime)
                {
                    laserInvulnExpireTime = Time.time + LASER_INVULN_DURATION;
                }

                if (!invuln)
                {
                    damageTaken = 1.0f;
                    FlashColor();
                }

                Vector3 targetPos = transform.position;
                if (collideObj.transform.parent.name == "Top Laser")
                {
                    targetPos = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
                }
                else if (collideObj.transform.parent.name == "Bottom Laser")
                {
                    targetPos = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
                }
                else if (collideObj.transform.parent.name == "Left Laser")
                {
                    targetPos = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
                }
                else if (collideObj.transform.parent.name == "Right Laser")
                {
                    targetPos = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
                }

                GetComponent<Rigidbody>().MovePosition(targetPos);
            }
            else
            {
                // this ticks damage so make it dependent on time in the bad area
                if (!invuln)
                {
                    damageTaken = (1.0f - (mitigation / 100)) * Time.fixedDeltaTime;
                }
            }

            healthRemaining -= damageTaken;
            statusText.text = "HP: " + healthRemaining.ToString("F2");
            CheckAndHandleDeath();
        }

        private void CheckAndHandleDeath()
        {
            if (healthRemaining <= 0)
            {
                statusText.text = "YOU ARE DED";
                gameRunning = false;
                continueButton.SetActive(true);
            }
        }

        public void OnContinueButtonClicked()
        {
            OverworldManager overworldManager = OverworldManager.Instance;
            if (!overworldManager)
            {
                return;
            }

            if (healthRemaining > 0)
            {
                overworldManager.CompleteLevel(overworldManager.CurLevel.LevelID);
            }

            overworldManager.StopMusic();
            overworldManager.ReadyScene(true);
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        private void PlayerHit(GameObject gameObj)
        {
            DamagePlayer(gameObj);
        }

        private void RotateTileColors()
        {
            // keep track of which tile's we've modified so we don't change them multiple times at once
            List<GameObject> modifiedTiles = new List<GameObject>();

            // green -> yellow
            foreach (GameObject greenTile in greenTiles)
            {
                // it's always fine to do this
                MakeTileYellow(greenTile);
                yellowTiles.Add(greenTile);
                modifiedTiles.Add(greenTile);
            }

            // our tiles are now yellow, remove from green
            foreach (GameObject tileToRemove in yellowTiles)
            {
                greenTiles.Remove(tileToRemove);
            }

            // yellow -> red
            foreach (GameObject yellowTile in yellowTiles)
            {
                // don't change if this just turned from green
                if (!modifiedTiles.Contains(yellowTile))
                {
                    MakeTileRed(yellowTile);
                    redTiles.Add(yellowTile);
                    modifiedTiles.Add(yellowTile);
                }
            }

            // our tiles are now red, remove from yellow
            foreach (GameObject tileToRemove in redTiles)
            {
                yellowTiles.Remove(tileToRemove);
            }

            // red -> green
            foreach (GameObject redTile in redTiles)
            {
                // don't change if this just turned from yellow
                if (!modifiedTiles.Contains(redTile))
                {
                    MakeTileGreen(redTile);
                    greenTiles.Add(redTile);
                    modifiedTiles.Add(redTile);
                }
            }

            // our tiles are now green, remove from red
            foreach (GameObject tileToRemove in greenTiles)
            {
                redTiles.Remove(tileToRemove);
            }
        }

        private void MakeTileGreen(GameObject tileObj)
        {
            tileObj.GetComponent<MeshRenderer>().material = greenMaterial;
        }

        private void MakeTileYellow(GameObject tileObj)
        {
            tileObj.GetComponent<MeshRenderer>().material = yellowMaterial;
        }

        private void MakeTileRed(GameObject tileObj)
        {
            tileObj.GetComponent<MeshRenderer>().material = redMaterial;
        }

        private void SpawnCollectible()
        {
            int numFails = 0;
            while(numFails < 10)
            {
                int tileIndex = UnityEngine.Random.Range(0, tileObjs.Length);
                float distance = Vector3.Distance(this.transform.position, tileObjs[tileIndex].transform.position);
                if (distance >= COLLECTIBLE_SPAWN_DISTANCE && numFails < 10)
                {
                    GameObject newCollectible = Instantiate(collectibleObj);
                    newCollectible.SetActive(true);

                    int spriteIndex = UnityEngine.Random.Range(0, collectibleObjectSprites.Count);
                    newCollectible.GetComponent<SpriteRenderer>().sprite = collectibleObjectSprites[spriteIndex];

                    // pick a random tile to spawn on
                    newCollectible.transform.position = new Vector3(tileObjs[tileIndex].transform.position.x, tileObjs[tileIndex].transform.position.y, -1);
                    break;
                }

                ++numFails;
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

            if (laserInvulnExpireTime > Time.time)
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