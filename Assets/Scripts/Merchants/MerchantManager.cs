using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    // todo - make it template id based like the inventory
    public class MerchantManager : MonoBehaviour
    {
        [SerializeField]
        GameObject helmObj = null;

        [SerializeField]
        GameObject chestObj = null;

        [SerializeField]
        GameObject glovesObj = null;

        [SerializeField]
        GameObject legsObj = null;

        [SerializeField]
        GameObject bootsObj = null;

        [SerializeField]
        GameObject healthPotionObj = null;

        [SerializeField]
        GameObject maxHalthPotionObj = null;

        [SerializeField]
        Text currenciesText = null;

        public static MerchantManager Instance = null;
        
        public struct ItemForSale
        {
            public Item item;
            public DataManager.Currencies currency;
            public int price;
        }

        private Dictionary<int, ItemForSale> itemsForSale = new Dictionary<int, ItemForSale>();
        public Dictionary<int, ItemForSale> GetItemsForSale() { return itemsForSale; }

        private ItemForSale selectedItem;
        private GameObject prevSelectedObj;

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
        }

        // for now just hard code the items
        void Start()
        {
            SetupInventory();
            UpdateCurrencies();
        }

        private void SetupInventory()
        {
            // 25% player heal
            DataManager.StatEffect healPlayerEffect;
            healPlayerEffect.amount = 25;
            healPlayerEffect.effectType = DataManager.StatModifierType.PlayerCurHealth;
            healPlayerEffect.asPercentage = true;

            // +10% max hp
            DataManager.StatEffect maxHPEffect;
            maxHPEffect.amount = 10;
            maxHPEffect.effectType = DataManager.StatModifierType.PlayerMaxHealth;
            maxHPEffect.asPercentage = true;

            // +10% damage reduction
            DataManager.StatEffect damageReductionEffect;
            damageReductionEffect.amount = 10;
            damageReductionEffect.effectType = DataManager.StatModifierType.IncomingDamage;
            damageReductionEffect.asPercentage = true;

            // hp potion
            ConsumableItem healthPotion = new ConsumableItem();
            healthPotion.itemName = "Health Potion";
            healthPotion.templateId = 0;
            healthPotion.AddEffect(healPlayerEffect);

            ItemForSale healthPotionItem;
            healthPotionItem.item = healthPotion;
            healthPotionItem.currency = DataManager.Currencies.Generic;
            healthPotionItem.price = 500;

            itemsForSale.Add(healthPotion.templateId, healthPotionItem);

            // max hp potion
            ConsumableItem maxHealthPotion = new ConsumableItem();
            maxHealthPotion.itemName = "Increase Max Health Potion";
            maxHealthPotion.templateId = 1;
            maxHealthPotion.AddEffect(maxHPEffect);

            ItemForSale maxHealthPotionItem;
            maxHealthPotionItem.item = maxHealthPotion;
            maxHealthPotionItem.currency = DataManager.Currencies.Generic;
            maxHealthPotionItem.price = 500;

            itemsForSale.Add(maxHealthPotion.templateId, maxHealthPotionItem);

            // helmet
            ArmorItem diHelm = new ArmorItem(DataManager.ArmorSlot.Head, DataManager.ArmorSet.DauntingInferno);
            diHelm.itemName = "Helm";
            diHelm.templateId = 2;
            diHelm.AddEffect(damageReductionEffect);

            ItemForSale diHelmItem;
            diHelmItem.item = diHelm;
            diHelmItem.currency = DataManager.Currencies.DauntingInferno;
            diHelmItem.price = 1000;

            itemsForSale.Add(diHelm.templateId, diHelmItem);

            // chest
            ArmorItem diChest = new ArmorItem(DataManager.ArmorSlot.Chest, DataManager.ArmorSet.DauntingInferno);
            diChest.itemName = "Chest";
            diChest.templateId = 3;
            diChest.AddEffect(damageReductionEffect);

            ItemForSale diChestItem;
            diChestItem.item = diChest;
            diChestItem.currency = DataManager.Currencies.DauntingInferno;
            diChestItem.price = 1000;

            itemsForSale.Add(diChest.templateId, diChestItem);

            // gloves
            ArmorItem diGloves = new ArmorItem(DataManager.ArmorSlot.Gloves, DataManager.ArmorSet.DauntingInferno);
            diGloves.itemName = "Gloves";
            diGloves.templateId = 4;
            diGloves.AddEffect(damageReductionEffect);

            ItemForSale diGlovesItem;
            diGlovesItem.item = diGloves;
            diGlovesItem.currency = DataManager.Currencies.DauntingInferno;
            diGlovesItem.price = 1000;

            itemsForSale.Add(diGloves.templateId, diGlovesItem);

            // legs
            ArmorItem diLegs = new ArmorItem(DataManager.ArmorSlot.Legs, DataManager.ArmorSet.DauntingInferno);
            diLegs.itemName = "Legs";
            diLegs.templateId = 5;
            diLegs.AddEffect(damageReductionEffect);

            ItemForSale diLegsItem;
            diLegsItem.item = diLegs;
            diLegsItem.currency = DataManager.Currencies.DauntingInferno;
            diLegsItem.price = 1000;

            itemsForSale.Add(diLegs.templateId, diLegsItem);

            // boots
            ArmorItem diBoots = new ArmorItem(DataManager.ArmorSlot.Boots, DataManager.ArmorSet.DauntingInferno);
            diBoots.itemName = "Boots";
            diBoots.templateId = 6;
            diBoots.AddEffect(damageReductionEffect);

            ItemForSale diBootsItem;
            diBootsItem.item = diBoots;
            diBootsItem.currency = DataManager.Currencies.DauntingInferno;
            diBootsItem.price = 1000;

            itemsForSale.Add(diBoots.templateId, diBootsItem);
        }

        public void OnBuyButtonClicked()
        {
            if (selectedItem.item != null)
            {
                PlayerInventory playerInventory = OverworldManager.Instance.PlayerInventory;
                if (playerInventory != null && playerInventory.GetCurrencyAmount(selectedItem.currency) >= selectedItem.price)
                {
                    // don't allow multiple purchases of the same armor piece
                    if(selectedItem.item is ArmorItem && playerInventory.GetInventoryItem(selectedItem.item.templateId) != null)
                    {
                        return;
                    }

                    playerInventory.AddItemToInventory(selectedItem.item);
                    playerInventory.RemoveCurrency(selectedItem.currency, selectedItem.price);
                    UpdateCurrencies();

                    // todo - fix this when we refactor, for now just disable the item
                    if (selectedItem.item.templateId == 2)
                    {
                        helmObj.SetActive(false);
                    }
                    else if (selectedItem.item.templateId == 3)
                    {
                        chestObj.SetActive(false);
                    }
                    else if (selectedItem.item.templateId == 4)
                    {
                        glovesObj.SetActive(false);
                    }
                    else if (selectedItem.item.templateId == 5)
                    {
                        legsObj.SetActive(false);
                    }
                    else if (selectedItem.item.templateId == 6)
                    {
                        bootsObj.SetActive(false);
                    }

                    prevSelectedObj.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
                    prevSelectedObj = null;
                    selectedItem.item = null;
                }
            }
        }

        // todo - fix this when we refactor
        public void OnItemSelected(int itemTemplateId)
        {
            if(itemsForSale.ContainsKey(itemTemplateId))
            {
                if (prevSelectedObj != null)
                {
                    prevSelectedObj.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
                }

                GameObject objToChange = null;
                selectedItem = itemsForSale[itemTemplateId];
                if(selectedItem.item.templateId == 0)
                {
                    objToChange = healthPotionObj;
                }
                else if (selectedItem.item.templateId == 1)
                {
                    objToChange = maxHalthPotionObj;
                }
                else if (selectedItem.item.templateId == 2)
                {
                    objToChange = helmObj;
                }
                else if (selectedItem.item.templateId == 3)
                {
                    objToChange = chestObj;
                }
                else if (selectedItem.item.templateId == 4)
                {
                    objToChange = glovesObj;
                }
                else if (selectedItem.item.templateId == 5)
                {
                    objToChange = legsObj;
                }
                else if (selectedItem.item.templateId == 6)
                {
                    objToChange = bootsObj;
                }

                if (objToChange != null)
                {
                    prevSelectedObj = objToChange;
                    objToChange.GetComponentInChildren<RawImage>().color = UnityEngine.Color.green;
                }
            }
        }

        public void UpdateCurrencies()
        {
            PlayerInventory inventory = OverworldManager.Instance.PlayerInventory;
            if (inventory != null)
            {
                currenciesText.text = "Gold: " + inventory.GetCurrencyAmount(DataManager.Currencies.Generic).ToString();
                currenciesText.text += "\nDaunting Inferno Marks: " + inventory.GetCurrencyAmount(DataManager.Currencies.DauntingInferno).ToString();
            }
            else
            {
                currenciesText.text = "ERROR. NO INVENTORY";
            }
        }

        public void ExitMerchantClicked()
        {
            if (prevSelectedObj != null)
            {
                prevSelectedObj.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
            }

            prevSelectedObj = null;
            selectedItem.item = null;

            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }
}