using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class PlayerInventory
    {
        private Dictionary<DataManager.Currencies, int> currencies = new Dictionary<DataManager.Currencies, int>();
        private Dictionary<int, Item> inventoryItems = new Dictionary<int, Item>();
        private Dictionary<ArmorItem.ArmorSlot, Item> equippedArmor = new Dictionary<ArmorItem.ArmorSlot, Item>();

        private float curMitigationPercent = 0.0f;
        public float GetMitigation() { return curMitigationPercent; }

        public PlayerInventory()
        {
            currencies.Add(DataManager.Currencies.Generic, 250);
            currencies.Add(DataManager.Currencies.DauntingInferno, 0);

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

            // todo - untie from Merchant template ids by making items data driven
            ConsumableItem healthPotionItem = new ConsumableItem();
            healthPotionItem.itemName = "Health Potion";
            healthPotionItem.itemQuantity = 1;
            healthPotionItem.templateId = 1;
            healthPotionItem.AddEffect(healPlayerEffect);
            inventoryItems.Add(healthPotionItem.templateId, healthPotionItem);

            ConsumableItem maxHealthPotionItem = new ConsumableItem();
            maxHealthPotionItem.itemName = "Increase Max Health Potion";
            maxHealthPotionItem.itemQuantity = 1;
            maxHealthPotionItem.templateId = 2;
            maxHealthPotionItem.AddEffect(maxHPEffect);
            inventoryItems.Add(maxHealthPotionItem.templateId, maxHealthPotionItem);
        }
        
        public int GetCurrencyAmount(DataManager.Currencies currencyType)
        {
            int amount = 0;
            currencies.TryGetValue(currencyType, out amount);

            return amount;
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            Item foundItem = null;
            if (inventoryItems.TryGetValue(itemToAdd.templateId, out foundItem))
            {
                foundItem.itemQuantity++;
            }
            else
            {
                inventoryItems.Add(foundItem.templateId, foundItem);
            }
        }

        public void EquipArmorItem(Item itemToEquip)
        {
            foreach(DataManager.StatEffect effect in itemToEquip.GetEffects())
            {
                curMitigationPercent += effect.amount;
            }
        }

        public void UnequipArmorSlot(ArmorItem.ArmorSlot slot)
        {
            Item itemToUnequip = null;
            if(equippedArmor.TryGetValue(slot, out itemToUnequip))
            {
                // remove the effects
                foreach (DataManager.StatEffect effect in itemToUnequip.GetEffects())
                {
                    if(effect.effectType == DataManager.StatModifierType.IncomingDamage)
                    {
                        curMitigationPercent += effect.amount;
                    }
                }

                // if we already have the item in our inventory, increase the quantity, otherwise add it
                if(inventoryItems.TryGetValue(itemToUnequip.templateId, out itemToUnequip))
                {
                    ++itemToUnequip.itemQuantity;
                }
                else
                {
                    inventoryItems.Add(itemToUnequip.templateId, itemToUnequip);
                }

                equippedArmor[slot] = null;
            }
        }
    }
}