using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ShrugWare
{
    public class PlayerInventory
    {
        private Dictionary<DataManager.Currencies, int> currencies = new Dictionary<DataManager.Currencies, int>();
        private Dictionary<int, Item> inventoryItems = new Dictionary<int, Item>();
        private Dictionary<DataManager.ArmorSlot, ArmorItem> equippedArmor = new Dictionary<DataManager.ArmorSlot, ArmorItem>(5);

        private float curMitigationPercent = 0.0f;

        public PlayerInventory()
        {
            // starting inventory
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
            healthPotionItem.templateId = 0;
            healthPotionItem.AddEffect(healPlayerEffect);
            inventoryItems.Add(healthPotionItem.templateId, healthPotionItem);

            ConsumableItem maxHealthPotionItem = new ConsumableItem();
            maxHealthPotionItem.itemName = "Increase Max Health Potion";
            maxHealthPotionItem.itemQuantity = 1;
            maxHealthPotionItem.templateId = 1;
            maxHealthPotionItem.AddEffect(maxHPEffect);
            inventoryItems.Add(maxHealthPotionItem.templateId, maxHealthPotionItem);

            foreach (DataManager.ArmorSlot slot in Enum.GetValues(typeof(DataManager.ArmorSlot)))
            {
                equippedArmor[DataManager.ArmorSlot.Head] = null;
                equippedArmor[DataManager.ArmorSlot.Chest] = null;
                equippedArmor[DataManager.ArmorSlot.Gloves] = null;
                equippedArmor[DataManager.ArmorSlot.Legs] = null;
                equippedArmor[DataManager.ArmorSlot.Boots] = null;
            }
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
                inventoryItems.Add(itemToAdd.templateId, itemToAdd);
            }

            // if this is armor, auto equip it
            if(itemToAdd is ArmorItem)
            {
                EquipArmorItem(itemToAdd as ArmorItem);
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

            if (equippedArmor.ContainsKey(armorToEquip.GetArmorSlot()))
            {
                equippedArmor[armorToEquip.GetArmorSlot()] = armorToEquip;
                RecalculateStats();
            }
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

        public void RecalculateStats()
        {
            if(BossGameManager.Instance == null)
            {
                return;
            }

            curMitigationPercent = 0;
            BossGameManager.Instance.ResetMaxHP();

            if (HasSetBonus())
            {
                // we have a full set, grab any piece's set bonus since they are all the same and we only need 1
                if (equippedArmor[DataManager.ArmorSlot.Head] != null)
                {
                    List<DataManager.StatEffect> setBonuses = equippedArmor[DataManager.ArmorSlot.Head].GetSetBonuses();
                    foreach (DataManager.StatEffect effect in setBonuses)
                    {
                        if (effect.effectType == DataManager.StatModifierType.IncomingDamage)
                        {
                            curMitigationPercent += effect.amount;
                        }
                        else if (effect.effectType == DataManager.StatModifierType.PlayerMaxHealth)
                        {
                            int effectAmount = (int)(BossGameManager.Instance.GetPlayerInfo().maxPlayerHealth * (effect.amount / 100.0f));
                            BossGameManager.Instance.AddToMaxHP(effectAmount);
                            BossGameManager.Instance.HealPlayerRaid(effectAmount);
                        }
                    }
                }
            }

            foreach (KeyValuePair<DataManager.ArmorSlot, ArmorItem> armor in equippedArmor)
            {
                if (armor.Value != null)
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

        public bool UseConsumableItem(int templateId)
        {
            bool usedConsumable = false;
            Item item = null;
            inventoryItems.TryGetValue(templateId, out item);

            if (item != null)
            {
                ConsumableItem consumableItem = item as ConsumableItem;
                if (consumableItem != null && consumableItem.itemQuantity > 0)
                {
                    usedConsumable = consumableItem.UseItem();
                    if(usedConsumable)
                    {
                        --consumableItem.itemQuantity;
                        if (consumableItem.itemQuantity == 0)
                        {
                            // inventoryItems.Remove(consumableItem.templateId);
                        }

                        AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.UsePotion, 0.4f);
                        BossUIManager.Instance.UpdateConsumableInfo();
                        BossGameManager.Instance.UpdateGameUI();
                    }
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
                if (armor.Value != null)
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
            }

            return numSameArmorSetEquipped == (int)DataManager.ArmorSlot.MAX + 1;
        }

        public Item GetInventoryItem(int templateId)
        {
            Item item = null;
            inventoryItems.TryGetValue(templateId, out item);

            return item;
        }
    }
}