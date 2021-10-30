using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class WarbossKard : Boss
    {
        // Start is called before the first frame update
        void Start()
        {
            bossName = "Warboss Kard";
            health = 100.0f;
            mechanics.Add(DataManager.Scenes.DauntingInfernoGetOutOfFire);
        }

        public void StartBoss()
        {
            foreach(DataManager.Scenes scene in mechanics)
            {

            }
        }
    }
}