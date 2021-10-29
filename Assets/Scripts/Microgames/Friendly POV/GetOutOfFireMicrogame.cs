using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class GetOutOfFireMicrogame : Microgame
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

            microgameCategory = MicrogameCategory.Friendly;
            StartCoroutine("DisableInstructionsText");
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;

            // don't "start" the microgame until we can orient the player to the microgame
            if (timeElapsed >= DataManager.SECONDS_TO_START_MICROGAME)
            {
                microgameDurationRemaining -= Time.deltaTime;
                timerText.text = microgameDurationRemaining.ToString("F2") + "s";

                HandleInput();

                if (microgameDurationRemaining <= 0.0f)
                {
                    // out of time
                    HandleMicrogameEnd(!inFire);
                }
            }
        }

        // easier to make this a coroutine since Update() will keep trying to disable it (for now at least)
        IEnumerator DisableInstructionsText()
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
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