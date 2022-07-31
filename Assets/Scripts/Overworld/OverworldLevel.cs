using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        bool unlocked = false;
        public bool Unlocked { get; set; }

        private void Start()
        {
            OverworldManager.Instance.AddLevel(this);
        }

        void CompleteLevel()
        {
            // do we do this here or from the manager?
            // OverworldManager.Instance.CompleteLevel(levelID);
        }

        public void EnterLevel()
        {
            if (levelType == DataManager.OverworldLevelType.Start)
            {
                // we don't enter these
                return;
            }

            OverworldUIManager.Instance.SetCanvasEnabled(false);
            SceneManager.LoadScene((int)sceneIDToLoad);
        }

        private void OnMouseDown()
        {
            OverworldManager.Instance.SetCurLevelID(levelID);
        }
    }
}