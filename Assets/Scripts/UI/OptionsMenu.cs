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

        // our audio manager is created by now
        private void Start()
        {
            audioVolumeSlider.value = AudioManager.Instance.GetAudioVolume();
            audioVolumeText.text = "Audio Volume: " + audioVolumeSlider.value.ToString();
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
            AudioManager.Instance.SetAudioVolume(audioVolumeSlider.value);
        }

        public void OnExitButtonPressed()
        {
            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.UnpauseGame();
            }
            
            this.gameObject.SetActive(false);
        }
    }
}