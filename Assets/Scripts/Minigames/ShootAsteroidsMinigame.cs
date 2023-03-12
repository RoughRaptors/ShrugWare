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
        private const float PLAYER_SPEED = 20.0f;

        // float so we can take partial damage via damage mitigation. it's not clean but blegh
        private const float START_HEALTH = 5;
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

        private void Update()
        {
            if (gameRunning)
            {
                HandlePlayerMovement();
                timeInGame += Time.deltaTime;
            }

            timeRemainingText.text = "Time Remaining: " + (DataManager.MINIGAME_DURATION_SECONDS - timeInGame).ToString("F2");
            if (timeInGame >= DataManager.MINIGAME_DURATION_SECONDS)
            {
                // out of time ,we lost
                gameRunning = false;
                statusText.text = "NO TIME LEFT!";
                continueButton.SetActive(true);
            }

            // destroyed all
            if(NumAsteroidsDestroyed == NUM_ASTEROIDS)
            {
                gameRunning = false;
                statusText.text = "Boss time!";
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
            // give thruster in facing direction
            if (Input.GetKey(KeyCode.W))
            {
                this.GetComponent<Rigidbody>().AddRelativeForce(2.5f, 0.0f, 0);
            }

            // rotate left/right
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, 0, .33f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, 0, -.33f);
            }

            // shoot
            if (Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Mouse0)
                || Input.GetKeyDown(KeyCode.Mouse1))
            {
                // fire and forget, we'll stop after 45 seconds anyways
                GameObject newBullet = Instantiate(bulletInitObj);
                newBullet.transform.position = this.transform.position;
                newBullet.transform.forward = this.transform.forward;
                newBullet.GetComponent<Rigidbody>().velocity = this.transform.right * 15.0f;
                newBullet.SetActive(true);

                // speed up the game per bullet shot
                timeScale += 0.01f;
                Time.timeScale = timeScale;
            }

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

            /*
            // https://stackoverflow.com/questions/49102831/teleporting-character-from-edge-to-edge-of-the-screen
            //you get a world space coord and transform it to viewport space.
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

            //everything from here on is in viewport space where 0,0 is the bottom 
            //left of your screen and 1,1 the top right.
            if (pos.x < 0.0f)
            {
                pos = new Vector3(1.0f, pos.y, pos.z);
            }
            else if (pos.x >= 1.0f)
            {
                pos = new Vector3(0.0f, pos.y, pos.z);
            }
            if (pos.y < 0.0f)
            {
                pos = new Vector3(pos.x, 1.0f, pos.z);
            }
            else if (pos.y >= 1.0f)
            {
                pos = new Vector3(pos.x, 0.0f, pos.z);
            }

            //and here it gets transformed back to world space.
            transform.position = Camera.main.ViewportToWorldPoint(pos);
            */
        }

        // we physically collided with something
        private void OnTriggerEnter(Collider other)
        {
            // don't hit our own bullets
            if(other.tag == "Bullet")
            {
                return;
            }

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
                OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Generic, 500);
                overworldManager.CompleteLevel(overworldManager.CurLevel.LevelID);
            }

            // set this back
            Time.timeScale = 1.0f;
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }
}