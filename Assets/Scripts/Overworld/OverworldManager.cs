using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

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

        [SerializeField]
        GameObject cameraObj;

        private bool isDebugMode = false;
        public bool IsDebugMode
        {
            get { return isDebugMode; }
            set { isDebugMode = value; }
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
                playerObj = OverworldManager.Instance.playerObj;
                EnableCamera();
                playerObj.SetActive(true);

                OverworldUIManager.Instance.SetCanvasEnabled(true);
            }

            // delay it to give our audio manager time to populate
            Invoke("PlayOverworldMusic", 0.1f);
        }

        private void Start()
        {
            // set fps so players don't have varying speeds
            Application.targetFrameRate = 60;

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

            // we want to show the level as locked if we've not completed it and it's locked
            if (newLevel.Locked && !newLevel.Completed)
            {
                newLevel.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
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
                        SetDefaultLevelColor(overworldLevelToUnlock);

                        // update the connection of our current level
                        foreach(GameObject levelConnection in overworldLevel.OutgoingLevelConnections)
                        {
                            levelConnection.GetComponent<MeshRenderer>().material.color = Color.green;
                        }
                    }
                }
            }
        }

        private void SetDefaultLevelColor(OverworldLevel overworldLevelToUnlock)
        {
            if (!overworldLevelToUnlock.Completed)
            {
                overworldLevelToUnlock.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }
        }
        
        public void SetCurLevelById(int levelID)
        {
            // only change if we've unlocked it
            OverworldLevel newOverworldLevel = GetOverworldLevel(levelID);
            if (newOverworldLevel != null)
            {
                bool force = false;
#if UNITY_EDITOR
                force = true;
#endif
                bool adjacent = false;
                if (curLevel != null && curLevel.AdjacentMapLevels.Contains(levelID))
                {
                    adjacent = true;
                }

                // special case for curLevel being null. this will happen in the beginning of the game before we set curLevel and only should happen once
                if ((!newOverworldLevel.Locked && adjacent) || force || curLevel == null)
                {
                    curLevel = newOverworldLevel;
                    StartCoroutine(MovePlayerToLevel(adjacent));
                    overworldUIManager.UpdateUI();
                }
                else
                {
                    Debug.Log("Level not unlocked");
                }
            }
        }

        private IEnumerator MovePlayerToLevel(bool adjacent)
        {
            // if we're not adjacent, we need to travel to the level along a path


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

            if (level.LevelType == DataManager.OverworldLevelType.Start || (level.Locked && !force))
            {
                // we don't enter these
                return;
            }

            // 15% chance we trigger an event
            bool isTrashOrBoss = level.LevelType == DataManager.OverworldLevelType.Trash || level.LevelType == DataManager.OverworldLevelType.Boss;
            int rand = UnityEngine.Random.Range(0, 100);
            if (rand < 15 && isTrashOrBoss)
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

            DisableCamera();
            playerObj.SetActive(false);
            GetComponent<AudioManager>().StopAudio();
            OverworldUIManager.Instance.SetCanvasEnabled(false);
            SceneManager.LoadScene((int)level.SceneIDToLoad);
        }

        private void PlayOverworldMusic()
        {
            GetComponent<AudioManager>().PlayAudioClip(DataManager.AudioEffectTypes.Overworld, .25f);
        }

        public void RandomEventContinuePressed()
        {            
            // hide the player
            // todo - fix this, it doesn't work. for whatever reason it only works when attached to a debugger
            playerObj.SetActive(false);

            GetComponent<AudioManager>().StopAudio();
        }

        public void PlayMusicClip(AudioClip audioClip, DataManager.AudioEffectTypes audioType, float volumeScale = 1)
        {
            GetComponent<AudioManager>().PlayMusicClip(audioClip, audioType);
        }

        public void StopMusic()
        {
            GetComponent<AudioManager>().StopAudio();
        }

        public void EnableCamera()
        {
            cameraObj.GetComponent<Camera>().enabled = true;
        }

        public void DisableCamera()
        {
            cameraObj.GetComponent<Camera>().enabled = false;
        }
    }
}
