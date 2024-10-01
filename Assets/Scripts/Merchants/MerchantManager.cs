using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        GameObject maxHealthPotionObj = null;

        [SerializeField]
        GameObject moveSpeedPotionObj = null;

        [SerializeField]
        Text currencyInfoText = null;

        [SerializeField]
        GameObject armorTab = null;

        [SerializeField]
        GameObject potionsTab = null;

        [SerializeField]
        GameObject armorTabButton = null;

        [SerializeField]
        GameObject potionsTabButton = null;

        public static MerchantManager Instance = null;
        
        public struct ItemForSale
        {
            public Item item;
            public DataManager.Currencies currency;
            public int price;
        }

        private Dictionary<int, ItemForSale> itemsForSale = new Dictionary<int, ItemForSale>();
        public Dictionary<int, ItemForSale> GetItemsForSale() { return itemsForSale; }

        private List<ItemForSale> selectedItems = new List<ItemForSale>();

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
            UpdateCurrenciesText();

            // default is armor, gray out potions
            potionsTabButton.GetComponentInChildren<RawImage>().color = Color.gray;
            armorTabButton.GetComponentInChildren<Button>().image.color = Color.red;
        }

        private void SetupInventory()
        {
            CreateHealthPotion();
            CreateMaxHealthPotion();
            CreateRunSpeedPotion();

            // +10% damage reduction
            DataManager.StatEffect damageReductionEffect;
            damageReductionEffect.amount = 10;
            damageReductionEffect.effectType = DataManager.StatModifierType.IncomingDamage;
            damageReductionEffect.asPercentage = true;

            CreateHelmet(damageReductionEffect);
            CreateChest(damageReductionEffect);
            CreateGloves(damageReductionEffect);
            CreateLegs(damageReductionEffect);
            CreateBoots(damageReductionEffect);
        }

        private void CreateHealthPotion()
        {
            // 25% player heal
            DataManager.StatEffect healPlayerEffect;
            healPlayerEffect.amount = 25;
            healPlayerEffect.effectType = DataManager.StatModifierType.PlayerCurHealth;
            healPlayerEffect.asPercentage = true;

            ConsumableItem healthPotion = new ConsumableItem();
            healthPotion.itemName = "Health Potion";
            healthPotion.templateId = (int)DataManager.ItemTemplateIds.HealthPotion;
            healthPotion.AddEffect(healPlayerEffect);

            ItemForSale healthPotionItem;
            healthPotionItem.item = healthPotion;
            healthPotionItem.currency = DataManager.Currencies.Gold;
            healthPotionItem.price = 250;
            healthPotionItem.item.itemObj = healthPotionObj;

            itemsForSale.Add(healthPotion.templateId, healthPotionItem);
        }

        private void CreateMaxHealthPotion()
        {
            // +10% max hp
            DataManager.StatEffect maxHPEffect;
            maxHPEffect.amount = 10;
            maxHPEffect.effectType = DataManager.StatModifierType.PlayerMaxHealth;
            maxHPEffect.asPercentage = true;

            ConsumableItem maxHealthPotion = new ConsumableItem();
            maxHealthPotion.itemName = "Increase Max Health Potion";
            maxHealthPotion.templateId = (int)DataManager.ItemTemplateIds.MaxHealthPotion;
            maxHealthPotion.AddEffect(maxHPEffect);

            ItemForSale maxHealthPotionItem;
            maxHealthPotionItem.item = maxHealthPotion;
            maxHealthPotionItem.currency = DataManager.Currencies.Gold;
            maxHealthPotionItem.price = 250;
            maxHealthPotionItem.item.itemObj = maxHealthPotionObj;

            itemsForSale.Add(maxHealthPotion.templateId, maxHealthPotionItem);
        }

        private void CreateRunSpeedPotion()
        {
            // 10% run speed bonus
            DataManager.StatEffect runSpeedEffect;
            runSpeedEffect.amount = 15;
            runSpeedEffect.effectType = DataManager.StatModifierType.PlayerMoveSpeed;
            runSpeedEffect.asPercentage = true;

            ConsumableItem moveSpeedPotion = new ConsumableItem();
            moveSpeedPotion.itemName = "Move Speed Potion";
            moveSpeedPotion.templateId = (int)DataManager.ItemTemplateIds.RunSpeedPotion;
            moveSpeedPotion.AddEffect(runSpeedEffect);

            ItemForSale moveSpeedPotionItem;
            moveSpeedPotionItem.item = moveSpeedPotion;
            moveSpeedPotionItem.currency = DataManager.Currencies.Gold;
            moveSpeedPotionItem.price = 500;
            moveSpeedPotionItem.item.itemObj = moveSpeedPotionObj;

            itemsForSale.Add(moveSpeedPotion.templateId, moveSpeedPotionItem);
        }

        private void CreateHelmet(DataManager.StatEffect damageReductionEffect)
        {
            // helmet
            ArmorItem diHelm = new ArmorItem(DataManager.ArmorSlot.Head, DataManager.ArmorSet.DauntingInferno);
            diHelm.itemName = "Helm";
            diHelm.templateId = (int)DataManager.ItemTemplateIds.Helm;
            diHelm.AddEffect(damageReductionEffect);

            ItemForSale diHelmItem;
            diHelmItem.item = diHelm;
            diHelmItem.currency = DataManager.Currencies.DKP;
            diHelmItem.price = 1000;
            diHelm.itemObj = helmObj;

            if (OverworldManager.Instance != null)
            {
                if (!OverworldManager.Instance.PlayerInventory.HasArmor(diHelm.templateId))
                {
                    itemsForSale.Add(diHelm.templateId, diHelmItem);
                }
                else
                {
                    bootsObj.SetActive(false);
                }
            }
        }

        private void CreateChest(DataManager.StatEffect damageReductionEffect)
        {
            ArmorItem diChest = new ArmorItem(DataManager.ArmorSlot.Chest, DataManager.ArmorSet.DauntingInferno);
            diChest.itemName = "Chest";
            diChest.templateId = (int)DataManager.ItemTemplateIds.Chest;
            diChest.AddEffect(damageReductionEffect);

            ItemForSale diChestItem;
            diChestItem.item = diChest;
            diChestItem.currency = DataManager.Currencies.DKP;
            diChestItem.price = 1000;
            diChest.itemObj = chestObj;

            if (OverworldManager.Instance != null)
            {
                if (!OverworldManager.Instance.PlayerInventory.HasArmor(diChest.templateId))
                {
                    itemsForSale.Add(diChest.templateId, diChestItem);
                }
                else
                {
                    chestObj.SetActive(false);
                }
            }
        }
        private void CreateGloves(DataManager.StatEffect damageReductionEffect)
        {
            ArmorItem diGloves = new ArmorItem(DataManager.ArmorSlot.Gloves, DataManager.ArmorSet.DauntingInferno);
            diGloves.itemName = "Gloves";
            diGloves.templateId = (int)DataManager.ItemTemplateIds.Gloves;
            diGloves.AddEffect(damageReductionEffect);

            ItemForSale diGlovesItem;
            diGlovesItem.item = diGloves;
            diGlovesItem.currency = DataManager.Currencies.DKP;
            diGlovesItem.price = 1000;
            diGloves.itemObj = glovesObj;

            if (OverworldManager.Instance != null)
            {
                if (!OverworldManager.Instance.PlayerInventory.HasArmor(diGloves.templateId))
                {
                    itemsForSale.Add(diGloves.templateId, diGlovesItem);
                }
                else
                {
                    glovesObj.SetActive(false);
                }
            }
        }

        private void CreateLegs(DataManager.StatEffect damageReductionEffect)
        {
            ArmorItem diLegs = new ArmorItem(DataManager.ArmorSlot.Legs, DataManager.ArmorSet.DauntingInferno);
            diLegs.itemName = "Legs";
            diLegs.templateId = (int)DataManager.ItemTemplateIds.Legs;
            diLegs.AddEffect(damageReductionEffect);

            ItemForSale diLegsItem;
            diLegsItem.item = diLegs;
            diLegsItem.currency = DataManager.Currencies.DKP;
            diLegsItem.price = 1000;
            diLegs.itemObj = legsObj;

            if (OverworldManager.Instance != null)
            {
                if (!OverworldManager.Instance.PlayerInventory.HasArmor(diLegs.templateId))
                {
                    itemsForSale.Add(diLegs.templateId, diLegsItem);
                }
                else
                {
                    legsObj.SetActive(false);
                }
            }
        }

        private void CreateBoots(DataManager.StatEffect damageReductionEffect)
        {
            ArmorItem diBoots = new ArmorItem(DataManager.ArmorSlot.Boots, DataManager.ArmorSet.DauntingInferno);
            diBoots.itemName = "Boots";
            diBoots.templateId = (int)DataManager.ItemTemplateIds.Boots;
            diBoots.AddEffect(damageReductionEffect);

            ItemForSale diBootsItem;
            diBootsItem.item = diBoots;
            diBootsItem.currency = DataManager.Currencies.DKP;
            diBootsItem.price = 1000;
            diBoots.itemObj = bootsObj;

            if(OverworldManager.Instance != null)
            {
                if (!OverworldManager.Instance.PlayerInventory.HasArmor(diBoots.templateId))
                {
                    itemsForSale.Add(diBoots.templateId, diBootsItem);
                }
                else
                {
                    bootsObj.SetActive(false);
                }
            }
        }

        public void OnBuyButtonClicked()
        {
            if(selectedItems.Count > 0)
            {
                int totalCost = GetTotalCostForSelectedItems();
                PlayerInventory playerInventory = OverworldManager.Instance.PlayerInventory;

                // if we have enough, iterate again and buy everything
                // used selectedItems[0] because they should all be the same currency
                if (totalCost <= playerInventory.GetCurrencyAmount(selectedItems[0].currency))
                {
                    foreach (ItemForSale selectedItem in selectedItems)
                    {
                        // unselect it
                        selectedItem.item.itemObj.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
                        
                        playerInventory.AddItemToInventory(selectedItem.item);
                        playerInventory.RemoveCurrency(selectedItem.currency, selectedItem.price);

                        // remove the armor from the ui. we don't do this for potions
                        if (selectedItem.item is ArmorItem)
                        {
                            selectedItem.item.itemObj.SetActive(false);
                        }
                    }

                    selectedItems.Clear();
                    UpdateCurrenciesText();
                }
            }
        }

        public void OnItemSelected(int itemTemplateId)
        {
            if(itemsForSale.ContainsKey(itemTemplateId))
            {
                ItemForSale selectedItem = itemsForSale[itemTemplateId];
                GameObject objToChange = selectedItem.item.itemObj;
                if (selectedItems.Contains(selectedItem))
                {
                    // this is already selected, deselect it
                    selectedItems.Remove(selectedItem);
                    objToChange.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
                }
                else
                {
                    // select it
                    selectedItems.Add(selectedItem);
                    objToChange.GetComponentInChildren<RawImage>().color = UnityEngine.Color.green;
                }
            }
        }

        public void UpdateCurrenciesText()
        {
            if (OverworldManager.Instance != null)
            {
                PlayerInventory inventory = OverworldManager.Instance.PlayerInventory;
                if (inventory != null)
                {
                    if (armorTab.activeInHierarchy)
                    {
                        currencyInfoText.text = "DKP: " + inventory.GetCurrencyAmount(DataManager.Currencies.DKP).ToString();
                    }
                    else if (potionsTab.activeInHierarchy)
                    {
                        currencyInfoText.text = "Gold: " + inventory.GetCurrencyAmount(DataManager.Currencies.Gold).ToString();
                    }
                }
                else
                {
                    currencyInfoText.text = "ERROR. NO INVENTORY";
                }
            }
        }

        public void ExitMerchantClicked()
        {
            OverworldManager.Instance.ReadyScene(true);
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        public void OnArmorTabButtonClicked()
        {
            // unselect everything
            foreach(ItemForSale selectedItem in selectedItems)
            {
                selectedItem.item.itemObj.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
            }

            selectedItems.Clear();
            armorTab.SetActive(true);
            potionsTab.SetActive(false);
            armorTabButton.GetComponentInChildren<RawImage>().color = Color.white;
            potionsTabButton.GetComponentInChildren<RawImage>().color = Color.grey;
            armorTabButton.GetComponentInChildren<Button>().image.color = Color.red;
            potionsTabButton.GetComponentInChildren<Button>().image.color = Color.white;
            UpdateCurrenciesText();
        }

        public void OnPotionsTabButtonClicked()
        {
            // unselect everything
            foreach (ItemForSale selectedItem in selectedItems)
            {
                selectedItem.item.itemObj.GetComponentInChildren<RawImage>().color = UnityEngine.Color.white;
            }
            
            selectedItems.Clear();
            armorTab.SetActive(false);
            potionsTab.SetActive(true);
            armorTabButton.GetComponentInChildren<RawImage>().color = Color.grey;
            potionsTabButton.GetComponentInChildren<RawImage>().color = Color.white;
            armorTabButton.GetComponentInChildren<Button>().image.color = Color.white;
            potionsTabButton.GetComponentInChildren<Button>().image.color = Color.red;
            UpdateCurrenciesText();
        }

        private int GetTotalCostForSelectedItems()
        {
            // iterate through and calculate if we have enough to buy everything combined
            int totalCost = 0;
            DataManager.Currencies currency = DataManager.Currencies.Gold;
            PlayerInventory playerInventory = OverworldManager.Instance.PlayerInventory;
            foreach (ItemForSale selectedItem in selectedItems)
            {
                // don't allow multiple purchases of the same armor piece. this shouldn't be hit but have it just in case
                if (selectedItem.item is ArmorItem && playerInventory.GetInventoryItem(selectedItem.item.templateId) != null)
                {
                    continue;
                }

                // should only be one currency per buy action. we have different tabs for different currency types
                currency = selectedItem.currency;
                totalCost += selectedItem.price;
            }

            return totalCost;
        }
    }
}