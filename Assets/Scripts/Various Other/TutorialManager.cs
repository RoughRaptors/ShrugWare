using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void OnBackClicked()
        {
            SceneManager.LoadScene((int)DataManager.Scenes.Overworld);
        }
    }
}
