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
        [SerializeField] private TextMeshProUGUI instructionTextUI = null;
        [SerializeField] private Slider timerBar = null;


        private void OnValidate()
        {
            SetInstructionText(instructionText);
        }

        private void Awake()
        {
            myMicrogame = FindObjectOfType<Microgame>();
        }

        private void OnEnable()
        {
            myMicrogame.MicrogameStart += OnMicrogameStart;
            myMicrogame.MicrogameEnd += OnMicrogameEnd;
        }

        private void OnDisable()
        {
            myMicrogame.MicrogameStart -= OnMicrogameStart;
            myMicrogame.MicrogameEnd -= OnMicrogameEnd;
        }

        private void OnMicrogameStart()
        {
            instructionTextUI.enabled = false;
        }

        private void OnMicrogameEnd()
        {
            instructionTextUI.enabled = true;
        }

        private void SetInstructionText(string text)
        {
            instructionTextUI.text = text.ToUpper();
        }


    }
}