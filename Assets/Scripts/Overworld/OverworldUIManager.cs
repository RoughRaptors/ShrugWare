using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace ShrugWare
{
    public class OverworldUIManager : MonoBehaviour
    {
        public static OverworldUIManager Instance;

        [SerializeField]
        GameObject mainUICanvas;

        [SerializeField]
        GameObject levelsObj;

        [SerializeField]
        Text curLevelText;

        [SerializeField]
        GameObject randomEventUIObj;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            Instance.SetCanvasEnabled(true);            
        }

        public void UpdateUI()
        {
            if(OverworldManager.Instance == null || OverworldManager.Instance.CurLevel == null)
            {
                return;
            }

            OverworldLevel curLevel = OverworldManager.Instance.GetOverworldLevel(OverworldManager.Instance.CurLevel.LevelID);
            if (curLevel != null)
            {
                if (curLevel.Locked)
                {
                    curLevelText.text = "Cur Level ID: " + curLevel.LevelID.ToString() +
                        "\n" + "Scene ID: " + curLevel.SceneIDToLoad +
                        "\n" + "**LOCKED**";
                }
                else if (curLevel.Completed && curLevel.LevelType == DataManager.OverworldLevelType.Boss)
                {
                    // we can only complete boss levels
                    curLevelText.text = "Cur Level ID: " + curLevel.LevelID.ToString() +
                        "\n" + "Scene ID: " + curLevel.SceneIDToLoad +
                        "\n" + "**COMPLETED**";
                }
                else
                {
                    curLevelText.text = "Cur Level ID: " + curLevel.LevelID.ToString() +
                        "\n" + "Scene ID: " + curLevel.SceneIDToLoad +
                        "\n" + "Type : " + curLevel.LevelType.ToString();
                }
            }
        }

        public void SetCanvasEnabled(bool enabled)
        {
            mainUICanvas.SetActive(enabled);
            levelsObj.SetActive(enabled);
            UpdateUI();
        }

        public void EnterCurLevelClicked()
        {
            if(OverworldManager.Instance.CurLevel == null)
            {
                return;
            }

            // don't go in completed boss levels (yet!)
            OverworldLevel overworldLevel = OverworldManager.Instance.GetOverworldLevel(OverworldManager.Instance.CurLevel.LevelID);
            if (overworldLevel != null && !(overworldLevel.LevelType == DataManager.OverworldLevelType.Boss && overworldLevel.Completed))
            {
                overworldLevel.EnterLevel();
            }
        }

        public void OnRandomEventTriggered()
        {
            // pop up the random event window and load the next scene on button press. 0 index is the text, 1 index is the button text
            // for some reason it won't let me change the color in the editor
            randomEventUIObj.SetActive(true);
            randomEventUIObj.GetComponentsInChildren<TextMeshProUGUI>()[0].color = new Color(0, 150, 150);
            randomEventUIObj.GetComponentsInChildren<TextMeshProUGUI>()[0].text =
                OverworldManager.Instance.CurRandomEvent.eventName + "\n\n" + OverworldManager.Instance.CurRandomEvent.eventText;
        }

        public void OnRandomEventContinuePressed()
        {
            OverworldManager.Instance.WaitingOnRandomEvent = false;
            randomEventUIObj.SetActive(false);
            OverworldUIManager.Instance.SetCanvasEnabled(false);
            SceneManager.LoadScene((int)OverworldManager.Instance.CurLevel.SceneIDToLoad);
        }
    }
}