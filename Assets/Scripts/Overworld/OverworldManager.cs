using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShrugWare
{

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

        Dictionary<int, OverworldLevel> overworldMap = new Dictionary<int, OverworldLevel>();

        int curLevelID = 0;
        public int CurLevelID { get; set; }

        // TODO UI
        [SerializeField]
        Text curLevelText;

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
                curLevelText = OverworldManager.Instance.curLevelText;
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
            OverworldLevel overworldLevel = null;
            overworldMap.TryGetValue(levelID, out overworldLevel);
            if(overworldLevel != null)
            {
                return overworldLevel.Unlocked;
            }

            return false;
        }

        public void EnterCurLevelClicked()
        {
            OverworldLevel overworldLevel = null;
            overworldMap.TryGetValue(curLevelID, out overworldLevel);
            if (overworldLevel != null)
            {
                overworldLevel.EnterLevel();
            }
        }

        public void CompleteLevel(int completedLevelID)
        {
            OverworldLevel overworldLevel = null;
            overworldMap.TryGetValue(completedLevelID, out overworldLevel);
            if (overworldMap != null)
            {
                foreach (int idToUnlock in overworldLevel.LevelIDsToUnlock)
                {
                    OverworldLevel overworldLevelToUnlock = null;
                    overworldMap.TryGetValue(idToUnlock, out overworldLevelToUnlock);
                    if(overworldLevelToUnlock != null)
                    {
                        overworldLevelToUnlock.Unlocked = true;
                    }
                }
            }
        }

        public void DEBUGSetCurLevelID(int levelID)
        {
            OverworldLevel overworldLevel = null;
            overworldMap.TryGetValue(levelID, out overworldLevel);
            if (overworldMap != null)
            {
                curLevelID = levelID;
                curLevelText.text = "Cur Level ID: " + curLevelID.ToString() +
                    "\n" + "Scene ID: " + overworldLevel.SceneIDToLoad +
                    "\n" + "Type : " + overworldLevel.LevelType.ToString();
            }
            else
            {
                curLevelText.text = "Invalid Level ID";
            }
        }
    }
}
