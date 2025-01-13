using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class DPSTheBoss : Microgame
    {
        [SerializeField]
        GameObject dpsButton1;

        [SerializeField]
        GameObject dpsButton2;

        [SerializeField]
        GameObject dpsButton3;

        [SerializeField]
        AudioClipData buttonClickAudio;

        [SerializeField]
        GameObject healthBarFill;

        [SerializeField]
        TextMeshProUGUI healthBarHealthText;

        [SerializeField]
        GameObject tank1Obj;

        [SerializeField]
        GameObject tank2Obj;

        [SerializeField]
        GameObject bloodVFX;

        [SerializeField]
        GameObject bossObj;

        private const float GCD = 0.175f;
        private const float DPS_1_COOLDOWN = GCD;
        private const float DPS_2_COOLDOWN = 1.5f;

        private bool hasUsedDPS3 = false;

        private float dps1CDProgress = DPS_1_COOLDOWN;
        private float dps2CDProgress = DPS_2_COOLDOWN;

        private float lastRotateTime = -1.0f;
        private const float BOSS_ROTATE_TIME = 1.5f;
        private const float MAX_HP = 100.0f;
        private float bossHealth = MAX_HP;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            float extraTime = 0.0f;
            if (BossGameManager.Instance != null)
            {
                extraTime = BossGameManager.Instance.GetCurTimeScale();
            }

            microGameTime *= 1.5f + extraTime;

            dpsButton1.GetComponent<Button>().interactable = true;
            dpsButton2.GetComponent<Button>().interactable = true;
            dpsButton3.GetComponent<Button>().interactable = true;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            DPSUpdate(dpsButton1, ref dps1CDProgress, DPS_1_COOLDOWN);
            DPSUpdate(dpsButton2, ref dps2CDProgress, DPS_2_COOLDOWN);

            if (Time.time > lastRotateTime + BOSS_ROTATE_TIME)
            {
                lastRotateTime = Time.time;

                if(bossObj.transform.rotation.y == 180)
                {
                    bossObj.transform.Rotate(0, 0, 1.0f);
                }
                else
                {
                    bossObj.transform.Rotate(0, 180, 1.0f);
                }
            }
        }

        protected override bool VictoryCheck()
        {
            return bossHealth <= 0.0f;
        }

        private void DPSUpdate(GameObject dpsButton, ref float dpsCDProgress, float cooldown)
        {
            dpsCDProgress += Time.deltaTime;
            if (dpsCDProgress > cooldown)
            {
                dpsCDProgress = cooldown;
            }

            dpsButton.GetComponent<Image>().fillAmount = dpsCDProgress / cooldown;
        }

        public void DPS1ButtonPressed()
        {
            if (timeElapsed > 0 && !gameOver)
            {
                if (dps1CDProgress >= DPS_1_COOLDOWN)
                {
                    bossHealth -= MAX_HP * 0.10f;
                    if (bossHealth <= 0.0f)
                    {
                        bossHealth = 0.0f;
                        SetMicrogameEndText(true);
                        timeLeft = 0;
                    }

                    dps1CDProgress = 0.0f;
                    dpsButton1.GetComponent<Image>().fillAmount = dps1CDProgress / DPS_1_COOLDOWN;
                    healthBarHealthText.text = bossHealth.ToString("F0") + "/" + MAX_HP.ToString("F0");
                    healthBarFill.GetComponent<Image>().fillAmount = bossHealth / MAX_HP;

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(buttonClickAudio);
                    }
                }
            }
        }

        public void DPS2ButtonPressed()
        {
            if (timeElapsed > 0 && !gameOver)
            {
                if (dps2CDProgress >= DPS_2_COOLDOWN)
                {
                    if(bossObj.transform.rotation.y == 1)
                    {
                        bossHealth -= MAX_HP * 0.15f;
                    }
                    else
                    {
                        bossHealth -= MAX_HP * 0.3f;
                    }

                    if (bossHealth < 0.0f)
                    {
                        bossHealth = 0.0f;
                        SetMicrogameEndText(true);
                    }

                    dps2CDProgress = 0.0f;
                    dpsButton2.GetComponent<Image>().fillAmount = dps2CDProgress / DPS_2_COOLDOWN;
                    healthBarHealthText.text = bossHealth.ToString("F0") + "/" + MAX_HP.ToString("F0");
                    healthBarFill.GetComponent<Image>().fillAmount = bossHealth / MAX_HP;

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(buttonClickAudio);
                    }
                }
            }
        }
        
        public void DPS3ButtonPressed()
        {
            if (timeElapsed > 0 && !gameOver)
            {
                if (!hasUsedDPS3)
                {
                    hasUsedDPS3 = true;

                    dps1CDProgress = DPS_1_COOLDOWN;
                    dps2CDProgress = DPS_2_COOLDOWN;

                    dpsButton3.GetComponent<Image>().fillAmount = 0.0f;

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(buttonClickAudio);
                    }
                }
            }
        }
    }
}