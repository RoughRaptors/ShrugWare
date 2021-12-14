using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    // todo - make it template id based like the inventory
    public class MerchantManager : MonoBehaviour
    {
        public static MerchantManager Instance = null;
        
        public struct ItemForSale
        {
            public Item item;
            public DataManager.Currencies currency;
            public int price;
        }

        private Dictionary<int, ItemForSale> itemsForSale = new Dictionary<int, ItemForSale>();
        public Dictionary<int, ItemForSale> GetItemsForSale() { return itemsForSale; }

        ItemForSale selectedItem;

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
            healthPotion.templateId = 1;
            healthPotion.AddEffect(healPlayerEffect);

            ItemForSale healthPotionItem;
            healthPotionItem.item = healthPotion;
            healthPotionItem.currency = DataManager.Currencies.Generic;
            healthPotionItem.price = 500;

            itemsForSale.Add(healthPotion.templateId, healthPotionItem);

            // max hp potion
            ConsumableItem maxHealthPotion = new ConsumableItem();
            maxHealthPotion.itemName = "Increase Max Health Potion";
            maxHealthPotion.templateId = 2;
            maxHealthPotion.AddEffect(maxHPEffect);

            ItemForSale maxHealthPotionItem;
            maxHealthPotionItem.item = maxHealthPotion;
            maxHealthPotionItem.currency = DataManager.Currencies.Generic;
            maxHealthPotionItem.price = 500;

            itemsForSale.Add(maxHealthPotion.templateId, maxHealthPotionItem);

            // helmet
            ArmorItem diHelm = new ArmorItem(DataManager.ArmorSlot.Head, DataManager.ArmorSet.DauntingInferno);
            diHelm.itemName = "Helm";
            diHelm.templateId = 3;
            diHelm.AddEffect(damageReductionEffect);

            ItemForSale diHelmItem;
            diHelmItem.item = maxHealthPotion;
            diHelmItem.currency = DataManager.Currencies.DauntingInferno;
            diHelmItem.price = 1000;

            itemsForSale.Add(diHelm.templateId, diHelmItem);

            // chest
            ArmorItem diChest = new ArmorItem(DataManager.ArmorSlot.Chest, DataManager.ArmorSet.DauntingInferno);
            diChest.itemName = "Chest";
            diChest.templateId = 4;
            diChest.AddEffect(damageReductionEffect);

            ItemForSale diChestItem;
            diChestItem.item = maxHealthPotion;
            diChestItem.currency = DataManager.Currencies.DauntingInferno;
            diChestItem.price = 1000;

            itemsForSale.Add(diChest.templateId, diChestItem);

            // gloves
            ArmorItem diGloves = new ArmorItem(DataManager.ArmorSlot.Gloves, DataManager.ArmorSet.DauntingInferno);
            diGloves.itemName = "Gloves";
            diGloves.templateId = 5;
            diGloves.AddEffect(damageReductionEffect);

            ItemForSale diGlovesItem;
            diGlovesItem.item = maxHealthPotion;
            diGlovesItem.currency = DataManager.Currencies.DauntingInferno;
            diGlovesItem.price = 1000;

            itemsForSale.Add(diGloves.templateId, diGlovesItem);

            // legs
            ArmorItem diLegs = new ArmorItem(DataManager.ArmorSlot.Legs, DataManager.ArmorSet.DauntingInferno);
            diLegs.itemName = "Legs";
            diLegs.templateId = 6;
            diLegs.AddEffect(damageReductionEffect);

            ItemForSale diLegsItem;
            diLegsItem.item = maxHealthPotion;
            diLegsItem.currency = DataManager.Currencies.DauntingInferno;
            diLegsItem.price = 1000;

            itemsForSale.Add(diLegs.templateId, diLegsItem);

            // boots
            ArmorItem diBoots = new ArmorItem(DataManager.ArmorSlot.Boots, DataManager.ArmorSet.DauntingInferno);
            diBoots.itemName = "Boots";
            diBoots.templateId = 7;
            diBoots.AddEffect(damageReductionEffect);

            ItemForSale diBootsItem;
            diBootsItem.item = maxHealthPotion;
            diBootsItem.currency = DataManager.Currencies.DauntingInferno;
            diBootsItem.price = 1000;

            itemsForSale.Add(diBoots.templateId, diBootsItem);
        }

        public void OnBuyButtonClicked()
        {
            if (selectedItem.item != null)
            {
                PlayerInventory inventory = GameManager.Instance.GetPlayerInventory();
                if (inventory != null && inventory.GetCurrencyAmount(selectedItem.currency) >= selectedItem.price)
                {
                    inventory.AddItemToInventory(selectedItem.item);
                    inventory.RemoveCurrency(selectedItem.currency, selectedItem.price);
                }
            }
        }

        public void OnItemSelected(int itemTemplateId)
        {
            if(itemsForSale.ContainsKey(itemTemplateId))
            {
                selectedItem = itemsForSale[itemTemplateId];

            }
        }
    }
}