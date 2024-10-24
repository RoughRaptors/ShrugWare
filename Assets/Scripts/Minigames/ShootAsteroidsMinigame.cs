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

        [SerializeField]
        Text endGameText;

        [SerializeField]
        GameObject bulletSpawn1;

        [SerializeField]
        GameObject bulletSpawn2;

        private float timeInGame = 0.0f;
        private const float PLAYER_SPEED = 12.5f;
        private const float TURN_SPEED = 7.5f;

        // float so we can take partial damage via damage mitigation. it's not clean but blegh
        private const float SHOOT_COOLDOWN = 0.65f;
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
            healthRemaining = DataManager.PLAYER_START_HEALTH_MINIGAME + healthToAdd;
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

            timeRemainingText.text = "Time Remaining: " + (minigameDuration - timeInGame).ToString("F2");
            if (timeInGame >= minigameDuration)
            {
                // out of time, we lost
                gameRunning = false;
                statusText.text = "NO TIME LEFT!";
                continueButton.SetActive(true);
            }

            // destroyed all
            if(gameRunning && NumAsteroidsDestroyed >= NUM_ASTEROIDS)
            {
                gameRunning = false;
                int lootAmount = 500;
                OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Gold, lootAmount);
                endGameText.text = "Boss time!\nReceived " + lootAmount + " gold";
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
                newAsteroid.asteroidObj.transform.position = new Vector3(asteroidXPos, asteroidYPos, 0);
            }
            else
            {
                // spawn a new one since this wasn't far enough from the player
                Destroy(newAsteroidObj.gameObject);
                SpawnAsteroid();
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
                (Input.GetKey(KeyCode.Space)
                || Input.GetKey(KeyCode.Mouse0)
                || Input.GetKey(KeyCode.Mouse1)))
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
                // extra bullet for set bonus, shoot backwards
                GameObject backwardsBullet = Instantiate(bulletInitObj);
                backwardsBullet.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1.9f, this.transform.position.z);
                backwardsBullet.transform.forward = -this.transform.forward;
                backwardsBullet.GetComponent<Rigidbody>().velocity = -this.transform.right * 15.0f;
                backwardsBullet.SetActive(true);
            }

            if (numBullets > 1)
            {
                GameObject bullet1 = Instantiate(bulletInitObj);
                bullet1.transform.position = bulletSpawn1.transform.position;
                bullet1.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                bullet1.SetActive(true);

                GameObject bullet2 = Instantiate(bulletInitObj);
                bullet2.transform.position = bulletSpawn2.transform.position;
                bullet2.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                bullet2.SetActive(true);
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
            timeScale += 0.025f;
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
            if (healthRemaining <= 0)
            {
                // we died
                statusText.text = "Aim better!";
                gameRunning = false;
                continueButton.SetActive(true);
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

            // did we win?
            if (healthRemaining >= 0 && NumAsteroidsDestroyed >= NUM_ASTEROIDS)
            {
                overworldManager.CompleteLevel(overworldManager.CurLevel.LevelID);
            }

            // set this back
            Time.timeScale = 1.0f;
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }
}