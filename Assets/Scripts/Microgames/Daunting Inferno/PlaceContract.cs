using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

namespace ShrugWare
{
    public class PlaceContract : Minigame
    {        
        [SerializeField]
        List<GameObject> hitVFXList;

        [SerializeField]
        GameObject contractObj;

        [SerializeField]
        List<GameObject> contractSpawnPositions = new List<GameObject>();

        [SerializeField]
        GameObject popupObj;

        [SerializeField]
        GameObject bossObj;

        [SerializeField]
        GameObject tankObj;

        [SerializeField]
        GameObject playerDisplayObj;

        [SerializeField]
        GameObject timerBarObj;

        [SerializeField]
        GameObject continueButton;

        [SerializeField]
        GameObject instructionContractBefore;

        [SerializeField]
        GameObject instructionContractAfter;

        [SerializeField]
        GameObject arrowObj;

        List<GameObject> placedContracts = new List<GameObject>();

        private int contractSpawnIndex;

        private bool failedPlacement = false;

        private const float PLAYER_SPEED = 50.0f;
        private const float SPAWN_INTERVAL = 2.75f; // how often a contract spawns
        private const float DROP_TIMER = 1.75f; // how long we have before the contract drops
        private float lastPlacedTime = 0; //1.25f; // start with this already set to give the player a bit of time to orient themselves

        [Serializable]
        public class ContractObject
        {
            public GameObject contractObj;
            public GameObject rays;
            public GameObject runes;
            public GameObject runesSmall;
        }

        private ContractObject curContract; // the contract on the player

        protected override void Start()
        {
            base.Start();

            minigameDuration = 25.0f;
        }

        protected override void Update()
        {
            base.Update();

            if (!gameOver && gameStarted)
            {
                if (timeElapsed > lastPlacedTime + SPAWN_INTERVAL + DROP_TIMER)
                {
                    lastPlacedTime = timeElapsed;
                    SpawnContract();
                }

                HandlePlayerMovement();

                float timeLeft = minigameDuration - timeElapsed;
                timerBarObj.GetComponent<Slider>().value = timeLeft / minigameDuration;
            }
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

            if(curContract != null)
            {
                curContract.contractObj.transform.position = new Vector2(newPos.x, newPos.y + 3.5f);
            }

            this.GetComponent<Rigidbody2D>().MovePosition(newPos);
        }

        public void OnStartPressed()
        {
            if (gameOver)
            {
                SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
            }
            else
            {
                gameStarted = true;
                bossObj.SetActive(true);
                tankObj.SetActive(true);
                playerDisplayObj.SetActive(true);
                popupObj.SetActive(false);
                timerBarObj.SetActive(true);

                contractSpawnIndex = UnityEngine.Random.Range(0, contractSpawnPositions.Count);
                contractSpawnPositions[contractSpawnIndex].SetActive(true);
            }
        }

        private void SpawnContract()
        {
            Vector2 spawnPos = this.transform.position;
            spawnPos = new Vector2(spawnPos.x, spawnPos.y);
            Quaternion rotPos = contractObj.transform.rotation;

            GameObject newContract = Instantiate(contractObj, spawnPos, rotPos);
            newContract.transform.parent = this.transform;
            newContract.SetActive(true);
            placedContracts.Add(newContract);

            ContractObject contract = new ContractObject();
            contract.contractObj = newContract;
            contract.rays = newContract.transform.Find("Rays").gameObject;
            contract.runes = newContract.transform.Find("Runes").gameObject;
            contract.runesSmall = newContract.transform.Find("Runes small").gameObject;
            curContract = contract;

            Invoke("DropContract", DROP_TIMER);
        }

        private void DropContract()
        {
            Color newColor = new Color(193, 168, 94, 90);
            ParticleSystem.MainModule psMainRays = curContract.rays.GetComponent<ParticleSystem>().main;
            psMainRays.startColor = newColor;

            ParticleSystem.MainModule psMainRunes = curContract.runes.GetComponent<ParticleSystem>().main;
            psMainRunes.startColor = newColor;

            ParticleSystem.MainModule psMainRunesSmall = curContract.runesSmall.GetComponent<ParticleSystem>().main;
            psMainRunesSmall.startColor = newColor;

            curContract = null;
        }

        protected override void EndGame(bool success)
        {
            base.EndGame(success);

            contractSpawnPositions[contractSpawnIndex].SetActive(false);
            foreach(GameObject placedContract in placedContracts)
            {
                placedContract.SetActive(false);
            }

            playerDisplayObj.SetActive(false);
            bossObj.SetActive(false);
            tankObj.SetActive(false);
            instructionContractBefore.SetActive(false);
            instructionContractAfter.SetActive(false);
            arrowObj.SetActive(false);
            timerBarObj.SetActive(false);
            popupObj.SetActive(true);

            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Back To Overworld";

            if (success)
            {
                popupObj.GetComponentInChildren<TextMeshProUGUI>().text = "Success!\nAwarded 500 DKP and 250 Gold!";

                if(OverworldManager.Instance != null)
                {
                    OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.DKP, 500);
                    OverworldManager.Instance.PlayerInventory.AddCurrency(DataManager.Currencies.Gold, 250);
                }
            }
            else
            {
                popupObj.GetComponentInChildren<TextMeshProUGUI>().text = "Placement Failure";
            }
        }
    }
}