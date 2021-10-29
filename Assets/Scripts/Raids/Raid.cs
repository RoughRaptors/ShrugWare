using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Raid : MonoBehaviour
    {
        protected string raidName;
        protected List<Boss> bosses = new List<Boss>();
    }
}