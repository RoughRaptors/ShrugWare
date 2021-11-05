using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Boss : MonoBehaviour
    {
        protected string bossName;
        protected float health;

        private int curMechanicIndex = -1;
        protected List<DataManager.Scenes> mechanics = new List<DataManager.Scenes>();

        public void TakeDamage(float amount)
        {
            health -= amount;
            if(health <= 0)
            {

            }
        }
    }
}