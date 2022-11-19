using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public class OverworldLevel : MonoBehaviour
    {
        [SerializeField]
        int levelID = -1;
        public int LevelID { get { return levelID; } }

        [SerializeField]
        DataManager.OverworldLevelType levelType;
        public DataManager.OverworldLevelType LevelType { get { return levelType; } }
        
        // the scene to load that we play
        [SerializeField]
        DataManager.Scenes sceneIDToLoad;
        public DataManager.Scenes SceneIDToLoad { get { return sceneIDToLoad; } }

        // the list of levels that are unlocked upon level completion
        [SerializeField]
        List<int> levelIDsToUnlock = new List<int>();
        public List<int> LevelIDsToUnlock { get { return levelIDsToUnlock; } }

        bool completed = false;
        public bool Completed { get; set; }

        bool unlocked = false;
        public bool Unlocked { get; set; }

        private void Start()
        {
            OverworldManager.Instance.AddLevel(this);
        }

        public void EnterLevel()
        {
            OverworldManager.Instance.EnterLevel(this);
        }

        private void OnMouseDown()
        {
            OverworldManager.Instance.SetCurLevelById(levelID);
        }
    }
}