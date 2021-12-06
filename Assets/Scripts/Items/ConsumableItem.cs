using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class ConsumableItem : Item
    {
        public ConsumableItem()
        {
            itemType = DataManager.ItemType.Consumable;
        }

        public void UseItem()
        {
            foreach(DataManager.StatEffect effect in effects)
            {
                if(effect.effectType == DataManager.StatModifierType.PlayerHealth)
                {
                    
                }
            }
        }
    }
}