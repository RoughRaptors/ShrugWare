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
        Button continueGameButton = null;

        [SerializeField]
        Text gameInfoText = null;

        [SerializeField]
        Button merchantButton = null;
        
        [SerializeField]
        GameObject mainUICanvas = null;

        [SerializeField]
        GameObject merchantUICanvas = null;

        [SerializeField]
        GameObject instructionsUICanvas = null;

        [SerializeField]
        GameObject instructionsUIButton = null;
 
        [SerializeField]
        GameObject healthPotionItem = null;

        [SerializeField]
        GameObject maxHealthPotionItem = null;

        [SerializeField]
        HealthBar playerHealthBar = null;

        [SerializeField]
        HealthBar bossHealthBar = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);

                // set all of our shit back - figure out a better solution later if there is one - TODO MAKE THIS BETTER
                timeScaleInputField = UIManager.Instance.timeScaleInputField;
                timeScaleInputField.text = "Time Scale: " + GameManager.Instance.GetCurTimeScale().ToString("F3");

#if !UNITY_EDITOR
                // don't show this outside of the editor, it's debug
                timeScaleInputField.gameObject.SetActive(false);
#endif
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            betweenMicrogameText.enabled = false;
            timeScaleInputField.text = "Time Scale: " + GameManager.Instance.GetCurTimeScale().ToString("F3");
            UpdateConsumableInfo();
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

        public void FillGameInfoText(Raid curRaid, GameManager.PlayerInfo playerInfo)
        {
            if (!(curRaid is null) && !(curRaid.curBoss is null))
            {
                gameInfoText.text = "Rezzes Left: " + playerInfo.livesLeft.ToString();
            }
            else
            {
                Debug.Log("Raid or Boss null in FillGameInfoText");
            }
        }

        public void UpdateHealthBars()
        {
            playerHealthBar.gameObject.SetActive(true);
            bossHealthBar.gameObject.SetActive(true);

            bossHealthBar.UpdateHealthBar();
            playerHealthBar.UpdateHealthBar();
        }

        public void SetMainCanvasEnabled(bool enabled)
        {
            mainUICanvas.SetActive(enabled);
        }

        public void SetTimescaleInputFieldText(string newText)
        {
            timeScaleInputField.text = newText;
        }

        public void OnContinueGameButtonClicked()
        {
            betweenMicrogameText.enabled = true;
            merchantButton.gameObject.SetActive(false);
            continueGameButton.gameObject.SetActive(false);
            playerHealthBar.gameObject.SetActive(false);
            bossHealthBar.gameObject.SetActive(false);
            gameInfoText.enabled = true;
            instructionsUIButton.SetActive(false);

            AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.ButtonClick);
            GameManager.Instance.ContinueGame();
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
                mainUICanvas.SetActive(false);
                merchantUICanvas.SetActive(true);

                GameManager.Instance.EnterMerchant();
                MerchantManager.Instance.UpdateCurrencies();
            }
        }

        public void OnExitMerchantClicked()
        {
            GameManager.Instance.SetGameState(GameManager.GameState.Paused);
            mainUICanvas.SetActive(true);
            merchantUICanvas.SetActive(false);

            UpdateConsumableInfo();
            AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.ButtonClick);
            GameManager.Instance.UpdateGameUI();
            MerchantManager.Instance.ExitMerchant();
        }

        public void SetMerchantButtonActive(bool enabled)
        {
            merchantButton.gameObject.SetActive(enabled);
        }

        public void OnItemForSaleSelected(int itemTemplateId)
        {
            MerchantManager.Instance.OnItemSelected(itemTemplateId);
        }

        public void OnMerchantBuyButtonClicked()
        {
            MerchantManager.Instance.OnBuyButtonClicked();
        }

        public void UpdateConsumableInfo()
        {
            healthPotionItem.GetComponentInChildren<Text>().text = "";
            maxHealthPotionItem.GetComponentInChildren<Text>().text = "";

            Item healthPotion = GameManager.Instance.GetPlayerInventory().GetInventoryItem(0);
            Item maxHealthPotion = GameManager.Instance.GetPlayerInventory().GetInventoryItem(1);
            if(healthPotion != null)
            {
                healthPotionItem.GetComponentInChildren<Text>().text += "Health Potion\n+25% Heal\nQuantity: " + healthPotion.itemQuantity;
            }

            if(maxHealthPotion != null)
            {
                maxHealthPotionItem.GetComponentInChildren<Text>().text += "\nMax Health Potion\n+10% Max HP\nQuantity: " + maxHealthPotion.itemQuantity;
            }
        }

        public void ToggleConsumableVisibility(bool enabled)
        {
            healthPotionItem.SetActive(enabled);
            maxHealthPotionItem.SetActive(enabled);
        }

        public void OnUseConsumableItemClicked(int templateId)
        {
            GameManager.Instance.UseConsumableItem(templateId);
        }

        public void OnInstructionsButtonClicked()
        {
            AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.ButtonClick);
            mainUICanvas.SetActive(false);
            instructionsUICanvas.SetActive(true);
        }

        public void OnInstructionsBackButtonClicked()
        {
            AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.ButtonClick);
            mainUICanvas.SetActive(true);
            instructionsUICanvas.SetActive(false);
        }
    }
}