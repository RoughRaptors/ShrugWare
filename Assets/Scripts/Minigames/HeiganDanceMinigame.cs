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

        private float timeInGame = 0.0f;

        const float PLAYER_X_MIN = -13;
        const float PLAYER_X_MAX = 115;
        const float PLAYER_Y_MIN = -30;
        const float PLAYER_Y_MAX = 35;
        private const float PLAYER_SPEED = 50.0f;

        // float so we can take partial damage via damage mitigation. it's not clean but blegh
        private const float START_HEALTH = 5;
        private float healthRemaining = 5;
        bool gameRunning = false;

        private void Awake()
        {
            continueButton.SetActive(false);
        }

        private new void Start()
        {
            base.Start();
            healthRemaining = START_HEALTH + healthToAdd;

            gameRunning = true;
            statusText.text = "HP: " + healthRemaining.ToString("F2");
        }

        private void FixedUpdate()
        {
            if(gameRunning)
            {
                HandlePlayerMovement();
                timeInGame += Time.deltaTime;
            }

            timeRemainingText.text = "Time Remaining: " + (DataManager.MINIGAME_DURATION_SECONDS - timeInGame).ToString("F2");
            if(gameRunning && timeInGame >= DataManager.MINIGAME_DURATION_SECONDS)
            {
                // out of time, we won
                gameRunning = false;

                int lootAmount = 500;
                endGameText.text = "Boss time!\nReceived " + lootAmount + " gold";
                OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Generic, lootAmount);

                continueButton.SetActive(true);
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