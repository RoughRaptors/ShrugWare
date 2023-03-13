using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace ShrugWare
{
    public class MainMenu : MonoBehaviour
    {
        private void Awake()
        {
            // this was generating errors on dev build startup
            // https://forum.unity.com/threads/errors-with-the-urp-debug-manager.987795/

            DebugManager.instance.enableRuntimeUI = false;
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