using TMPro;
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
        TextMeshProUGUI infoText = null;

        [SerializeField]
        Button continueGameButton = null;
        
        [SerializeField]
        GameObject mainUICanvas = null;
 
        [SerializeField]
        GameObject healthPotionItem = null;

        [SerializeField]
        GameObject maxHealthPotionItem = null;

        [SerializeField]
        GameObject moveSpeedPotionItem = null;

        [SerializeField]
        HealthBar playerHealthBar = null;

        [SerializeField]
        HealthBar bossHealthBar = null;

        bool countdownTimerRunning = false;
        bool gameOver = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                // set all of our shit back - figure out a better solution later if there is one - TODO MAKE THIS BETTER
                timeScaleInputField = BossUIManager.Instance.timeScaleInputField;
                timeScaleInputField.text = "Time Scale: " + BossGameManager.Instance.GetCurTimeScale().ToString("F3");
                mainUICanvas = BossUIManager.Instance.mainUICanvas;
                mainUICanvas.SetActive(true);

#if !UNITY_EDITOR
                // don't show this outside of the editor, it's debug
                timeScaleInputField.gameObject.SetActive(false);
#endif
            }

            continueGameButton.GetComponentInChildren<Text>().text = "Continue";
        }

        // Start is called before the first frame update
        void Start()
        {
            gameOver = false;
            infoText.enabled = false;
            timeScaleInputField.text = "Time Scale: " + BossGameManager.Instance.GetCurTimeScale().ToString("F3");
            UpdateConsumableInfo();
        }

        public void UpdateBetweenMicrogameText()
        {
            if(BossGameManager.Instance.CurBoss.isDead)
            {
                return;
            }

            float timeLeft = DataManager.SECONDS_BETWEEN_MICROGAMES - BossGameManager.Instance.GetTimeInBossScene();

            if (!gameOver)
            {
                infoText.enabled = true;
                infoText.text = "Next Level In: " + timeLeft.ToString("F2") + "s\n";
            }
            else
            {
                infoText.text = "";
            }

            if (BossGameManager.Instance.GetPreviouslyRanEffects().Count > 0)
            {
                infoText.text += BossGameManager.Instance.GetPreviousEffectInfoString();
            }

            if (countdownTimerRunning)
            {
                // play a sound effect on every even number except for 0. just do a countdown then a ding at 0
                if (timeLeft > 0 && (timeLeft % 1 < 0.05f ))
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.MicrogameTimerTick);
                    }
                    countdownTimerRunning = true;
                }
                else if (timeLeft <= 0.0f)
                {
                    countdownTimerRunning = false;

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.MicrogameTimerDing);
                    }
                }
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

        public void UpdateHealthBars()
        {
            playerHealthBar.gameObject.SetActive(true);
            bossHealthBar.gameObject.SetActive(true);

            bossHealthBar.UpdateHealthBar();
            playerHealthBar.UpdateHealthBar();
        }

        public void SetBossUICanvasEnabled(bool enabled)
        {
            mainUICanvas.SetActive(enabled);
            countdownTimerRunning = false;
        }

        public void SetTimescaleInputFieldText(string newText)
        {
            timeScaleInputField.text = newText;
        }

        public void OnContinueGameButtonClicked()
        {
            infoText.enabled = true;
            continueGameButton.gameObject.SetActive(false);
            playerHealthBar.gameObject.SetActive(false);
            bossHealthBar.gameObject.SetActive(false);
            //betweenMicrogameText.gameObject.SetActive(false);
            countdownTimerRunning = true;

            // we died or killed the boss, go back instead
            if (BossGameManager.Instance.GetPlayerInfo().livesLeft < 0 || BossGameManager.Instance.CurBoss.isDead)
            {
                mainUICanvas.SetActive(false);
                OverworldManager.Instance.ReadyScene(true);
                BossGameManager.Instance.LoadScene((int)DataManager.Scenes.OverworldScene);

                // do we want to un-singleton BossGameManager? destroy this for now
                Destroy(BossGameManager.Instance.gameObject);

                return;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.ButtonClick);
            }

            BossGameManager.Instance.ContinueGame();
        }

        public void HandleBeatBoss(DataManager.Currencies lootCurrency, int lootAmount)
        {
            OverworldManager overworldManager = OverworldManager.Instance;
            if (!overworldManager)
            {
                return;
            }

            overworldManager.CompleteLevel(overworldManager.CurLevel.LevelID);

            gameOver = true;
            healthPotionItem.gameObject.SetActive(false);
            maxHealthPotionItem.gameObject.SetActive(false);
            moveSpeedPotionItem.gameObject.SetActive(false);
            infoText.gameObject.SetActive(true);
            infoText.text = "You beat the boss!\nReceived  " + lootAmount + " DKP";
            infoText.enabled = true;
            continueGameButton.GetComponentInChildren<Text>().text = "Back to Overworld";
            continueGameButton.gameObject.SetActive(true);
        }

        public void HandleGameOver()
        {
            gameOver = true;
            healthPotionItem.gameObject.SetActive(false);
            maxHealthPotionItem.gameObject.SetActive(false);
            moveSpeedPotionItem.gameObject.SetActive(false);
            infoText.gameObject.SetActive(true);
            infoText.text = "Game Over! \n 50 DKP MINUS!";
            continueGameButton.GetComponentInChildren<Text>().text = "Back To Overworld";
            infoText.enabled = true;
            continueGameButton.gameObject.SetActive(true);
        }

        public void UpdateConsumableInfo()
        {
            if (!OverworldManager.Instance)
            { 
                return; 
            }

            healthPotionItem.GetComponentInChildren<TextMeshProUGUI>().text = "";
            maxHealthPotionItem.GetComponentInChildren<TextMeshProUGUI>().text = "";
            moveSpeedPotionItem.GetComponentInChildren<TextMeshProUGUI>().text = "";

            Item healthPotion = OverworldManager.Instance.PlayerInventory.GetInventoryItem((int)DataManager.ItemTemplateIds.HealthPotion);
            Item maxHealthPotion = OverworldManager.Instance.PlayerInventory.GetInventoryItem((int)DataManager.ItemTemplateIds.MaxHealthPotion);
            Item moveSpeedPotion = OverworldManager.Instance.PlayerInventory.GetInventoryItem((int)DataManager.ItemTemplateIds.RunSpeedPotion);
            if (healthPotion != null)
            {
                healthPotionItem.GetComponentInChildren<TextMeshProUGUI>().text += "Health Potion\n+25% Heal\nQuantity: " + healthPotion.itemQuantity;
            }

            if(maxHealthPotion != null)
            {
                maxHealthPotionItem.GetComponentInChildren<TextMeshProUGUI>().text += "Max Health Potion\n+10% Max HP\nQuantity: " + maxHealthPotion.itemQuantity;
            }

            if (moveSpeedPotionItem != null)
            {
                moveSpeedPotionItem.GetComponentInChildren<TextMeshProUGUI>().text += "Move Speed Potion\n+10% Max HP\nQuantity: " + moveSpeedPotion.itemQuantity;
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