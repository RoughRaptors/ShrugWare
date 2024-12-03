using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class MoreDOTs : Microgame
    {
        [SerializeField]
        GameObject debuffIcon;

        [SerializeField]
        TextMeshProUGUI debuffIconText;

        [SerializeField]
        AudioClipData dotSound;

        [SerializeField]
        GameObject dotButton1;

        [SerializeField]
        GameObject dotButton2;

        [SerializeField]
        GameObject dotButton3;

        private const int NUM_DOTS_REQUIRED = 10;
        private int numDotsApplied = 0;

        private bool stoppedDOTs = true;

        private const float GCD = 0.33f;
        private const float DOT_1_COOLDOWN = GCD;
        private const float DOT_2_COOLDOWN = 1.5f;

        private bool hasUsedDOT3 = false;

        private float dot1CDProgress = DOT_1_COOLDOWN;
        private float dot2CDProgress = DOT_2_COOLDOWN;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();

            debuffIconText.text = numDotsApplied.ToString() + "/" + NUM_DOTS_REQUIRED.ToString();
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            DOTUpdate(dotButton1, ref dot1CDProgress, DOT_1_COOLDOWN);
            DOTUpdate(dotButton2, ref dot2CDProgress, DOT_2_COOLDOWN);

            if(!gameOver && VictoryCheck())
            {
                dotButton1.GetComponent<Button>().interactable = false;
                dotButton2.GetComponent<Button>().interactable = false;
                dotButton3.GetComponent<Button>().interactable = false;
                SetMicrogameEndText(true);
            }
        }

        protected override bool VictoryCheck()
        {
            return numDotsApplied >= NUM_DOTS_REQUIRED && stoppedDOTs;
        }

        private void DOTUpdate(GameObject dotButton, ref float dotCDProgress, float cooldown)
        {
            dotCDProgress += Time.deltaTime;
            if (dotCDProgress > cooldown)
            {
                dotCDProgress = cooldown;
            }

            dotButton.GetComponent<Image>().fillAmount = dotCDProgress / cooldown;
        }

        public void OnDOT1Clicked()
        {
            if (timeElapsed > 0 && !gameOver)
            {
                if (dot1CDProgress >= DOT_1_COOLDOWN)
                {
                    dot1CDProgress = 0.0f;
                    dotButton1.GetComponent<Image>().fillAmount = dot1CDProgress / DOT_1_COOLDOWN;
                    ++numDotsApplied;
                    debuffIconText.text = numDotsApplied.ToString() + "/" + NUM_DOTS_REQUIRED.ToString();

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(dotSound);
                    }
                }
            }
        }

        public void OnDOT2Clicked()
        {
            if (timeElapsed > 0 && !gameOver)
            {
                if (dot2CDProgress >= DOT_2_COOLDOWN)
                {
                    dot2CDProgress = 0.0f;
                    dotButton2.GetComponent<Image>().fillAmount = dot2CDProgress / DOT_2_COOLDOWN;
                    numDotsApplied += 3;
                    debuffIconText.text = numDotsApplied.ToString() + "/" + NUM_DOTS_REQUIRED.ToString();

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAudioClip(dotSound);
                    }
                }
            }
        }

        public void OnDOT3Clicked()
        {
            if (timeElapsed > 0 && !gameOver && !hasUsedDOT3)
            {
                dot1CDProgress = DOT_1_COOLDOWN;
                dot2CDProgress = DOT_2_COOLDOWN;
                dotButton3.GetComponent<Image>().color = Color.white;
                dotButton3.GetComponent<Button>().interactable = false;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAudioClip(dotSound);
                }
            }
        }
    }
}