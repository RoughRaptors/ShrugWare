using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShrugWare
{
    public class RandomEvent : MonoBehaviour
    {
        public int eventID;
        public string eventName;
        public string eventText;
        public List<DataManager.StatEffect> eventEffects;
    }
}
