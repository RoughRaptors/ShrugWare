using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class ArmorItem : Item
    {
        public enum ArmorSlot
        {
            Head = 0,
            Chest,
            Gloves,
            Legs,
            Boots
        }

        protected ArmorSlot armorSlot;

        public ArmorItem(ArmorSlot slot)
        {
            armorSlot = slot;
            itemType = DataManager.ItemType.Armor;
        }
    }
}