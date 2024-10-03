using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class ArmorItem : Item
    {
        protected DataManager.ArmorSlot armorSlot;
        public DataManager.ArmorSlot GetArmorSlot() { return armorSlot; }

        protected DataManager.ArmorSet armorSet;
        public DataManager.ArmorSet GetArmorSet() { return armorSet; }

        private List<DataManager.StatEffect> setBonuses = new List<DataManager.StatEffect>();
        public List<DataManager.StatEffect> GetSetBonuses() { return setBonuses; }

        public ArmorItem(DataManager.ArmorSlot newArmorSlot, DataManager.ArmorSet newArmorSet)
        {
            armorSlot = newArmorSlot;
            armorSet = newArmorSet;
            itemType = DataManager.ItemType.Armor;

            // hard code our set bonus for daunting inferno right now
            if(armorSet == DataManager.ArmorSet.DauntingInferno)
            {
                // +10% max hp
                DataManager.StatEffect maxHPEffect;
                maxHPEffect.amount = 10;
                maxHPEffect.effectType = DataManager.StatModifierType.PlayerMaxHealth;
                maxHPEffect.asPercentage = true;
                maxHPEffect.effectDescriptionString = "\n\n5 Piece Set Bonus:\n+10% Bonus Health";

                setBonuses.Add(maxHPEffect);
            }
        }
    }
}