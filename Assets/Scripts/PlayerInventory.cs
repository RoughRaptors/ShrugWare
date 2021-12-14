using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class PlayerInventory
    {
        private Dictionary<DataManager.Currencies, int> currencies = new Dictionary<DataManager.Currencies, int>();
        private Dictionary<int, Item> inventoryItems = new Dictionary<int, Item>();
        private Dictionary<DataManager.ArmorSlot, ArmorItem> equippedArmor = new Dictionary<DataManager.ArmorSlot, ArmorItem>();

        private float curMitigationPercent = 0.0f;

        public PlayerInventory()
        {
            currencies.Add(DataManager.Currencies.Generic, 1000);
            currencies.Add(DataManager.Currencies.DauntingInferno, 5000);

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

        public void EquipArmorItem(ArmorItem armorToEquip)
        {
            foreach(DataManager.StatEffect effect in armorToEquip.GetEffects())
            {
                curMitigationPercent += effect.amount;
                if(curMitigationPercent > 100.0f)
                {
                    curMitigationPercent = 100.0f;
                }
            }

            equippedArmor[armorToEquip.GetArmorSlot()] = armorToEquip;
            RecalculateStats();
        }

        public void UnequipArmorSlot(DataManager.ArmorSlot slot)
        {
            ArmorItem itemToUnequip = null;
            if(equippedArmor.TryGetValue(slot, out itemToUnequip))
            {
                // if we already have the item in our inventory, increase the quantity, otherwise add it
                if(inventoryItems.ContainsKey(itemToUnequip.templateId))
                {
                    ++inventoryItems[itemToUnequip.templateId].itemQuantity;
                }
                else
                {
                    inventoryItems.Add(itemToUnequip.templateId, itemToUnequip);
                }

                equippedArmor[slot] = null;
                RecalculateStats();
            }
        }

        private void RecalculateStats()
        {
            curMitigationPercent = 0;
            GameManager.Instance.ResetMaxHP();

            if (HasSetBonus())
            {
                // we have a full set, grab any piece's set bonus since they are all the same and we only need 1
                List<DataManager.StatEffect> setBonuses = equippedArmor[DataManager.ArmorSlot.Head].GetSetBonuses();
                foreach (DataManager.StatEffect effect in setBonuses)
                {
                    if (effect.effectType == DataManager.StatModifierType.IncomingDamage)
                    {
                        curMitigationPercent += effect.amount;
                    }
                    else if(effect.effectType == DataManager.StatModifierType.PlayerMaxHealth)
                    {
                        GameManager.Instance.AddToMaxHP((int)(GameManager.Instance.GetPlayerInfo().maxRaidHealth * effect.amount));
                    }
                }
            }

            foreach (KeyValuePair<DataManager.ArmorSlot, ArmorItem> armor in equippedArmor)
            {
                foreach (DataManager.StatEffect effect in armor.Value.GetEffects())
                {
                    if (effect.effectType == DataManager.StatModifierType.IncomingDamage)
                    {
                        curMitigationPercent += effect.amount;
                    }
                }
            }
        }

        public void RemoveCurrency(DataManager.Currencies currency, int amount)
        {
            if(currencies.ContainsKey(currency))
            {
                currencies[currency] -= amount;

                if(currencies[currency] < 0)
                {
                    currencies[currency] = 0;
                    Debug.Log("Currency " + currency.ToString() + " is below 0");
                }
            }
        }

        public bool UseConsumable(ConsumableItem consumable)
        {
            bool usedConsumable = false;
            if (consumable != null)
            {
                if (usedConsumable)
                {
                    --consumable.itemQuantity;
                    if (consumable.itemQuantity == 0)
                    {
                        inventoryItems.Remove(consumable.templateId);
                    }

                    consumable.UseItem();
                    usedConsumable = true;
                }
            }

            return usedConsumable;
        }


        public float GetMitigation() 
        {
            return curMitigationPercent;
        }

        public bool HasSetBonus()
        {
            // check for armor set - this can be dumb and just check for 5 of the same
            int numSameArmorSetEquipped = 0;
            DataManager.ArmorSet prevSet = DataManager.ArmorSet.DauntingInferno;
            foreach (KeyValuePair<DataManager.ArmorSlot, ArmorItem> armor in equippedArmor)
            {
                // if we have none, this means we haven't started yet so set our set to the current piece of armor
                if (numSameArmorSetEquipped == 0)
                {
                    prevSet = armor.Value.GetArmorSet();
                }

                // then if we detect a match of pieces, increment the number of matches
                if (prevSet == armor.Value.GetArmorSet())
                {
                    ++numSameArmorSetEquipped;
                }
                else
                {
                    // not a match, therefore not a full set
                    break;
                }
            }

            return numSameArmorSetEquipped == (int)DataManager.ArmorSlot.MAX + 1;
        }
    }
}