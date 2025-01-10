using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [SerializeField]
        AudioClipData openAudioClipData;

        [SerializeField]
        AudioClipData closeAudioClipData;

        [SerializeField]
        Button exitButton;

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
            bool validOverworldState = false;
            if (OverworldManager.Instance != null)
            {
                if(OverworldManager.Instance.GetOverworldGameState() != OverworldManager.OverworldGameState.TrashEncounter
                    && OverworldManager.Instance.GetOverworldGameState() != OverworldManager.OverworldGameState.InfiniteMode)
                {
                    validOverworldState = true;
                }
            }

            if (!validOverworldState || BossGameManager.Instance != null)
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

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseMusic();
                AudioManager.Instance.PlayAudioClip(openAudioClipData);
            }
        }

        private void OnDisable()
        {
            Close();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnExitButtonPressed();
            }
        }

        public void OnAudioVolumeSliderValueChanged()
        {
            audioVolumeText.text = "Audio Volume: " + audioVolumeSlider.value.ToString();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetAudioVolume(audioVolumeSlider.value);
            }
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

            audioVolumeText.gameObject.SetActive(false);
            difficultyToggle.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
        }

        public void OnItemPopupContinuePressed()
        {
            addItemPopup.SetActive(false);
            removeItemPopup.SetActive(false);
            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.PlayerInventory.DifficultyChanged(casualMode);
            }

            audioVolumeText.gameObject.SetActive(true);
            difficultyToggle.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(true);
        }

        public bool GetIsCasualMode()
        {
            return casualMode;
        }

        public void OnExitButtonPressed()
        {
            Close();
            this.gameObject.SetActive(false);

            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.RevertTimescale();
            }
        }

        private void Close()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UnPauseMusic();
                AudioManager.Instance.PlayAudioClip(closeAudioClipData);
            }
        }

        public void OnReturnToOverworldPressed()
        {
            if(BossGameManager.Instance != null)
            {
                Destroy(BossGameManager.Instance.gameObject);
            }

            Time.timeScale = 1.0f;
            gameObject.SetActive(false);
            OverworldManager.Instance.ReadyScene(true);
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }
}