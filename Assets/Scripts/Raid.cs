using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Raid
    {
        public string raidName { get; set; }

        public int curBossIndex { get; set; }
        public Boss curBoss { get; set; } = null;

        protected List<Boss> bosses = new List<Boss>();
        public List<Boss> GetBosses() { return bosses; }

        protected abstract void InitializeBosses();

        public bool IsComplete { get; set; } = false;

        public Boss GetNextBoss()
        {
            if(curBossIndex < bosses.Count - 1)
            {
                ++curBossIndex;
                curBoss = bosses[curBossIndex];
                return curBoss;
            }
            else
            {
                IsComplete = true;
            }

            return null;
        }
    }
}