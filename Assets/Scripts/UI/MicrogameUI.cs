using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    /// <summary>
    /// General Microgame UI
    /// </summary>
    public class MicrogameUI : MonoBehaviour
    {
        private Microgame myMicrogame = null;
        [SerializeField] private string instructionText = "";

        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI instructionTextUI = null;
        [SerializeField] private Slider timerBar = null;

        [Header("Text Colors")]
        [SerializeField] private Color introTextColor = Color.green;
        [SerializeField] private Color winTextColor = Color.green;
        [SerializeField] private Color loseTextColor = Color.red;

        [SerializeField] private GameObject controlSchemeImage;

        [SerializeField] Canvas backgroundCanvas;

        private void OnValidate()
        {
            SetInstructionText(instructionText, introTextColor);
        }

        private void Awake()
        {
            myMicrogame = FindObjectOfType<Microgame>();
            //controlSchemeImage.SetActive(true);

            if(OverworldManager.Instance != null)
            {
                int randSpriteIndex = UnityEngine.Random.Range(0, OverworldManager.Instance.GetMicrogameBackgrounds().Count);
                Sprite sprite = OverworldManager.Instance.GetMicrogameBackgrounds()[randSpriteIndex];
                backgroundCanvas.GetComponent<Image>().sprite = sprite;
            }

            // todo fix this - bad hard code. don't want to edit every scene right now
            if (BossGameManager.Instance != null)
            {
                bool isWASD = controlSchemeImage.name == "WASD Image";
                BossGameManager.Instance.SetTransitionControlImage(true, isWASD);
            }
        }

        private void OnEnable()
        {
            myMicrogame.MicrogameStartText += OnMicrogameStart;
            myMicrogame.MicrogameTick += ReduceSlider;
            myMicrogame.MicrogameEndText += OnMicrogameEnd;
        }

        private void OnDisable()
        {
            myMicrogame.MicrogameStartText -= OnMicrogameStart;
            myMicrogame.MicrogameTick -= ReduceSlider;
            myMicrogame.MicrogameEndText -= OnMicrogameEnd;
        }

        private void ReduceSlider(float percentTimeLeft)
        {
            timerBar.value = percentTimeLeft;
        }

        private void OnMicrogameStart(string startText)
        {
            if (BossGameManager.Instance != null)
            {
                //BossGameManager.Instance.SetTransitionControlImage(false, false);
            }

            controlSchemeImage.SetActive(false);
            if (string.IsNullOrEmpty(startText))
            {
                instructionTextUI.enabled = false;
            }
            else
            {
                SetInstructionText(startText, introTextColor);
            }
        }

        private void OnMicrogameEnd(bool victory, string displayText)
        {
            instructionTextUI.enabled = true;
            Color textColor = victory ? winTextColor : loseTextColor;
            SetInstructionText(displayText, textColor, victory, true);
        }

        private void SetInstructionText(string text, Color color, bool victory = false, bool displayPlusMinus = false)
        {
            // make it a little clear for now if they won or lost since it's not always clear, add a + or - respectively of winning or losing
            if(displayPlusMinus && victory)
            {
               text = text.Insert(0, "+");
            }
            else if(displayPlusMinus)
            {
                text = text.Insert(0, "-");
            }

            instructionTextUI.color = color;
            instructionTextUI.text = text.ToUpper();
        }

        // used in things like simon says to convey another message or warning within a microgame
        public void ShowInstructionText(string message)
        {
            instructionTextUI.text = message;
            instructionTextUI.enabled = true;
        }

        public void DisableInstructionsText()
        {
            instructionTextUI.enabled = false;
        }
    }
}