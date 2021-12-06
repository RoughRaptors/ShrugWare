using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Item
    {
        public string itemName { get; set; } = "DUMMY";
        public DataManager.ItemType itemType { get; protected set; }
        public int itemQuantity { get; set; } = 1;

        protected List<DataManager.StatEffect> effects = new List<DataManager.StatEffect>();
        public void AddEffect(DataManager.StatEffect effect)
        {
            effects.Add(effect);
        }
    }
}