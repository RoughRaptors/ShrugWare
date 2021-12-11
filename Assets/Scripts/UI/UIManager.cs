using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance = null;

        [SerializeField]
        InputField timeScaleInputField = null;

        [SerializeField]
        Text betweenMicrogameText = null;

        [SerializeField]
        Canvas mainCanvas = null;

        [SerializeField]
        Button continueGameButton = null;

        [SerializeField]
        Text gameInfoText = null;

        [SerializeField]
        Button merchantButton = null;

        [SerializeField]
        GameObject merchantUI = null;

        private void Awake()
        {
            if (Instance == null)
            {
                // DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);

                // set all of our shit back - figure out a better solution later if there is one - TODO MAKE THIS BETTER
                timeScaleInputField = UIManager.Instance.timeScaleInputField;
                timeScaleInputField.text = "Time Scale: " + GameManager.Instance.GetCurTimeScale().ToString("F3");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            gameInfoText.enabled = false;
            betweenMicrogameText.enabled = false;
            timeScaleInputField.text = "Time Scale: " + GameManager.Instance.GetCurTimeScale().ToString("F3");
        }

        public void UpdateBetweenMicrogameText()
        {
            betweenMicrogameText.enabled = true;
            betweenMicrogameText.text = "Next Level In: " + (DataManager.SECONDS_BETWEEN_MICROGAMES - GameManager.Instance.GetTimeInMainScene()).ToString("F2") + "s";
            if (GameManager.Instance.GetPreviouslyRanEffects().Count > 0)
            {
                betweenMicrogameText.text += "\n" + GameManager.Instance.GetPreviousEffectInfoString();
            }
        }

        public void ConfirmTimeScaleButtonClicked()
        {
            float newTimeScale = 1.0f;
            float.TryParse(timeScaleInputField.text, out newTimeScale);
            if (GameManager.Instance.GetCurTimeScale() != newTimeScale && newTimeScale != 0)
            {
                GameManager.Instance.SetCurTimeScale(newTimeScale);
            }

            timeScaleInputField.text = "Time Scale: " + GameManager.Instance.GetCurTimeScale().ToString("F3");
        }

        public void FillBossInfoText(Raid curRaid, GameManager.PlayerInfo playerInfo)
        {
            if (!(curRaid is null) && !(curRaid.curBoss is null))
            {
                gameInfoText.text = curRaid.raidName + "\n" + curRaid.curBoss.bossName + "\n"
                    + "Health: " + curRaid.curBoss.curHealth.ToString() + " / " + curRaid.curBoss.maxHealth + "\n \n"
                    + "Raid Health: " + playerInfo.curRaidHealth.ToString() + " / " + playerInfo.maxRaidHealth.ToString() + "\n"
                    + "Rezzes Left: " + playerInfo.livesLeft.ToString();
            }
            else
            {
                Debug.Log("Raid or Boss null in FillBossInfoText");
            }
        }

        public void SetCanvasEnabled(bool enabled)
        {
            mainCanvas.enabled = enabled;
        }

        public void SetTimescaleInputFieldText(string newText)
        {
            timeScaleInputField.text = newText;
        }

        public void OnContinueGameButtonClicked()
        {
            // if the merchant screen is open, close that
            if (GameManager.Instance.GetGameState() == GameManager.GameState.Merchant)
            {
                GameManager.Instance.SetGameState(GameManager.GameState.Paused);
                merchantButton.gameObject.SetActive(true);
                timeScaleInputField.gameObject.SetActive(true);
                continueGameButton.GetComponentInChildren<Text>().text = "Continue";
                gameInfoText.enabled = true;

                merchantUI.SetActive(true);
            }
            else if (GameManager.Instance.GetGameState() == GameManager.GameState.Paused)
            {
                // otherwise we are continuing the game
                betweenMicrogameText.enabled = true;
                merchantButton.gameObject.SetActive(false);
                continueGameButton.gameObject.SetActive(false);
                gameInfoText.enabled = true;

                GameManager.Instance.ContinueGame();
            }
        }

        public void HandleWinGame()
        {
            betweenMicrogameText.enabled = false;
            gameInfoText.text += "\n \n CONGLADURATION. YOU ARE WIN";
        }

        public void HandlePauseGame()
        {
            continueGameButton.GetComponentInChildren<Text>().text = "Continue to " + GameManager.Instance.GetCurRaid().curBoss.bossName;
            continueGameButton.gameObject.SetActive(true);
            merchantButton.gameObject.SetActive(false);
        }

        public void HandleGameOver()
        {
            betweenMicrogameText.enabled = false;
            gameInfoText.text += "\n \n 50 DKP MINUS!";
        }
        
        public void OnMerchantButtonClicked()
        {
            // don't open the merchant if we're not ready/able to - todo change this to a proper game state when we add it
            if (GameManager.Instance.GetGameState() == GameManager.GameState.Paused)
            {
                merchantButton.gameObject.SetActive(false);
                timeScaleInputField.gameObject.SetActive(false);
                betweenMicrogameText.enabled = false;
                continueGameButton.GetComponentInChildren<Text>().text = "Exit Merchant";
                gameInfoText.enabled = false;

                merchantUI.SetActive(true);
                GameManager.Instance.EnterMerchant();
            }
        }

        public void SetMerchantButtonActive(bool enabled)
        {
            merchantButton.gameObject.SetActive(enabled);
        }
    }
}