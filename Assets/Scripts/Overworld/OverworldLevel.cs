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

        // the list of connection objects that this level points to
        [SerializeField]
        List<GameObject> outgoingLevelConnections;
        public List<GameObject> OutgoingLevelConnections { get { return outgoingLevelConnections; } }

        [SerializeField]
        List<int> adjacentMapLevels = new List<int>(); 
        public List<int> AdjacentMapLevels
        {
            get { return adjacentMapLevels; }
        }

        bool completed = false;
        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }

        [SerializeField]
        bool locked = true;
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

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
            if (!OverworldManager.Instance.WaitingOnRandomEvent)
            {
                OverworldManager.Instance.SetCurLevelById(levelID);
            }
        }
    }
}