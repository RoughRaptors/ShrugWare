using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace ShrugWare{

/* 
    this connects the game world together by OverworldLevels
        each OverworldLevel will represent a single playable game level space
            type that can be seen in DataManager.OverworldLevelTypes
            list of OverworldLevels that it unlocks upon completion
        one game scenario per OverworldLevel (merchant, trash, boss...)
    the manager controls things, NOT the levels
*/

    public class OverworldManager : MonoBehaviour
    {
        public static OverworldManager Instance = null;

        [SerializeField]
        OverworldUIManager overworldUIManager;

        [SerializeField]
        Camera sceneCamera = null;

        Dictionary<int, OverworldLevel> overworldMap = new Dictionary<int, OverworldLevel>();

        private OverworldLevel curLevel = null;
        public OverworldLevel CurLevel
        {
            get { return curLevel; }
            set { curLevel = value; }
        }

        private PlayerInventory inventory = new PlayerInventory();
        public PlayerInventory GetPlayerInventory() { return inventory; }

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);

                // set all of our shit back - figure out a better solution later if there is one - TODO MAKE THIS BETTER
                overworldMap = OverworldManager.Instance.overworldMap;
                curLevel = OverworldManager.Instance.curLevel;

                OverworldUIManager.Instance.SetCanvasEnabled(true);
            }
        }

        private void Start()
        {
            if (inventory == null)
            {
                inventory = new PlayerInventory();
            }

            overworldUIManager.UpdateUI();
        }

        public void AddLevel(OverworldLevel newLevel)
        {
            if(!overworldMap.ContainsKey(newLevel.LevelID))
            {
                overworldMap.Add(newLevel.LevelID, newLevel);
            }

            // we need an initial starting level
            if (OverworldManager.Instance.CurLevel == null && newLevel.LevelType == DataManager.OverworldLevelType.Start)
            {
                OverworldManager.Instance.SetCurLevelById(newLevel.LevelID);
            }
        }

        public bool IsLevelUnlocked(int levelID)
        {
            OverworldLevel overworldLevel = GetOverworldLevel(levelID);
            if(overworldLevel != null)
            {
                return overworldLevel.Unlocked;
            }

            return false;
        }

        public void CompleteLevel(int completedLevelID)
        {
            OverworldLevel overworldLevel = GetOverworldLevel(completedLevelID);
            if (overworldLevel != null)
            {
                overworldLevel.Completed = true;
                foreach (int idToUnlock in overworldLevel.LevelIDsToUnlock)
                {
                    OverworldLevel overworldLevelToUnlock = GetOverworldLevel(idToUnlock);
                    if(overworldLevelToUnlock != null)
                    {
                        overworldLevelToUnlock.Unlocked = true;
                    }
                }
            }
        }

        public void SetCurLevelById(int levelID)
        {
            // only change if it's valid
            OverworldLevel overworldLevel = GetOverworldLevel(levelID);
            if (overworldLevel != null)
            {
                curLevel = overworldLevel;
                overworldUIManager.UpdateUI();
            }
        }

        public OverworldLevel GetOverworldLevel(int levelID)
        {
            OverworldLevel overworldLevel = null;
            overworldMap.TryGetValue(levelID, out overworldLevel);
            if (overworldLevel != null)
            {
                return overworldLevel;
            }

            return null;
        }

        public void EnterLevel(OverworldLevel level)
        {
            if (level.LevelType == DataManager.OverworldLevelType.Start)
            {
                // we don't enter these
                return;
            }

            OverworldUIManager.Instance.SetCanvasEnabled(false);
            SceneManager.LoadScene((int)level.SceneIDToLoad);
        }
    }
}
