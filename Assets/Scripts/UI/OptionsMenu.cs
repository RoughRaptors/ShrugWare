using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI audioVolumeText;

        [SerializeField]
        Slider audioVolumeSlider;

        [SerializeField]
        Toggle difficultyToggle;

        [SerializeField]
        TextMeshProUGUI restrictionText;

        [SerializeField]
        GameObject addItemPopup;

        [SerializeField]
        GameObject removeItemPopup;

        bool casualMode = false;

        private AudioManager audioManager;

        // our audio manager is created by now
        private void Start()
        {
            audioManager = AudioManager.Instance;
            if (audioManager  != null)
            {
                audioVolumeSlider.value = audioManager.GetAudioVolume();
            }

            audioVolumeText.text = "Audio Volume: " + audioVolumeSlider.value.ToString();
        }

        private void OnEnable()
        {
            if (BossGameManager.Instance == null && OverworldManager.Instance == null
                || (BossGameManager.Instance != null && BossGameManager.Instance.GetGameState() != BossGameManager.GameState.Inactive)
                || (InfiniteModeManager.Instance != null && InfiniteModeManager.Instance.GetGameState() != InfiniteModeManager.GameState.Inactive))
            {
                ColorBlock cb = difficultyToggle.colors;
                cb.normalColor = Color.grey;
                cb.highlightedColor = Color.grey;
                restrictionText.enabled = true;
                difficultyToggle.isOn = casualMode;
                difficultyToggle.interactable = false;
            }
            else
            {
                ColorBlock cb = difficultyToggle.colors;
                cb.normalColor = Color.white;
                cb.highlightedColor = Color.white;
                restrictionText.enabled = false;
                difficultyToggle.isOn = casualMode;
                difficultyToggle.interactable = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.gameObject.SetActive(false);
            }
        }

        public void OnAudioVolumeSliderValueChanged()
        {
            audioVolumeText.text = "Audio Volume: " + audioVolumeSlider.value.ToString();

            if (audioManager != null)
            {
                audioManager.SetAudioVolume(audioVolumeSlider.value);
            }
        }

        public void OnExitButtonPressed()
        {
            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.UnpauseGame();
            }
            
            this.gameObject.SetActive(false);
        }

        // give an item for casual mode that adds buffs
        public void OnChangeDifficultyToggled()
        {
            casualMode = !casualMode;
            if (casualMode)
            {
                addItemPopup.SetActive(true);
                removeItemPopup.SetActive(false);
            }
            else
            {
                addItemPopup.SetActive(false);
                removeItemPopup.SetActive(true);
            }
        }

        public void OnItemPopupContinuePressed()
        {
            addItemPopup.SetActive(false);
            removeItemPopup.SetActive(false);
            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.PlayerInventory.DifficultyChanged(casualMode);
            }
        }

        public bool GetIsCasualMode()
        {
            return casualMode;
        }
    }
}