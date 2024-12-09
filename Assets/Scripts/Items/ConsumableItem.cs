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
                    return BossGameManager.Instance.HealPlayerRaid((int)effect.amount);
                }
                else if (effect.effectType == DataManager.StatModifierType.PlayerMaxHealth)
                {
                    float amount = (effect.amount * DataManager.PLAYER_MAX_HP_BOSSENCOUNTER) / 100.0f;
                    BossGameManager.Instance.AddToPlayerRaidMaxHP((int)amount);
                    BossGameManager.Instance.HealPlayerRaid((int)amount);
                    return true;
                }
                else if (effect.effectType == DataManager.StatModifierType.PlayerMoveSpeed)
                {
                    //OverworldManager.Instance.PlayerInventory.SetMoveSpeedBonus(effect.amount);
                    OverworldManager.Instance.PlayerInventory.AddToMoveSpeedBonus(effect.amount);
                    return true;
                }
            }

            return false;
        }
    }
}