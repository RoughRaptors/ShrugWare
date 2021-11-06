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

            WarbossKard warbossKard = new WarbossKard();
            bosses.Add(warbossKard);

            curBoss = warbossKard;
        }
    }
}