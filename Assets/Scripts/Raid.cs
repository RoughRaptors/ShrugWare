using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Raid
    {
        public string raidName { get; set; }

        public Boss curBoss { get; set; } = null;

        protected List<Boss> bosses = new List<Boss>();
        public List<Boss> GetBosses() { return bosses; }
    }
}