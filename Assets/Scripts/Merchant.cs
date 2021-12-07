using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    // todo - make it template id based like the inventory
    public class Merchant : MonoBehaviour
    {
        private struct ItemForSale
        {
            public Item item;
            public DataManager.Currencies currency;
            public int price;
        }

        private List<ItemForSale> itemsForSale = new List<ItemForSale>();

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
            healthPotion.AddEffect(healPlayerEffect);

            ItemForSale healthPotionItem;
            healthPotionItem.item = healthPotion;
            healthPotionItem.currency = DataManager.Currencies.Generic;
            healthPotionItem.price = 100;

            itemsForSale.Add(healthPotionItem);

            // max hp potion
            ConsumableItem maxHealthPotion = new ConsumableItem();
            maxHealthPotion.itemName = "Increase Max Health Potion";
            maxHealthPotion.AddEffect(maxHPEffect);

            ItemForSale maxHealthPotionItem;
            maxHealthPotionItem.item = maxHealthPotion;
            maxHealthPotionItem.currency = DataManager.Currencies.Generic;
            maxHealthPotionItem.price = 100;

            itemsForSale.Add(maxHealthPotionItem);

            // helmet
            ArmorItem diHelm = new ArmorItem(ArmorItem.ArmorSlot.Head);
            diHelm.AddEffect(damageReductionEffect);

            ItemForSale diHelmItem;
            diHelmItem.item = maxHealthPotion;
            diHelmItem.currency = DataManager.Currencies.DauntingInferno;
            diHelmItem.price = 1000;

            itemsForSale.Add(diHelmItem);

            // chest
            ArmorItem diChest = new ArmorItem(ArmorItem.ArmorSlot.Chest);
            diChest.AddEffect(damageReductionEffect);

            ItemForSale diChestItem;
            diChestItem.item = maxHealthPotion;
            diChestItem.currency = DataManager.Currencies.DauntingInferno;
            diChestItem.price = 1000;

            itemsForSale.Add(diChestItem);

            // gloves
            ArmorItem diGloves = new ArmorItem(ArmorItem.ArmorSlot.Gloves);
            diGloves.AddEffect(damageReductionEffect);

            ItemForSale diGlovesItem;
            diGlovesItem.item = maxHealthPotion;
            diGlovesItem.currency = DataManager.Currencies.DauntingInferno;
            diGlovesItem.price = 1000;

            itemsForSale.Add(diGlovesItem);

            // legs
            ArmorItem diLegs = new ArmorItem(ArmorItem.ArmorSlot.Legs);
            diLegs.AddEffect(damageReductionEffect);

            ItemForSale diLegsItem;
            diLegsItem.item = maxHealthPotion;
            diLegsItem.currency = DataManager.Currencies.DauntingInferno;
            diLegsItem.price = 1000;

            itemsForSale.Add(diLegsItem);

            // boots
            ArmorItem diBoots = new ArmorItem(ArmorItem.ArmorSlot.Boots);
            diBoots.AddEffect(damageReductionEffect);

            ItemForSale diBootsItem;
            diBootsItem.item = maxHealthPotion;
            diBootsItem.currency = DataManager.Currencies.DauntingInferno;
            diBootsItem.price = 1000;

            itemsForSale.Add(diBootsItem);
        }

        bool PurchaseItem(ItemForSale itemToPurchase)
        {
            PlayerInventory inventory = GameManager.Instance.GetPlayerInventory();
            if (inventory != null && inventory.GetCurrencyAmount(itemToPurchase.currency) >= itemToPurchase.price)
            {
                inventory.AddItemToInventory(itemToPurchase.item);
                inventory.RemoveCurrency(itemToPurchase.currency, itemToPurchase.price);

                return true;
            }

            return false;
        }
    }
}