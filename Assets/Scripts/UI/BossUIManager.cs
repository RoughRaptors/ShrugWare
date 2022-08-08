using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class BossUIManager : MonoBehaviour
    {
        public static BossUIManager Instance = null;

        [SerializeField]
        InputField timeScaleInputField = null;

        [SerializeField]
        Text betweenMicrogameText = null;

        [SerializeField]
        Button continueGameButton = null;

        [SerializeField]
        Text gameInfoText = null;
        
        [SerializeField]
        GameObject mainUICanvas = null;
 
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
                timeScaleInputField = BossUIManager.Instance.timeScaleInputField;
                timeScaleInputField.text = "Time Scale: " + BossGameManager.Instance.GetCurTimeScale().ToString("F3");

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
            timeScaleInputField.text = "Time Scale: " + BossGameManager.Instance.GetCurTimeScale().ToString("F3");
            UpdateConsumableInfo();
        }

        public void UpdateBetweenMicrogameText()
        {
            betweenMicrogameText.enabled = true;
            betweenMicrogameText.text = "Next Level In: " + (DataManager.SECONDS_BETWEEN_MICROGAMES - BossGameManager.Instance.GetTimeInMainScene()).ToString("F2") + "s";
            if (BossGameManager.Instance.GetPreviouslyRanEffects().Count > 0)
            {
                betweenMicrogameText.text += "\n" + BossGameManager.Instance.GetPreviousEffectInfoString();
            }
        }

        public void ConfirmTimeScaleButtonClicked()
        {
            float newTimeScale = 1.0f;
            float.TryParse(timeScaleInputField.text, out newTimeScale);
            if (BossGameManager.Instance.GetCurTimeScale() != newTimeScale && newTimeScale != 0)
            {
                BossGameManager.Instance.SetCurTimeScale(newTimeScale);
            }

            timeScaleInputField.text = "Time Scale: " + BossGameManager.Instance.GetCurTimeScale().ToString("F3");
        }

        public void FillGameInfoText(Boss curBoss, BossGameManager.PlayerInfo playerInfo)
        {
            if (!(curBoss is null))
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
            continueGameButton.gameObject.SetActive(false);
            playerHealthBar.gameObject.SetActive(false);
            bossHealthBar.gameObject.SetActive(false);
            gameInfoText.enabled = true;

            AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.ButtonClick);
            BossGameManager.Instance.ContinueGame();
        }

        public void HandleWinGame()
        {
            betweenMicrogameText.enabled = false;
            gameInfoText.text += "\n CONGLADURATION. YOU ARE WIN";
        }

        public void HandlePauseGame()
        {
            continueGameButton.GetComponentInChildren<Text>().text = "Continue to " + BossGameManager.Instance.CurBoss.bossName;
            continueGameButton.gameObject.SetActive(true);
        }

        public void HandleGameOver()
        {
            betweenMicrogameText.enabled = false;
            gameInfoText.text += "\n \n 50 DKP MINUS!";
        }

        public void UpdateConsumableInfo()
        {
            healthPotionItem.GetComponentInChildren<Text>().text = "";
            maxHealthPotionItem.GetComponentInChildren<Text>().text = "";

            if(!OverworldManager.Instance) return;

            Item healthPotion = OverworldManager.Instance.GetPlayerInventory().GetInventoryItem(0);
            Item maxHealthPotion = OverworldManager.Instance.GetPlayerInventory().GetInventoryItem(1);
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
            BossGameManager.Instance.UseConsumableItem(templateId);
        }
    }
}