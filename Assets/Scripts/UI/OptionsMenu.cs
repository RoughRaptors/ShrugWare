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

        bool casualMode = false;

        // our audio manager is created by now
        private void Start()
        {
            if (AudioManager.Instance != null)
            {
                audioVolumeSlider.value = AudioManager.Instance.GetAudioVolume();
            }

            audioVolumeText.text = "Audio Volume: " + audioVolumeSlider.value.ToString();
        }

        private void OnEnable()
        {
            if(BossGameManager.Instance == null && OverworldManager.Instance == null
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

            if (AudioManager.Instance = null)
            {
                AudioManager.Instance.SetAudioVolume(audioVolumeSlider.value);
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

        // do we want to make this only changeable outside of a boss fight? maybe
        // give an item for casual mode that adds buffs
        public void ChangeDifficulty()
        {
            if (OverworldManager.Instance != null)
            {
                casualMode = !casualMode;
                OverworldManager.Instance.PlayerInventory.DifficultyChanged(casualMode);
            }
        }

        public bool GetDifficulty()
        {
            return casualMode;
        }
    }
}