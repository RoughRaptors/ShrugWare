using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{

    public class OverworldUIManager : MonoBehaviour
    {
        public static OverworldUIManager Instance;

        [SerializeField]
        GameObject mainUICanvas;

        [SerializeField]
        Text curLevelText;

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
    }
}