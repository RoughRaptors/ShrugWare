using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

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
        GameObject playerObj;

        Dictionary<int, OverworldLevel> overworldMap = new Dictionary<int, OverworldLevel>();

        private OverworldLevel curLevel = null;
        public OverworldLevel CurLevel
        {
            get { return curLevel; }
            set { curLevel = value; }
        }

        private PlayerInventory playerInventory = new PlayerInventory();
        public PlayerInventory PlayerInventory { get { return playerInventory; } }

        [SerializeField]
        private List<RandomEvent> randomEventList = new List<RandomEvent>();

        // the current random event that is triggered
        private RandomEvent curRandomEvent;
        public RandomEvent CurRandomEvent 
        {
            get { return curRandomEvent; }
            set { curRandomEvent = value; }
        }

        // it's possible that when reading a random event, we click a level on accident underneath the ui. don't allow that
        private bool waitingOnRandomEvent = false;
        public bool WaitingOnRandomEvent
        {
            get { return waitingOnRandomEvent; }
            set { waitingOnRandomEvent = value; }
        }

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
            if (playerInventory == null)
            {
                playerInventory = new PlayerInventory();
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

            // we want to show the locked vfx if we've not completed the level and it's available to do
            if (newLevel.Locked && !newLevel.Completed && newLevel.LockedRenderMaterials.Count > 0)
            {
                // make a new material array to use that's large enough to fit locked + current materials
                Material[] matArr = new Material[newLevel.LockedRenderMaterials.Count + 1];

                // set default mesh renderer and compensate in the for loop
                matArr[0] = newLevel.GetComponent<MeshRenderer>().materials[0];

                // these are offset because i'm an idiot
                for(int i = 1; i <= newLevel.LockedRenderMaterials.Count; ++i)
                {
                    matArr[i] = newLevel.LockedRenderMaterials[i - 1];
                }

                // set our materials array back
                newLevel.GetComponent<MeshRenderer>().materials = matArr;
            }
        }

        public bool IsLevelLocked(int levelID)
        {
#if UNITY_EDITOR
            return false;
#endif

            OverworldLevel overworldLevel = GetOverworldLevel(levelID);
            if(overworldLevel != null)
            {
                return overworldLevel.Locked;
            }

            return false;
        }

        public void CompleteLevel(int completedLevelID)
        {
            OverworldLevel overworldLevel = GetOverworldLevel(completedLevelID);
            if (overworldLevel != null)
            {
                overworldLevel.Completed = true;

                // now unlock the next level(s)
                foreach (int idToUnlock in overworldLevel.LevelIDsToUnlock)
                {
                    OverworldLevel overworldLevelToUnlock = GetOverworldLevel(idToUnlock);
                    if(overworldLevelToUnlock != null)
                    {
                        overworldLevelToUnlock.Locked = false;
                        UpdateMeshRendererMaterials(overworldLevelToUnlock);

                        // update the connection
                        foreach(GameObject levelConnection in overworldLevelToUnlock.OutgoingLevelConnections)
                        {
                            levelConnection.GetComponent<MeshRenderer>().material.color = Color.green;
                        }
                    }
                }
            }
        }

        // disable the materials that we were showing for it being locked
        private void UpdateMeshRendererMaterials(OverworldLevel overworldLevelToUnlock)
        {
            if (!overworldLevelToUnlock.Completed && overworldLevelToUnlock.LockedRenderMaterials.Count > 0)
            {
                // lazy, set default, it should exist
                // originally was gonna create a new copy of the array with the original's meshes being calculated, but i know i'm not changing that right now 
                Material[] newMatArr = new Material[1];
                newMatArr[0] = overworldLevelToUnlock.GetComponent<MeshRenderer>().materials[0];
                overworldLevelToUnlock.GetComponent<MeshRenderer>().materials = newMatArr;
            }
        }
        
        public void SetCurLevelById(int levelID)
        {
            // only change if we've unlocked it
            OverworldLevel overworldLevel = GetOverworldLevel(levelID);
            if (overworldLevel != null)
            {
                bool force = false;
#if UNITY_EDITOR
                force = true;
#endif
                if (!overworldLevel.Locked || force)
                {
                    curLevel = overworldLevel;
                    StartCoroutine(MovePlayerToLevel());
                    overworldUIManager.UpdateUI();
                }
                else
                {
                    Debug.Log("Level not unlocked");
                }
            }
        }

        private IEnumerator MovePlayerToLevel()
        {
            Vector3 playerPos = playerObj.transform.position;
            Vector3 levelPos = curLevel.gameObject.transform.position;
            while (Vector3.Distance(playerPos, levelPos) > 0.1f)
            {
                playerPos = playerObj.transform.position;
                levelPos = curLevel.gameObject.transform.position;
                playerObj.transform.position = Vector3.MoveTowards(playerPos, levelPos, 5 * Time.deltaTime);
                yield return 0;
            }

            // move it up above the level
            playerObj.transform.position = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y + 1, playerObj.transform.position.z);
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
            bool force = false;
#if UNITY_EDITOR
            force = true;
#endif

            if (level.LevelType == DataManager.OverworldLevelType.Start || level.Locked && !force)
            {
                // we don't enter these
                return;
            }

            // 10% chance we trigger an event
            int rand = UnityEngine.Random.Range(0, 100);
            if (rand < 25 && (level.LevelType == DataManager.OverworldLevelType.Trash || level.LevelType == DataManager.OverworldLevelType.Boss))
            {
                // pick a random event
                int randomEventIndex = UnityEngine.Random.Range(0, randomEventList.Count);
                curRandomEvent = randomEventList[randomEventIndex];
                
                // don't need to do anything else, we use this cached value later
                curLevel = level;
                overworldUIManager.OnRandomEventTriggered();
                waitingOnRandomEvent = true;
                return;
            }

            OverworldUIManager.Instance.SetCanvasEnabled(false);
            SceneManager.LoadScene((int)level.SceneIDToLoad);
        }
    }
}
