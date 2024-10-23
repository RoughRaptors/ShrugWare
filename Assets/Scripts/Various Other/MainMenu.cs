using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShrugWare
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        Button startButton;

        [SerializeField]
        Button tutorialButton;

        [SerializeField]
        AudioManager audioManager;

        [SerializeField]
        GameObject optionsMenu;

        private void Awake()
        {
            // this was generating errors on dev build startup
            // https://forum.unity.com/threads/errors-with-the-urp-debug-manager.987795/
            DebugManager.instance.enableRuntimeUI = false;
        }

        private void Start()
        {
            audioManager.PlayAudioClip(DataManager.AudioEffectTypes.MainMenu, .15f);
        }

        public void OnStartClicked()
        {
            audioManager.StopAudio();
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        public void OnTutorialClicked()
        {
            audioManager.StopAudio();
            SceneManager.LoadScene((int)DataManager.Scenes.TutorialScene);
        }

        public void OnOpenOptionsClicked()
        {
            optionsMenu.SetActive(true);
        }
    }
}