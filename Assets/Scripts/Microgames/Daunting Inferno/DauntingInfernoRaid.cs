using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class DauntingInferno : Raid
    {
        public DauntingInferno()
        {
            raidName = "Daunting Inferno";

            InitializeBosses();
        }

        protected override void InitializeBosses()
        {
            WarbossKard warbossKard = new WarbossKard();
            bosses.Add(warbossKard);

            Tuzi tuzi = new Tuzi();
            bosses.Add(tuzi);

            curBoss = warbossKard;
            curBossIndex = 0;
        }
    }
}