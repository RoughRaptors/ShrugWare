using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    public class MainMenu : MonoBehaviour
    {
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