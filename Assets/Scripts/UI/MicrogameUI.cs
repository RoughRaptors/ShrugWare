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



        private void OnValidate()
        {
            SetInstructionText(instructionText, introTextColor);
        }

        private void Awake()
        {
            myMicrogame = FindObjectOfType<Microgame>();
        }

        private void OnEnable()
        {
            myMicrogame.MicrogameStart += OnMicrogameStart;
            myMicrogame.MicrogameTick += ReduceSlider;
            myMicrogame.MicrogameEnd += OnMicrogameEnd;
        }

        private void OnDisable()
        {
            myMicrogame.MicrogameStart -= OnMicrogameStart;
            myMicrogame.MicrogameTick -= ReduceSlider;
            myMicrogame.MicrogameEnd -= OnMicrogameEnd;
        }

        private void ReduceSlider(float percentTimeLeft)
        {
            timerBar.value = percentTimeLeft;
        }

        private void OnMicrogameStart(string startText)
        {
            if(string.IsNullOrEmpty(startText))
                instructionTextUI.enabled = false;
            else
                SetInstructionText(startText, introTextColor);
        }

        private void OnMicrogameEnd(bool victory, string displayText)
        {
            instructionTextUI.enabled = true;
            Color textColor = victory ? winTextColor : loseTextColor;
            SetInstructionText(displayText, textColor);
        }

        private void SetInstructionText(string text, Color color)
        {
            instructionTextUI.color = color;
            instructionTextUI.text = text.ToUpper();
        }
    }
}