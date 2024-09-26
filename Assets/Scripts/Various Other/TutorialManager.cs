using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance;

        [SerializeField]
        GameObject page1;

        [SerializeField]
        GameObject page2;

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
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }

        public void OnToPage1Clicked()
        {
            page1.SetActive(true);
            page2.SetActive(false);
        }

        public void OnToPage2Clicked()
        {
            page1.SetActive(false);
            page2.SetActive(true);
        }
    }
}
