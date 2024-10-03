using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField]
        string tooltipMessage;

        [SerializeField]
        DataManager.ArmorSlot armorSlot;

        [SerializeField]
        int templateId = -1;

        private void OnMouseEnter()
        {
            tooltipMessage = "";
            if(OverworldManager.Instance != null)
            {
                if (armorSlot != DataManager.ArmorSlot.NONE)
                {
                    ArmorItem armorItem = OverworldManager.Instance.PlayerInventory.GetEquippedItem(armorSlot);
                    if (armorItem != null)
                    {
                        foreach(DataManager.StatEffect statEffect in armorItem.GetEffects())
                        {
                            tooltipMessage += statEffect.effectDescriptionString + "\n";
                        }
                    }
                }
                else if(templateId != -1)
                {
                    ConsumableItem consumableItem = OverworldManager.Instance.PlayerInventory.GetInventoryItem(templateId) as ConsumableItem;
                    if(consumableItem != null)
                    {
                        foreach (DataManager.StatEffect statEffect in consumableItem.GetEffects())
                        {
                            tooltipMessage += statEffect.effectDescriptionString + "\n";
                        }
                    }
                }
            }

            if (tooltipMessage != "")
            {
                TooltipManager.Instance.SetAndShowTooltip(tooltipMessage);
            }
        }

        private void OnMouseExit()
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}