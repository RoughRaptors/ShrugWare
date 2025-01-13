using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class HealTheTank : Microgame
    {
        [SerializeField]
        GameObject healButton1;

        [SerializeField]
        GameObject healButton2;

        [SerializeField]
        GameObject healButton3;

        [SerializeField]
        AudioClipData buttonClickAudio;

        [SerializeField]
        GameObject healthBarFill;

        [SerializeField]
        TextMeshProUGUI healthBarHealthText;

        [SerializeField]
        GameObject loseVFX;

        [SerializeField]
        GameObject tankObj;

        private const float GCD = 0.2f;
        private const float HEAL_1_COOLDOWN = GCD;
        private const float HEAL_2_COOLDOWN = float.MaxValue;
        private const float HEAL_3_COOLDOWN = 4.0f;

        private float heal1CDProgress = HEAL_1_COOLDOWN;
        private float heal2CDProgress = HEAL_2_COOLDOWN;
        private float heal3CDProgress = HEAL_3_COOLDOWN;

        private const float MAX_HP = 100.0f;
        private float tankHealth = MAX_HP;

        private const float DAMAGE_INTERVAL_MIN = 0.185f;
        private const float DAMAGE_INTERVAL_MAX = 0.475f;
        private const float MIN_DAMAGE = 8.0f;
        private const float MAX_DAMAGE = 20.0f;

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

            microGameTime *= 1.3f + extraTime;

            float nextDamageTime = UnityEngine.Random.Range(DAMAGE_INTERVAL_MIN, DAMAGE_INTERVAL_MAX);
            Invoke("DamageUpdate", nextDamageTime);

            healButton1.GetComponent<Button>().interactable = true;
            healButton2.GetComponent<Button>().interactable = true;
            healButton3.GetComponent<Button>().interactable = true;
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            HealUpdate(healButton1, ref heal1CDProgress, HEAL_1_COOLDOWN);
            HealUpdate(healButton2, ref heal2CDProgress, HEAL_2_COOLDOWN);
            HealUpdate(healButton3, ref heal3CDProgress, HEAL_3_COOLDOWN);
        }

        protected override bool VictoryCheck()
        {
            return tankHealth > 0.0f;
        }

        private void DamageUpdate()
        {
            if (!gameOver)
            {
                float damage = UnityEngine.Random.Range(MIN_DAMAGE, MAX_DAMAGE);
                tankHealth -= damage;

                if (tankHealth <= 0)
                {
                    Vector3 pos = new Vector3(tankObj.transform.position.x, tankObj.transform.position.y + 25.0f, 100.0f);
                    GameObject loseVFXGO = Instantiate(loseVFX, pos, Quaternion.identity);
                    loseVFXGO.transform.localScale = new Vector2(3.5f, 3.5f);
                    loseVFXGO.SetActive(true);

                    tankHealth = 0.0f;
                    SetMicrogameEndText(false);
                }
                else
                {
                    float nextDamageTime = UnityEngine.Random.Range(DAMAGE_INTERVAL_MIN, DAMAGE_INTERVAL_MAX);
                    Invoke("DamageUpdate", nextDamageTime);
                }

                healthBarHealthText.text = tankHealth.ToString("F0") + "/" + MAX_HP.ToString("F0");
                healthBarFill.GetComponent<Image>().fillAmount = tankHealth / MAX_HP;
            }
        }

        private void HealUpdate(GameObject healButton, ref float healCDProgress, float cooldown)
        {
            healCDProgress += Time.deltaTime;
            if (healCDProgress > cooldown)
            {
                healCDProgress = cooldown;
            }

            healButton.GetComponent<Image>().fillAmount = healCDProgress / cooldown;
        }

        public void Heal1ButtonPressed()
        {
            if (heal1CDProgress >= HEAL_1_COOLDOWN)
            {
                tankHealth += MAX_HP * 0.1f;
                if (tankHealth > MAX_HP)
                {
                    tankHealth = MAX_HP;
                }

                heal1CDProgress = 0.0f;
                healButton1.GetComponent<Image>().fillAmount = heal1CDProgress / HEAL_1_COOLDOWN;
                healthBarHealthText.text = tankHealth.ToString("F0") + "/" + MAX_HP.ToString("F0");
                healthBarFill.GetComponent<Image>().fillAmount = tankHealth / MAX_HP;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAudioClip(buttonClickAudio);
                }
            }
        }

        public void Heal2ButtonPressed()
        {
            if (heal2CDProgress >= HEAL_2_COOLDOWN)
            {
                //tankHealth += MAX_HP * 0.25f;
                tankHealth = MAX_HP;
                if (tankHealth > MAX_HP)
                {
                    tankHealth = MAX_HP;
                }

                heal2CDProgress = 0.0f;
                healButton2.GetComponent<Image>().fillAmount = heal2CDProgress / HEAL_2_COOLDOWN;
                healthBarHealthText.text = tankHealth.ToString("F0") + "/" + MAX_HP.ToString("F0");
                healthBarFill.GetComponent<Image>().fillAmount = tankHealth / MAX_HP;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAudioClip(buttonClickAudio);
                }
            }
        }
        
        public void Heal3ButtonPressed()
        {
            if (heal3CDProgress >= HEAL_3_COOLDOWN)
            {
                tankHealth = MAX_HP;
                if (tankHealth > MAX_HP)
                {
                    tankHealth = MAX_HP;
                }

                heal3CDProgress = 0.0f;
                healButton3.GetComponent<Image>().fillAmount = heal3CDProgress / HEAL_3_COOLDOWN;
                healthBarHealthText.text = tankHealth.ToString("F0") + "/" + MAX_HP.ToString();
                healthBarFill.GetComponent<Image>().fillAmount = tankHealth / MAX_HP;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAudioClip(buttonClickAudio);
                }
            }
        }
    }
}