using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class WarbossKard : Boss
    {
        // Start is called before the first frame update
        public WarbossKard()
        {
            bossName = "Warboss Kard";
            curHealth = 100.0f;
            maxHealth = 100.0f;

            PopulateMechanicsList();
        }

        protected override void PopulateMechanicsList()
        {
            for (int sceneIndex = (int)DataManager.MicrogameScenes.BossScene + 1; sceneIndex <= (int)DataManager.MicrogameScenes.MICROGAME_END; ++sceneIndex)
            {
                mechanics.Add((DataManager.MicrogameScenes)sceneIndex);
            }
        }
    }
}