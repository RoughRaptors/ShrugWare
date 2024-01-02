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

        private void Awake()
        {
            // this was generating errors on dev build startup
            // https://forum.unity.com/threads/errors-with-the-urp-debug-manager.987795/
            DebugManager.instance.enableRuntimeUI = false;
            GetComponent<AudioManager>().PlayAudioClip(DataManager.AudioEffectTypes.MainMenu, .175f);
        }

        private void Start()
        {
            GetComponent<AudioManager>().PlayAudioClip(DataManager.AudioEffectTypes.MainMenu, .175f);
        }

        public void OnStartClicked()
        {

            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        public void OnTutorialClicked()
        {

            SceneManager.LoadScene((int)DataManager.Scenes.TutorialScene);
        }
    }
}