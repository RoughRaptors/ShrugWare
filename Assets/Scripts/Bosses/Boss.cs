using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShrugWare
{
    public abstract class Boss
    {
        public string bossName { get; set; }
        public bool isDead { get; set; } = false;
        public float curHealth { get; set; }
        public float maxHealth { get; set; }

        // the microgames associated with this boss
        protected List<DataManager.Scenes> mechanics = new List<DataManager.Scenes>();

        protected abstract void PopulateMechanicsList();

        public void TakeDamage(float amount)
        {
            curHealth -= amount;
            if (curHealth <= 0)
            {
                isDead = true;
            }
        }

        public DataManager.Scenes PickNextMicrogame()
        {
            if(mechanics.Count == 0)
            {
                PopulateMechanicsList();
            }

            // randomly grab one and remove it from our list
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            int microgameSceneIndex = UnityEngine.Random.Range(0, mechanics.Count); // exclusive max
            DataManager.Scenes scene = mechanics[microgameSceneIndex];
            mechanics.RemoveAt(microgameSceneIndex);

            return scene;
        }

        public static Boss InitializeBoss(string bossName)
        {
            Boss newBoss = null;
            if (bossName == "Warboss Kard")
            {
                newBoss = new WarbossKard();
            }
            else if (bossName == "Tuzi")
            {
                newBoss = new Tuzi();
            }

            return newBoss;
        }
    }
}