using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Dictionary<int, OverworldLevel> overworldMap = new Dictionary<int, OverworldLevel>();

        private int curLevelID = 0;
        public int CurLevelID
        {
            get { return curLevelID; }
            set { curLevelID = value; }
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
                curLevelID = OverworldManager.Instance.curLevelID;
            }
        }

        private void Start()
        {
            if (inventory == null)
            {
                inventory = new PlayerInventory();
            }
        }

        public void AddLevel(OverworldLevel newLevel)
        {
            if(!overworldMap.ContainsKey(newLevel.LevelID))
            {
                overworldMap.Add(newLevel.LevelID, newLevel);
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

        public void SetCurLevelID(int levelID)
        {
            // only change if it's valid
            OverworldLevel overworldLevel = GetOverworldLevel(levelID);
            if (overworldLevel != null)
            {
                curLevelID = levelID;
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
    }
}
