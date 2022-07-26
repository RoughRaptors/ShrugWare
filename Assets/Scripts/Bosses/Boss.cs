using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Boss
    {
        public string bossName { get; set; }

        public bool isDead { get; set; } = false;
        public float curHealth { get; set; }
        public float maxHealth { get; set; }

        // the microgames associated with this boss
        protected List<DataManager.MicrogameScenes> mechanics = new List<DataManager.MicrogameScenes>();

        protected abstract void PopulateMechanicsList();

        public void TakeDamage(float amount)
        {
            curHealth -= amount;
            if (curHealth <= 0)
            {
                isDead = true;
            }
        }

        public DataManager.MicrogameScenes PickNextMicrogame()
        {
            if(mechanics.Count == 0)
            {
                PopulateMechanicsList();
            }

            // randomly grab one and remove it from our list
            int microgameSceneIndex = Random.Range(0, mechanics.Count); // exclusive max
            DataManager.MicrogameScenes scene = mechanics[microgameSceneIndex];
            mechanics.RemoveAt(microgameSceneIndex);

            return scene;
        }
    }
}