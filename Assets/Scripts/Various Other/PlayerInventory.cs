using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace ShrugWare
{
    public class PlayerInventory
    {
        private Dictionary<DataManager.Currencies, int> currencies = new Dictionary<DataManager.Currencies, int>();
        private Dictionary<int, Item> inventoryItems = new Dictionary<int, Item>();
        private Dictionary<DataManager.ArmorSlot, ArmorItem> equippedArmor = new Dictionary<DataManager.ArmorSlot, ArmorItem>(6);

        private float curMitigationPercent = 0.0f;
        private float moveSpeedBonus = 0.0f;
        private float maxHealthBonus = 0.0f;

        public PlayerInventory()
        {
            // starting inventory
            currencies.Add(DataManager.Currencies.Gold, 500);
            currencies.Add(DataManager.Currencies.DKP, 2500);

            foreach (DataManager.ArmorSlot slot in Enum.GetValues(typeof(DataManager.ArmorSlot)))
            {
                equippedArmor[DataManager.ArmorSlot.Head] = null;
                equippedArmor[DataManager.ArmorSlot.Chest] = null;
                equippedArmor[DataManager.ArmorSlot.Gloves] = null;
                equippedArmor[DataManager.ArmorSlot.Legs] = null;
                equippedArmor[DataManager.ArmorSlot.Boots] = null;
                equippedArmor[DataManager.ArmorSlot.Accessory] = null;
            }

            CreateHealthPotion();
            CreateMaxHealthPotion();
            CreateMoveSpeedPotion();
            CreateAccessory();
        }

        private void CreateHealthPotion()
        {
            // 25% player heal
            DataManager.StatEffect healPlayerEffect;
            healPlayerEffect.amount = 25;
            healPlayerEffect.effectType = DataManager.StatModifierType.PlayerCurHealth;
            healPlayerEffect.asPercentage = true;
            healPlayerEffect.effectDescriptionString = healPlayerEffect.amount.ToString() + "% Heal";

            ConsumableItem healthPotionItem = new ConsumableItem();
            healthPotionItem.itemName = "Health Potion";
            healthPotionItem.itemQuantity = 1;
            healthPotionItem.templateId = (int)DataManager.ItemTemplateIds.HealthPotion;
            healthPotionItem.AddEffect(healPlayerEffect);
            inventoryItems.Add(healthPotionItem.templateId, healthPotionItem);
        }

        private void CreateMaxHealthPotion()
        {
            // +10% max hp
            DataManager.StatEffect maxHPEffect;
            maxHPEffect.amount = 10;
            maxHPEffect.effectType = DataManager.StatModifierType.PlayerMaxHealth;
            maxHPEffect.asPercentage = true;
            maxHPEffect.effectDescriptionString = maxHPEffect.amount.ToString() + "% Max Health";

            ConsumableItem maxHealthPotionItem = new ConsumableItem();
            maxHealthPotionItem.itemName = "Increase Max Health Potion";
            maxHealthPotionItem.itemQuantity = 1;
            maxHealthPotionItem.templateId = (int)DataManager.ItemTemplateIds.MaxHealthPotion;
            maxHealthPotionItem.AddEffect(maxHPEffect);
            inventoryItems.Add(maxHealthPotionItem.templateId, maxHealthPotionItem);
        }

        private void CreateMoveSpeedPotion()
        {
            // +15% move speed
            DataManager.StatEffect moveSpeedEffect;
            moveSpeedEffect.amount = 15;
            moveSpeedEffect.effectType = DataManager.StatModifierType.PlayerMoveSpeed;
            moveSpeedEffect.asPercentage = true;
            moveSpeedEffect.effectDescriptionString = moveSpeedEffect.amount.ToString() + "% Move Speed";

            ConsumableItem moveSpeedPotionItem = new ConsumableItem();
            moveSpeedPotionItem.itemName = "Move Speed Potion";
            moveSpeedPotionItem.itemQuantity = 0;
            moveSpeedPotionItem.templateId = (int)DataManager.ItemTemplateIds.RunSpeedPotion;
            moveSpeedPotionItem.AddEffect(moveSpeedEffect);
            inventoryItems.Add(moveSpeedPotionItem.templateId, moveSpeedPotionItem);
        }

        private void CreateAccessory()
        {
            DataManager.StatEffect microgameTimeEffect;
            microgameTimeEffect.amount = 20;
            microgameTimeEffect.effectType = DataManager.StatModifierType.MicrogameTime;
            microgameTimeEffect.asPercentage = true;
            microgameTimeEffect.effectDescriptionString = "";

            DataManager.StatEffect livesEffect;
            livesEffect.amount = 1;
            livesEffect.effectType = DataManager.StatModifierType.Lives;
            livesEffect.asPercentage = false;
            livesEffect.effectDescriptionString = "";

            DataManager.StatEffect incomingDamageEffect;
            incomingDamageEffect.amount = 25;
            incomingDamageEffect.effectType = DataManager.StatModifierType.IncomingDamage;
            incomingDamageEffect.asPercentage = true;
            incomingDamageEffect.effectDescriptionString = "";

            ArmorItem accessory = new ArmorItem(DataManager.ArmorSlot.Accessory, DataManager.ArmorSet.DauntingInferno);
            accessory.itemName = "Skull of the Noob";
            accessory.templateId = (int)DataManager.ItemTemplateIds.Accessory;
            accessory.AddEffect(microgameTimeEffect);
            accessory.AddEffect(livesEffect);
            accessory.AddEffect(incomingDamageEffect);

            inventoryItems.Add(accessory.templateId, accessory);
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

            // if this is armor that isn't an accessory, auto equip it
            if(itemToAdd is ArmorItem && (itemToAdd as ArmorItem).GetArmorSlot() != DataManager.ArmorSlot.Accessory)
            {
                EquipArmorItem(itemToAdd as ArmorItem);
            }
        }

        public void EquipArmorItem(ArmorItem armorToEquip)
        {
            if (equippedArmor.ContainsKey(armorToEquip.GetArmorSlot()))
            {
                equippedArmor[armorToEquip.GetArmorSlot()] = armorToEquip;
                inventoryItems.Remove(armorToEquip.templateId);
                RecalculateStats();
            }
        }

        public void EquipItemInArmorSlot(int itemTemplateId, DataManager.ArmorSlot slot)
        {
            Item item = GetInventoryItem(itemTemplateId);
            if (item != null)
            {
                if (equippedArmor.ContainsKey(slot) && equippedArmor[slot] == null)
                {
                    equippedArmor[slot] = item as ArmorItem;
                    inventoryItems.Remove(item.templateId);
                }

                RecalculateStats();
            }
        }

        public void UnequipArmorSlot(DataManager.ArmorSlot slot)
        {
            ArmorItem itemToUnequip = null;
            if (equippedArmor.TryGetValue(slot, out itemToUnequip))
            {
                if (itemToUnequip != null)
                {
                    // if we already have the item in our inventory, increase the quantity, otherwise add it
                    if (inventoryItems.ContainsKey(itemToUnequip.templateId))
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
        }

        public void RecalculateStats()
        {
            if(BossGameManager.Instance != null)
            {
                BossGameManager.Instance.ResetPlayerRaidMaxHP();
                BossGameManager.Instance.ResetMicrogameTimeBonus();
                BossGameManager.Instance.ResetLivesBonus();
            }

            curMitigationPercent = 0;

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
                            maxHealthBonus += effect.amount;
                            if (BossGameManager.Instance != null)
                            {
                                float effectAmount = (BossGameManager.Instance.GetPlayerInfo().maxPlayerHealth * (effect.amount / 100.0f));
                                BossGameManager.Instance.AddToPlayerRaidMaxHP((int)effectAmount);
                                BossGameManager.Instance.HealPlayerRaid((int)effectAmount);
                            }
                        }
                    }
                }
            }

            float microgameTimeBonus = 0.0f;
            int extraLives = 0;
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
                        else if (effect.effectType == DataManager.StatModifierType.MicrogameTime)
                        {
                            microgameTimeBonus += effect.amount;
                        }
                        else if(effect.effectType == DataManager.StatModifierType.Lives)
                        {
                            extraLives += (int)effect.amount;
                        }
                    }
                }
            }

            if (BossGameManager.Instance != null)
            {
                BossGameManager.Instance.AddToMicrogameTimeBonus(microgameTimeBonus);
                BossGameManager.Instance.AddLives(extraLives);
                BossGameManager.Instance.UpdateGameUI();
            }
        }

        public void AddCurrency(DataManager.Currencies currency, int amount)
        {
            if (currencies.ContainsKey(currency))
            {
                currencies[currency] += amount;
            }
            else
            {
                currencies.Add(currency, amount);
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

                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayAudioClip(DataManager.AudioEffectTypes.UsePotion, 0.4f);
                        }

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

        public void SetMoveSpeedBonus(float amount)
        {
            moveSpeedBonus = amount;
        }

        public float GetMoveSpeedBonus()
        {
            return moveSpeedBonus;
        }

        public bool HasSetBonus()
        {
            // check for armor set - this can be dumb and just check for 5 of the same (not accessory)
            int numSameArmorSetEquipped = 0;
            DataManager.ArmorSet prevSet = DataManager.ArmorSet.DauntingInferno;
            foreach (KeyValuePair<DataManager.ArmorSlot, ArmorItem> armor in equippedArmor)
            {
                if (armor.Value != null && armor.Value.GetArmorSlot() != DataManager.ArmorSlot.Accessory)
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

            return numSameArmorSetEquipped == (int)DataManager.ArmorSlot.MAX;
        }

        public Item GetInventoryItem(int templateId)
        {
            Item item = null;
            inventoryItems.TryGetValue(templateId, out item);

            return item;
        }

        public ArmorItem GetEquippedItem(DataManager.ArmorSlot slot)
        {
            ArmorItem item = null;
            equippedArmor.TryGetValue(slot, out item);

            return item;
        }

        public bool HasArmor(int templateId)
        {
            foreach (KeyValuePair<DataManager.ArmorSlot, ArmorItem> armor in equippedArmor)
            {
                if(armor.Value != null && armor.Value.templateId == templateId)
                {
                    return true;
                }
            }

            return false;
        }

        // equip on casual unequip otherwise
        public void DifficultyChanged(bool casualMode)
        {
            if(casualMode)
            {
                Item accessoryItem = null;
                inventoryItems.TryGetValue((int)DataManager.ItemTemplateIds.Accessory, out accessoryItem);
                if(accessoryItem != null)
                {
                    EquipArmorItem(accessoryItem as ArmorItem);
                }
            }
            else
            {
                UnequipArmorSlot(DataManager.ArmorSlot.Accessory);
            }

            RecalculateStats();
        }
    }
}