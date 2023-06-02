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

    public class ShootAsteroidsMinigame : Minigame
    {
        [SerializeField]
        GameObject asteroidInitObj;

        [SerializeField]
        GameObject bulletInitObj;

        [SerializeField]
        Text statusText;

        [SerializeField]
        Text timeRemainingText;

        [SerializeField]
        GameObject continueButton;

        private float timeInGame = 0.0f;
        private const float PLAYER_SPEED = 12.5f;
        private const float TURN_SPEED = 7.5f;

        // float so we can take partial damage via damage mitigation. it's not clean but blegh
        private const float START_HEALTH = 5;
        private const float SHOOT_COOLDOWN = 0.15f;
        private float timeSinceLastShot = 0.0f;

        private float healthRemaining = 5;
        bool gameRunning = false;

        const int NUM_ASTEROIDS = 15;
        const float ASTEROID_X_MIN = -10;
        const float ASTEROID_X_MAX = 110;
        const float ASTEROID_Y_MIN = -30;
        const float ASTEROID_Y_MAX = 40;

        const float PLAYER_X_MIN = -25;
        const float PLAYER_X_MAX = 110;
        const float PLAYER_Y_MIN = -30;
        const float PLAYER_Y_MAX = 50;
        const float PLAYER_BUFFER = 5.0f;

        float timeScale = 1.0f;

        // keep track of this so we can modify it when our bullets collide in ShootAsteroidsMinigameAsteroid
        public static int NumAsteroidsDestroyed = 0;

        private void Awake()
        {
            continueButton.SetActive(false);
        }

        private new void Start()
        {
            base.Start();
            this.transform.position = new Vector3(50, 0, 0);
            healthRemaining = START_HEALTH + healthToAdd;
            NumAsteroidsDestroyed = 0;

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                SpawnAsteroid();
            }

            gameRunning = true;
            statusText.text = "HP: " + healthRemaining.ToString("F2");
        }

        private void FixedUpdate()
        {
            if (gameRunning)
            {
                HandlePlayerMovement();
                timeInGame += Time.deltaTime;
            }

            timeRemainingText.text = "Time Remaining: " + (DataManager.MINIGAME_DURATION_SECONDS - timeInGame).ToString("F2");
            if (timeInGame >= DataManager.MINIGAME_DURATION_SECONDS)
            {
                // out of time, we lost
                gameRunning = false;
                statusText.text = "NO TIME LEFT!";
                continueButton.SetActive(true);
            }

            // destroyed all
            if(gameRunning && NumAsteroidsDestroyed == NUM_ASTEROIDS)
            {
                gameRunning = false;
                int lootAmount = 500;
                OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Generic, lootAmount);
                statusText.text = "Boss time!\nReceived " + lootAmount + " gold";
                continueButton.SetActive(true);
            }
        }

        private void SpawnAsteroid()
        {
            ShootAsteroidsMinigameAsteroid.Asteroid newAsteroid = new ShootAsteroidsMinigameAsteroid.Asteroid();
            GameObject newAsteroidObj = Instantiate(asteroidInitObj);
            newAsteroidObj.gameObject.SetActive(true);
            newAsteroid.asteroidObj = newAsteroidObj;

            float asteroidXPos = Random.Range(ASTEROID_X_MIN, ASTEROID_X_MAX);
            float asteroidYPos = Random.Range(ASTEROID_Y_MIN, ASTEROID_Y_MAX);
            Vector3 newPos = new Vector3(asteroidXPos, asteroidYPos, 0);

            // make sure we're not spawning this asteroid on top of the player, it will hurt us
            if (Vector3.Distance(this.transform.position, newPos) > 10)
            {
                // let's just count it as destroyed in this edge case
                newAsteroid.asteroidObj.transform.position = new Vector3(asteroidXPos, asteroidYPos, 0);
            }
            else
            {
                ++NumAsteroidsDestroyed;
            }
        }

        // use physics instead of just moving the transform so we can have momentum
        private void HandlePlayerMovement()
        {
            timeSinceLastShot += Time.deltaTime;

            // give thruster in facing direction
            if (Input.GetKey(KeyCode.W))
            {
                this.GetComponent<Rigidbody>().AddRelativeForce(PLAYER_SPEED, 0.0f, 0);
            }

            // rotate left/right
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, 0, TURN_SPEED);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, 0, -TURN_SPEED);
            }

            // shoot
            if (timeSinceLastShot >= SHOOT_COOLDOWN &&
                (Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Mouse0)
                || Input.GetKeyDown(KeyCode.Mouse1)))
            {
                timeSinceLastShot = 0;
                Shoot();
            }

            // edges of map. teleport to the other side
            if(transform.position.x < -21)
            {
                transform.position = new Vector3(120, transform.position.y, 0);
            }
            else if(transform.position.x > 120)
            {
                transform.position = new Vector3(-21, transform.position.y, 0);
            }
            else if (transform.position.y < -35)
            {
                transform.position = new Vector3(transform.position.x, 45, 0);
            }
            else if (transform.position.y > 45)
            {
                transform.position = new Vector3(transform.position.x, -35, 0);
            }
        }

        private void Shoot()
        {
            // how many bullets should we fire based on our armor (shoot extra bullets instead of mitigation)
            int numBullets = 5;
            if (OverworldManager.Instance != null)
            {
                numBullets = Mathf.Max((int)OverworldManager.Instance.PlayerInventory.GetMitigation() / 10, 1);
            }

            // lazy
            if (numBullets == 5)
            {
                GameObject newBullet1 = Instantiate(bulletInitObj);
                newBullet1.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
                newBullet1.transform.forward = this.transform.forward;
                newBullet1.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet1.SetActive(true);

                GameObject newBullet2 = Instantiate(bulletInitObj);
                newBullet2.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
                newBullet2.transform.forward = this.transform.forward;
                newBullet2.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet2.SetActive(true);

                GameObject newBullet3 = Instantiate(bulletInitObj);
                newBullet3.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
                newBullet3.transform.forward = this.transform.forward;
                newBullet3.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet3.SetActive(true);

                GameObject newBullet4 = Instantiate(bulletInitObj);
                newBullet4.transform.position = new Vector3(this.transform.position.x - 0.5f, this.transform.position.y + 1 + this.transform.position.z);
                newBullet4.transform.forward = this.transform.forward;
                newBullet4.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet4.SetActive(true);

                GameObject newBullet5 = Instantiate(bulletInitObj);
                newBullet5.transform.position = new Vector3(this.transform.position.x + 0.5f, this.transform.position.y + 1, this.transform.position.z);
                newBullet5.transform.forward = this.transform.forward;
                newBullet5.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet5.SetActive(true);

                GameObject newBullet6 = Instantiate(bulletInitObj);
                newBullet6.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.9f, this.transform.position.z);
                newBullet6.transform.forward = this.transform.forward;
                newBullet6.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet6.SetActive(true);

                // extra bullet for set bonus, shoot backwards
                GameObject backwardsBullet = Instantiate(bulletInitObj);
                backwardsBullet.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1.9f, this.transform.position.z);
                backwardsBullet.transform.forward = -this.transform.forward;
                backwardsBullet.GetComponent<Rigidbody>().velocity = -this.transform.right * 15.0f;
                backwardsBullet.SetActive(true);
            }
            else if (numBullets == 4)
            {
                GameObject newBullet1 = Instantiate(bulletInitObj);
                newBullet1.transform.position = new Vector3(this.transform.position.x - 1.5f, this.transform.position.y, this.transform.position.z);
                newBullet1.transform.forward = this.transform.forward;
                newBullet1.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet1.SetActive(true);

                GameObject newBullet2 = Instantiate(bulletInitObj);
                newBullet2.transform.position = new Vector3(this.transform.position.x - 0.5f, this.transform.position.y, this.transform.position.z);
                newBullet2.transform.forward = this.transform.forward;
                newBullet2.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet2.SetActive(true);

                GameObject newBullet3 = Instantiate(bulletInitObj);
                newBullet3.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
                newBullet3.transform.forward = this.transform.forward;
                newBullet3.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet3.SetActive(true);

                GameObject newBullet4 = Instantiate(bulletInitObj);
                newBullet4.transform.position = new Vector3(this.transform.position.x + 2, this.transform.position.y, this.transform.position.z);
                newBullet4.transform.forward = this.transform.forward;
                newBullet4.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet4.SetActive(true);
            }
            else if (numBullets == 3)
            {
                GameObject newBullet1 = Instantiate(bulletInitObj);
                newBullet1.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
                newBullet1.transform.forward = this.transform.forward;
                newBullet1.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet1.SetActive(true);

                GameObject newBullet2 = Instantiate(bulletInitObj);
                newBullet2.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
                newBullet2.transform.forward = this.transform.forward;
                newBullet2.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet2.SetActive(true);

                GameObject newBullet3 = Instantiate(bulletInitObj);
                newBullet3.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
                newBullet3.transform.forward = this.transform.forward;
                newBullet3.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet3.SetActive(true);
            }
            else if (numBullets == 2)
            {
                GameObject newBullet1 = Instantiate(bulletInitObj);
                newBullet1.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
                newBullet1.transform.forward = this.transform.forward;
                newBullet1.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet1.SetActive(true);

                GameObject newBullet2 = Instantiate(bulletInitObj);
                newBullet2.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
                newBullet2.transform.forward = this.transform.forward;
                newBullet2.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet2.SetActive(true);
            }
            else
            {
                GameObject newBullet = Instantiate(bulletInitObj);
                newBullet.transform.position = this.transform.position;
                newBullet.transform.forward = this.transform.forward;
                newBullet.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet.SetActive(true);
            }

            // speed up the game per bullet shot
            timeScale += 0.01f;
            Time.timeScale = timeScale;
        }

        // we physically collided with something
        private void OnTriggerEnter(Collider other)
        {
            // don't hit our own bullets
            if(other.tag == "Bullet")
            {
                return;
            }

            // no mitigation for now, this is a dps based minigame
            float mitigation = 0;
            if (OverworldManager.Instance != null)
            {
                //mitigation = OverworldManager.Instance.PlayerInventory.GetMitigation();
            }

            // damage the player
            float damageTaken = 1.0f - (mitigation / 100);
            healthRemaining -= damageTaken;
            statusText.text = "HP: " + healthRemaining.ToString();
            if (healthRemaining < 0)
            {
                statusText.text = "Le DED!";
                gameRunning = false;
                continueButton.SetActive(true);
            }

            // we died
            if (healthRemaining < 0)
            {
                gameRunning = false;
                statusText.text = "Le DED!";
                Destroy(other.gameObject);
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

            // set this back
            Time.timeScale = 1.0f;
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }
}