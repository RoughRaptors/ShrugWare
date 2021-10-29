using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class InfernalDawn : Raid
    {
        // Start is called before the first frame update
        void Start()
        {
            raidName = "Infernal Dawn";

            WarbossKard warbossKard = new WarbossKard();
            bosses.Add(warbossKard);
        }

        public void StartRaid()
        {
            foreach(Boss boss in bosses)
            {

            }
        }
    }
}