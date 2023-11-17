using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

namespace ShrugWare
{
    // 45 seconds long
    // 0-15s - come from right
    // 15s-30s come from top + right
    // 30s-45s come from bottom + top + right

    public class HeiganDanceMinigame : Minigame
    {
        [SerializeField]
        Text statusText;

        [SerializeField]
        Text timeRemainingText;

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

        [SerializeField]
        GameObject startingTileObj;

        private const float TIME_TO_SWITCH_COLORS = 3.5f;
        private float timeSinceLastColorSwitch = 0.0f;
        private float totalTimeInGame = 0.0f;
        private const float PLAYER_SPEED = 50.0f;

        // float so we can take partial damage via damage mitigation
        private const float START_HEALTH = 5;

        private const int NUM_EACH_COLOR = 6;

        private int numGreenTilesLit = 0;
        private int numYellowTilesLit = 0;
        private int numRedTilesLit = 0;

        private List<GameObject> greenTiles = new List<GameObject>();
        private List<GameObject> yellowTiles = new List<GameObject>();
        private List<GameObject> redTiles = new List<GameObject>();

        private float healthRemaining = 5;
        private bool gameRunning = false;
        bool exploited = false;

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

            // just start everything as green
            foreach(GameObject tileObj in tileObjs)
            {
                ++numGreenTilesLit;
                tileObj.GetComponent<MeshRenderer>().material = greenMaterial;
                greenTiles.Add(tileObj);
            }

            // turn numYellowTilesLit from green to yellow
            for (; numYellowTilesLit < NUM_EACH_COLOR; ++numYellowTilesLit)
            {
                --numGreenTilesLit;
                int randomGreenTileIndex = Random.Range(0, greenTiles.Count);
                GameObject randomTile = tileObjs[randomGreenTileIndex];
                randomTile.GetComponent<MeshRenderer>().material = yellowMaterial;
                greenTiles.Remove(randomTile);
                yellowTiles.Add(randomTile);
            }

            // turn numRedTilesLit from yellow to red
            for(; numRedTilesLit < NUM_EACH_COLOR; ++numRedTilesLit)
            {
                --numYellowTilesLit;
                int randomYellowTileIndex = Random.Range(0, yellowTiles.Count);
                GameObject randomTile = tileObjs[randomYellowTileIndex];
                randomTile.GetComponent<MeshRenderer>().material = redMaterial;
                yellowTiles.Remove(randomTile);
                redTiles.Add(randomTile);
            }

            // change the spawn tile to green
            MakeTileGreen(startingTileObj);
        }

        private void FixedUpdate()
        {
            if(gameRunning)
            {
                HandlePlayerMovement();

                // this doesn't reset
                totalTimeInGame += Time.deltaTime;

                // this does
                timeSinceLastColorSwitch += Time.deltaTime;
                if(timeSinceLastColorSwitch >= TIME_TO_SWITCH_COLORS)
                {
                    timeSinceLastColorSwitch = 0;
                    RotateTileColors();
                }
            }

            if (!exploited)
            {
                timeRemainingText.text = "Time Remaining: " + (microgameTimeDuration - totalTimeInGame).ToString("F2");
            }

            if(gameRunning && totalTimeInGame >= microgameTimeDuration)
            {
                // out of time, we won
                gameRunning = false;

                int lootAmount = 500;
                endGameText.text = "Boss time!\nReceived " + lootAmount + " gold";

                if (OverworldManager.Instance != null)
                {
                    OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Generic, lootAmount);
                }

                continueButton.SetActive(true);
            }
        }

        private void HandlePlayerMovement()
        {
            // laser handles out of bounds. can't go past it without dying
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

        // funny exploit if you go around the lasers. there's barely enough room
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Barrier")
            {
                healthRemaining = 0;
                exploited = true;
                statusText.text = "Quack";
                timeRemainingText.text = "Quack";
            }
        }

        private void OnTriggerStay(Collider other)
        {
            MeshRenderer meshRenderer = other.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.material.color == Color.red)
            {
                DamagePlayer(other.gameObject);
            }
        }

        private void DamagePlayer(GameObject collideObj)
        {
            float mitigation = 0;
            if (OverworldManager.Instance != null)
            {
                mitigation = OverworldManager.Instance.PlayerInventory.GetMitigation();
            }

            // everything ticks damage so make it dependent on time in the bad area
            float damageTaken = (1.0f - (mitigation / 100)) * Time.deltaTime;
            if (collideObj.tag == "Laser")
            {
                // don't tick from the laser, insta-kill to prevent going out of bounds
                damageTaken = (1.0f - (mitigation / 100));
            }

            healthRemaining -= damageTaken;
            statusText.text = "HP: " + healthRemaining.ToString("F2");
            if (healthRemaining < 0)
            {
                statusText.text = "YOU ARE DED";
                gameRunning = false;
                continueButton.SetActive(true);
            }

            if (exploited)
            {
                statusText.text = "Quack";
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
    }
}