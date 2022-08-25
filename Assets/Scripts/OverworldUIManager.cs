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
            OverworldLevel curLevel = OverworldManager.Instance.GetOverworldLevel(OverworldManager.Instance.CurLevel.LevelID);
            if (curLevel != null)
            {
                curLevelText.text = "Cur Level ID: " + curLevel.LevelID.ToString() +
                    "\n" + "Scene ID: " + curLevel.SceneIDToLoad +
                    "\n" + "Type : " + curLevel.LevelType.ToString();
            }
        }

        public void SetCanvasEnabled(bool enabled)
        {
            mainUICanvas.SetActive(enabled);
        }

        public void EnterCurLevelClicked()
        {
            OverworldLevel overworldLevel = OverworldManager.Instance.GetOverworldLevel(OverworldManager.Instance.CurLevel.LevelID);
            if (overworldLevel != null)
            {
                overworldLevel.EnterLevel();
            }
        }
    }
}