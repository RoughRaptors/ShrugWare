using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    // 45 seconds long
    // 0-15s - come from right
    // 15s-30s come from top + right
    // 30s-45s come from bottom + top + right

    public class DodgeFireballsMinigame : Minigame
    {
        [SerializeField]
        GameObject fireballInitObj;

        [SerializeField]
        Text statusText;

        [SerializeField]
        Text timeRemainingText;

        [SerializeField]
        GameObject continueButton;

        const float FIREBALL_X_MIN = -10;
        const float FIREBALL_X_MAX = 125;
        const float FIREBALL_Y_MIN = -30;
        const float FIREBALL_Y_MAX = 40;
        const float FIREBALL_SPEED_MIN = 0.45f;
        const float FIREBALL_SPEED_MAX = 0.75f;

        const float PLAYER_X_MIN = -13;
        const float PLAYER_X_MAX = 115;
        const float PLAYER_Y_MIN = -30;
        const float PLAYER_Y_MAX = 35;

        const float FIREBALL_SPAWN_INTERVAL = 0.75f;
        private float timeSinceLastSpawn = 0.0f;

        private float timeInGame = 0.0f;
        private const float PLAYER_SPEED = 50.0f;

        // float so we can take partial damage via damage mitigation. it's not clean but blegh
        private const float START_HEALTH = 5;
        private float healthRemaining = 5;
        bool gameRunning = false;

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

            gameRunning = true;
            statusText.text = "HP: " + healthRemaining.ToString("F2");
        }

        private void Update()
        {
            if(gameRunning)
            {
                HandlePlayerMovement();
                HandleFireballs();
                timeInGame += Time.deltaTime;
            }

            timeRemainingText.text = "Time Remaining: " + (DataManager.MINIGAME_DURATION_SECONDS - timeInGame).ToString("F2");
            if(gameRunning && timeInGame >= DataManager.MINIGAME_DURATION_SECONDS)
            {
                // out of time, we won
                gameRunning = false;

                int lootAmount = 500;
                statusText.text = "Boss time!\nReceived " + lootAmount + " gold";
                OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Generic, lootAmount);

                continueButton.SetActive(true);
            }
        }

        private void HandleFireballs()
        {
            if (timeSinceLastSpawn >= FIREBALL_SPAWN_INTERVAL)
            {
                timeSinceLastSpawn = 0;

                // cast down to int to be able to cleanly mod 15
                // spawn more at 15s and 30s
                if ((int)timeInGame != 0 && (int)timeInGame % 30 == 0)
                {
                    // spawn top
                    for (int i = 0; i < 10; ++i)
                    {
                        SpawnFireball(FromDirection.FromBottom);
                    }
                }
                else if ((int)timeInGame != 0 && (int)timeInGame % 15 == 0)
                {
                    // spawn bottom
                    for (int i = 0; i < 10; ++i)
                    {
                        SpawnFireball(FromDirection.FromTop);
                    }
                }

                // spawn if enough time has been spent
                if (timeInGame > 30)
                {
                    SpawnFireball(FromDirection.FromBottom);
                }

                if (timeInGame > 15)
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

                Vector3 targetPos = new Vector3(-25, yPos, 0);
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

                // done with it
                if (fireball.fireballObj.transform.position == fireball.targetPos)
                {
                    fireballsList.RemoveAt(i);
                    Destroy(fireball.fireballObj);
                    continue;
                }

                fireball.fireballObj.transform.position =
                    Vector3.MoveTowards(fireball.fireballObj.transform.position, fireball.targetPos, fireball.speed);
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
            float mitigation = 0;
            if (OverworldManager.Instance != null)
            {
                mitigation = OverworldManager.Instance.PlayerInventory.GetMitigation();
            }

            // damage the player
            float damageTaken = 1.0f - (mitigation / 100);
            healthRemaining -= damageTaken;
            statusText.text = "HP: " + healthRemaining.ToString();
            if (healthRemaining < 0)
            {
                statusText.text = "YOU ARE DED";
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
    }
}