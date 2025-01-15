using UnityEngine;

namespace ShrugWare
{
    public class DodgeCenterArrows : Microgame
    {
        [SerializeField]
        GameObject bloodVFX;

        [SerializeField]
        GameObject vertHorizArrowsObj;

        [SerializeField]
        GameObject diagonalArrowsObj;

        [SerializeField]
        GameObject vertHorizBreathObj;

        [SerializeField]
        GameObject diagonalBreathObj;

        [SerializeField]
        MicrogameUI microgameUI;

        [SerializeField]
        AudioClipData fireSound;

        private bool safe = true;
        private bool isDiagonalArrowPattern = false;
        private bool exploiting;

        private float nextSwapTime = -1.0f;
        private float MIN_DELAY_BETWEEN_PATTERN_SWAPS = 0.45f;
        private float MAX_DELAY_BETWEEN_PATTERN_SWAPS = 1.15f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnMyGameAwake()
        {
            base.OnMyGameAwake();
        }

        protected override void OnMyGameStart()
        {
            base.OnMyGameStart();

            Invoke("BreatheFire", microGameTime);
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                vertHorizArrowsObj.SetActive(true);
                isDiagonalArrowPattern = false;
            }
            else
            {
                diagonalArrowsObj.SetActive(true);
                isDiagonalArrowPattern = true;
            }
        }

        protected override void OnMyGameTick(float timePercentLeft)
        {
            base.OnMyGameTick(timePercentLeft);

            if (timeLeft > 0.9f && Time.time > nextSwapTime && !gameOver)
            {
                SwapArrowPatterns();
            }
        }

        protected override bool VictoryCheck()
        {
            if(exploiting)
            {
                microgameUI.ShowInstructionText("!QUACK!");
            }

            return safe;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Friendly Collider"))
            {
                exploiting = true;
                microgameUI.ShowInstructionText("!QUACK!");
                return;
            }

            transform.GetChild(1).gameObject.SetActive(false);
            GameObject bloodObj = Instantiate(bloodVFX, transform.position, Quaternion.identity);
            bloodObj.SetActive(true);
            
            safe = false;
            SetMicrogameEndText(safe);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Friendly Collider"))
            {
                exploiting = false;
            }

            microgameUI.DisableInstructionsText();
        }

        private void SwapArrowPatterns()
        {
            float patternSwapDelay = UnityEngine.Random.Range(MIN_DELAY_BETWEEN_PATTERN_SWAPS, MAX_DELAY_BETWEEN_PATTERN_SWAPS);
            nextSwapTime = Time.time + patternSwapDelay;

            // from diagonal to verthoriz
            if (isDiagonalArrowPattern)
            {
                vertHorizArrowsObj.SetActive(true);
                isDiagonalArrowPattern = false;
                diagonalArrowsObj.SetActive(false);
            }
            else
            {
                // from verthoriz to diagonal
                vertHorizArrowsObj.SetActive(false);
                isDiagonalArrowPattern = true;
                diagonalArrowsObj.SetActive(true);
            }
        }

        private void BreatheFire()
        {
            GetComponent<PlayerMover>().enabled = false;
            gameOver = true;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAudioClip(fireSound);
            }

            if (isDiagonalArrowPattern)
            {
                diagonalBreathObj.SetActive(true);
            }
            else
            {
                vertHorizBreathObj.SetActive(true);
            }
        }
    }
}