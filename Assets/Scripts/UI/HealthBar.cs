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
                curHP = GameManager.Instance.GetPlayerInfo().curPlayerHealth;
                maxHP = GameManager.Instance.GetPlayerInfo().maxPlayerHealth;
                infoText.text = "Player: ";
            }
            else
            {
                curHP = GameManager.Instance.GetCurRaid().curBoss.curHealth;
                maxHP = GameManager.Instance.GetCurRaid().curBoss.maxHealth;
                infoText.text = GameManager.Instance.GetCurRaid().curBoss.bossName + ": ";
            }

            infoText.text += curHP.ToString() + " / " + maxHP.ToString();
            GetComponent<Slider>().value = curHP / maxHP;
        }
    }
}
