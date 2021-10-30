using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Boss : MonoBehaviour
    {
        protected string bossName;
        protected float health;
        protected List<DataManager.Scenes> mechanics = new List<DataManager.Scenes>();
    }
}