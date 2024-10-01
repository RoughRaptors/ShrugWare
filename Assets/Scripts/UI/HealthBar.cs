using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        Text infoText;

        [SerializeField]
        bool isPlayer;

        public void UpdateHealthBar()
        {
            float curHP = 0;
            float maxHP = 0;
            if (isPlayer)
            {
                curHP = BossGameManager.Instance.GetPlayerInfo().curPlayerHealth;
                maxHP = BossGameManager.Instance.GetPlayerInfo().maxPlayerHealth;
                infoText.text = "Player: ";
            }
            else if(BossGameManager.Instance.CurBoss != null)
            {
                curHP = BossGameManager.Instance.CurBoss.curHealth;
                maxHP = BossGameManager.Instance.CurBoss.maxHealth;
                infoText.text = BossGameManager.Instance.CurBoss.bossName + ": ";
            }

            infoText.text += curHP.ToString() + " / " + maxHP.ToString();

            if(isPlayer)
            {
                if (BossGameManager.Instance.GetPlayerInfo().livesLeft < 0)
                {
                    infoText.text += "   Raid wipe";
                }
                else
                {
                    infoText.text += "   Lives: " + BossGameManager.Instance.GetPlayerInfo().livesLeft;
                }
            }

            GetComponent<Slider>().value = curHP / maxHP;
        }
    }
}
