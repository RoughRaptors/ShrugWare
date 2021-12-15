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

        public bool UseItem()
        {
            foreach(DataManager.StatEffect effect in effects)
            {
                if(effect.effectType == DataManager.StatModifierType.PlayerCurHealth)
                {
                    return GameManager.Instance.HealPlayerRaid((int)effect.amount);
                }
                else if (effect.effectType == DataManager.StatModifierType.PlayerMaxHealth)
                {
                    float amount = (effect.amount * DataManager.PLAYER_RAID_MAX_HP) / 100.0f;
                    GameManager.Instance.AddToMaxHP((int)amount);
                    GameManager.Instance.HealPlayerRaid((int)amount);
                    return true;
                }
            }

            return false;
        }
    }
}