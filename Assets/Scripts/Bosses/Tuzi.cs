using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class Tuzi : Boss
    {
        // Start is called before the first frame update
        public Tuzi()
        {
            bossName = "Tuzi";
            curHealth = 100.0f;
            maxHealth = 100.0f;

            PopulateMechanicsList();
            BossGameManager.Instance.SetBoss(this);
        }

        protected override void PopulateMechanicsList()
        {
            for (int sceneIndex = (int)DataManager.Scenes.MICROGAME_START; sceneIndex <= (int)DataManager.Scenes.MICROGAME_END; ++sceneIndex)
            {
                mechanics.Add((DataManager.Scenes)sceneIndex);
            }

            BossGameManager.Instance.CurBoss = this;
        }
    }
}