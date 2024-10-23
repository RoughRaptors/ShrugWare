using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

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
        TextMeshProUGUI curLevelText;

        [SerializeField]
        GameObject randomEventUIObj;

        [SerializeField]
        TMP_Dropdown debugDropdown;

        [SerializeField]
        Button debugButton;

        [SerializeField]
        Button stopDebugButton;

        [SerializeField]
        GameObject enterLevelButtonObj;

        [SerializeField]
        GameObject optionsButton;

        [SerializeField]
        GameObject gearButton;

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

#if UNITY_EDITOR
            curLevelText.gameObject.SetActive(true);
            debugButton.gameObject.SetActive(true);
#endif
        }

        public void UpdateUI()
        {
            // only show debug text in editor
#if !UNITY_EDITOR
            curLevelText.text = "";
            return;
#endif
            if (OverworldManager.Instance == null || OverworldManager.Instance.CurLevel == null)
            {
                return;
            }

            OverworldLevel curLevel = OverworldManager.Instance.GetOverworldLevelByID(OverworldManager.Instance.CurLevel.LevelID);
            if (curLevel != null)
            {                
#if UNITY_EDITOR
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
#endif

                if (curLevel.LevelType != DataManager.OverworldLevelType.Start && !OverworldManager.Instance.IsMoving)
                {
                    enterLevelButtonObj.SetActive(true);
                }
                else
                {
                    enterLevelButtonObj.SetActive(false);
                }
            }
        }

        public void SetCanvasEnabled(bool enabled)
        {
            mainUICanvas.SetActive(enabled);
            levelsObj.SetActive(enabled);
            UpdateUI();
            if (enabled && OverworldManager.Instance != null)
            {
                OverworldManager.Instance.EnableCamera();
            }

            EnableOptionsButton(true);
            EnableGearScreenButton(true);
        }

        public void EnterCurLevelClicked()
        {
            if(OverworldManager.Instance.CurLevel == null)
            {
                return;
            }

            // don't go in completed boss levels (yet!)
            OverworldLevel overworldLevel = OverworldManager.Instance.GetOverworldLevelByID(OverworldManager.Instance.CurLevel.LevelID);
            //if (overworldLevel != null && !(overworldLevel.LevelType == DataManager.OverworldLevelType.Boss && overworldLevel.Completed))
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
            bool isTrashOrBoss = OverworldManager.Instance.CurLevel.LevelType == DataManager.OverworldLevelType.Trash
                || OverworldManager.Instance.CurLevel.LevelType == DataManager.OverworldLevelType.Boss;
            if (isTrashOrBoss)
            {
                OverworldManager.Instance.RandomEventContinuePressed();
            }

            OverworldManager.Instance.WaitingOnRandomEvent = false;
            randomEventUIObj.SetActive(false);
            SetCanvasEnabled(false);
            SceneManager.LoadScene((int)OverworldManager.Instance.CurLevel.SceneIDToLoad, LoadSceneMode.Additive);
        }

        public void OnDebugButtonPressed()
        {
            // if the debug menu is already open, make the button enter the level
            if(OverworldManager.Instance.IsDebugMode)
            {
                SetCanvasEnabled(false);
                OverworldManager.Instance.DisableCamera();

                // offset our index
                int sceneIndex = debugDropdown.value + (int)DataManager.Scenes.MICROGAME_START;
                SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
            }
            else
            {
                PopulateDebugDropdown();
            }

            stopDebugButton.gameObject.SetActive(true);
            debugDropdown.gameObject.SetActive(true);
            OverworldManager.Instance.IsDebugMode = true;
        }

        void PopulateDebugDropdown()
        {
            List<string> debugScenes = new List<string>();
            string[] scenes = new string[SceneManager.sceneCountInBuildSettings];
            for (int sceneIndex = (int)DataManager.Scenes.MICROGAME_START; sceneIndex <= (int)DataManager.Scenes.MICROGAME_END; ++sceneIndex)
            {
                scenes[sceneIndex] = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
                debugScenes.Add("Scene Id: " + sceneIndex.ToString() + ": " + scenes[sceneIndex]);
            }

            debugDropdown.AddOptions(debugScenes);
        }

        public void OnStopDebuggingPressed()
        {
            debugButton.gameObject.SetActive(false);
            stopDebugButton.gameObject.SetActive(false);
            debugDropdown.gameObject.SetActive(false);
            OverworldManager.Instance.IsDebugMode = false;
            debugDropdown.ClearOptions();
        }

        public void EnableDebugging()
        {
            debugButton.gameObject.SetActive(true);
            stopDebugButton.gameObject.SetActive(false);
            debugDropdown.gameObject.SetActive(true);
            OverworldManager.Instance.IsDebugMode = true;
            PopulateDebugDropdown();
        }

        public void EnableStartButton()
        {
            enterLevelButtonObj.SetActive(true);
        }

        public void DisableStartButton()
        {
            enterLevelButtonObj.SetActive(false);
        }

        public void OnExitButtonPressed()
        {
            this.gameObject.SetActive(false);
            OverworldManager.Instance.UnpauseGame();
        }

        public void EnableOptionsButton(bool enabled)
        {
            optionsButton.SetActive(enabled);
        }

        public void EnableGearScreenButton(bool enabled)
        {
            gearButton.SetActive(enabled);
        }
    }
}