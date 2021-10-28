using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class GetOutOfFireMinigame : Minigame
    {
        [SerializeField]
        Text instructionsText;
        
        [SerializeField]
        Text timerText;

        [SerializeField]
        GameObject playerObject;

        private const float PLAYER_MOVE_SPEED = 2.5f;

        private bool inFire = true;

        new private void Start()
        {
            base.Start();

            minigameCategory = MinigameCategory.Friendly;
            StartCoroutine("DisableInstructionsText");
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;

            // don't "start" the minigame until we can orient the player to the minigame
            if (timeElapsed >= DataManager.SECONDS_TO_START_MINIGAME)
            {
                minigameDurationRemaining -= Time.deltaTime;
                timerText.text = minigameDurationRemaining.ToString("F2") + "s";

                HandleInput();

                if (minigameDurationRemaining <= 0.0f)
                {
                    // out of time
                    HandleMinigameEnd(!inFire);
                }
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MINIGAME);
            instructionsText.gameObject.SetActive(false);
        }

        private void HandleInput()
        {
            Vector3 newPos = playerObject.transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                newPos.y += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                newPos.y -= PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                newPos.x -= PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                newPos.x += PLAYER_MOVE_SPEED * Time.deltaTime;
            }

            playerObject.transform.position = newPos;
        }

        // once they're out, we don't care if they go back in
        private void OnTriggerExit(Collider other)
        {
            inFire = false;

            instructionsText.gameObject.SetActive(true);
            instructionsText.text = "No noms for dargon";
        }
    }
}