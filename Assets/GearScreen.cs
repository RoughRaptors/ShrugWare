using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;

namespace ShrugWare
{
    public class GearScreen : MonoBehaviour
    {
        public static GearScreen Instance;

        [SerializeField]
        private GameObject helmObj;

        [SerializeField]
        private GameObject chestObj;

        [SerializeField]
        private GameObject glovesObj;

        [SerializeField]
        private GameObject legsObj;

        [SerializeField]
        private GameObject bootsObj;

        [SerializeField]
        private GameObject healthPotionObj;

        [SerializeField]
        private GameObject maxHealthPotionObj;

        [SerializeField]
        private GameObject runSpeedPotionObj;

        private float startTime;

        private PlayerInventory inventory;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            startTime = Time.time;
            SetupScreen();
        }

        private void Update()
        {
            // it keeps 'I' pressed through the scene transition so we need to tell it to not count if it hasn't been long enough
            if (Input.GetKeyDown(KeyCode.Escape) || (Time.time - startTime > 1.0f) && (Input.GetKeyDown(KeyCode.I)))
            {
                ExitScene();
            }
        }

        private void SetupScreen()
        {
            inventory = OverworldManager.Instance.PlayerInventory;
            if (inventory != null)
            {
                HandleHelmetSlot();
                HandleChestSlot();
                HandleGlovesSlot();
                HandleLegsSlot();
                HandleBootsSlot();

                HandleHealthPotionSlot();
                HandleMaxHealthPotionSlot();
                HandleRunSpeedPotionSlot();
            }
        }

        void HandleHelmetSlot()
        {
            Color newHelmColor;
            Color helmColor = helmObj.GetComponentInChildren<RawImage>().color;
            newHelmColor = new Color(helmColor.r, helmColor.g, helmColor.b, 0.7f);

            Item helmItem = inventory.GetEquippedItem(DataManager.ArmorSlot.Head);
            if (helmItem != null)
            {
                helmObj.GetComponentInChildren<RawImage>().texture = helmItem.itemImage.texture;
                newHelmColor = new Color(helmColor.r, helmColor.g, helmColor.b, 1.0f);
            }

            helmObj.GetComponentInChildren<RawImage>().color = newHelmColor;
        }

        void HandleChestSlot()
        {
            Color newChestColor;
            Color chestColor = chestObj.GetComponentInChildren<RawImage>().color;
            newChestColor = new Color(chestColor.r, chestColor.g, chestColor.b, 0.7f);

            Item chestItem = inventory.GetEquippedItem(DataManager.ArmorSlot.Chest);
            if (chestItem != null)
            {
                chestObj.GetComponentInChildren<RawImage>().texture = chestItem.itemImage.texture;
                newChestColor = new Color(chestColor.r, chestColor.g, chestColor.b, 1.0f);
            }

            chestObj.GetComponentInChildren<RawImage>().color = newChestColor;
        }

        void HandleGlovesSlot()
        {
            Color newGlovesColor;
            Color glovesColor = glovesObj.GetComponentInChildren<RawImage>().color;
            newGlovesColor = new Color(glovesColor.r, glovesColor.g, glovesColor.b, 0.7f);

            Item glovesItem = inventory.GetEquippedItem(DataManager.ArmorSlot.Gloves);
            if (glovesItem != null)
            {
                glovesObj.GetComponentInChildren<RawImage>().texture = glovesItem.itemImage.texture;
                newGlovesColor = new Color(glovesColor.r, glovesColor.g, glovesColor.b, 1.0f);
            }

            glovesObj.GetComponentInChildren<RawImage>().color = newGlovesColor;
        }

        void HandleLegsSlot()
        {
            Color newLegsColor;
            Color legsColor = legsObj.GetComponentInChildren<RawImage>().color;
            newLegsColor = new Color(legsColor.r, legsColor.g, legsColor.b, 0.7f);

            Item legsItem = inventory.GetEquippedItem(DataManager.ArmorSlot.Legs);
            if (legsItem != null)
            {
                legsObj.GetComponentInChildren<RawImage>().texture = legsItem.itemImage.texture;
                newLegsColor = new Color(legsColor.r, legsColor.g, legsColor.b, 1.0f);
            }

            legsObj.GetComponentInChildren<RawImage>().color = newLegsColor;
        }

        void HandleBootsSlot()
        {
            Color newBootsColor;
            Color bootsColor = bootsObj.GetComponentInChildren<RawImage>().color;
            newBootsColor = new Color(bootsColor.r, bootsColor.g, bootsColor.b, 0.7f);

            Item bootsItem = inventory.GetEquippedItem(DataManager.ArmorSlot.Boots);
            if (bootsItem != null)
            {
                bootsObj.GetComponentInChildren<RawImage>().texture = bootsItem.itemImage.texture;
                newBootsColor = new Color(bootsColor.r, bootsColor.g, bootsColor.b, 1.0f);
            }

            bootsObj.GetComponentInChildren<RawImage>().color = newBootsColor;
        }

        // equipment is generated in the merchant and passed to the inventory. this causes a problem when the merchatn doesn't sell potions
        // since it's not created on entering the merchant, we need to somehow get its item image, so i made a helper class for that
        void HandleHealthPotionSlot()
        {
            Color newHealthPotionColor;
            Color healthPotionColor = healthPotionObj.GetComponentInChildren<RawImage>().color;
            newHealthPotionColor = new Color(healthPotionColor.r, healthPotionColor.g, healthPotionColor.b, 0.7f);

            Item healthPotionItem = inventory.GetInventoryItem((int)DataManager.ItemTemplateIds.HealthPotion);
            if (healthPotionItem != null)
            {
                healthPotionObj.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                if (healthPotionItem.itemQuantity > 0)
                {
                    healthPotionObj.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    healthPotionObj.GetComponentInChildren<TextMeshProUGUI>().text = healthPotionItem.itemQuantity.ToString();
                    healthPotionObj.GetComponentInChildren<RawImage>().texture = ImageHelper.Instance.healthPotionImage.texture;
                    newHealthPotionColor = new Color(healthPotionColor.r, healthPotionColor.g, healthPotionColor.b, 1.0f);
                }
            }

            healthPotionObj.GetComponentInChildren<RawImage>().color = newHealthPotionColor;
        }

        void HandleMaxHealthPotionSlot()
        {
            Color newMaxHealthPotionColor;
            Color maxHealthPotionColor = maxHealthPotionObj.GetComponentInChildren<RawImage>().color;
            newMaxHealthPotionColor = new Color(maxHealthPotionColor.r, maxHealthPotionColor.g, maxHealthPotionColor.b, 0.7f);

            Item maxHealthPotionItem = inventory.GetInventoryItem((int)DataManager.ItemTemplateIds.MAX);
            if (maxHealthPotionItem != null)
            {
                maxHealthPotionObj.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                if (maxHealthPotionItem.itemQuantity > 0)
                {
                    maxHealthPotionObj.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    maxHealthPotionObj.GetComponentInChildren<TextMeshProUGUI>().text = maxHealthPotionItem.itemQuantity.ToString();
                    maxHealthPotionObj.GetComponentInChildren<RawImage>().texture = ImageHelper.Instance.maxHealthPotionImage.texture;
                    newMaxHealthPotionColor = new Color(maxHealthPotionColor.r, maxHealthPotionColor.g, maxHealthPotionColor.b, 1.0f);
                }

                maxHealthPotionObj.GetComponentInChildren<RawImage>().color = newMaxHealthPotionColor;
            }
        }

        void HandleRunSpeedPotionSlot()
        {
            Color newRunSpeedPotionColor;
            Color runSpeedPotionColor = runSpeedPotionObj.GetComponentInChildren<RawImage>().color;
            newRunSpeedPotionColor = new Color(runSpeedPotionColor.r, runSpeedPotionColor.g, runSpeedPotionColor.b, 0.7f);

            Item runSpeedPotionItem = inventory.GetInventoryItem((int)DataManager.ItemTemplateIds.RunSpeedPotion);
            if (runSpeedPotionItem != null)
            {
                runSpeedPotionObj.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                if (runSpeedPotionItem.itemQuantity > 0)
                {
                    runSpeedPotionObj.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    runSpeedPotionObj.GetComponentInChildren<TextMeshProUGUI>().text = runSpeedPotionItem.itemQuantity.ToString();
                    runSpeedPotionObj.GetComponentInChildren<RawImage>().texture = ImageHelper.Instance.runSpeedPotionImage.texture;
                    newRunSpeedPotionColor = new Color(runSpeedPotionColor.r, runSpeedPotionColor.g, runSpeedPotionColor.b, 1.0f);
                }
            }

            runSpeedPotionObj.GetComponentInChildren<RawImage>().color = runSpeedPotionColor;
        }

        private void ExitScene()
        {
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
            Destroy(this);
        }

        public void OnBackButtonPressed()
        {
            ExitScene();
        }
    }
}