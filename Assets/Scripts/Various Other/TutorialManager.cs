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

            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.CompleteLevel(OverworldManager.Instance.CurLevel.LevelID);
            }
        }

        public void OnBackClicked()
        {
            if (OverworldManager.Instance != null)
            {
                OverworldManager.Instance.ReadyScene(true);
            }

            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }
}
