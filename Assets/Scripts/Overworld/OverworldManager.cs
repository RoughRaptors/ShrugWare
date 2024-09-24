using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
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
        public static OverworldManager Instance { get; private set; } = null;

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

        private AudioManager audioManager;
        public AudioManager AudioManager
        {
            get { return audioManager; }
        }

        [SerializeField]
        GameObject eventSystemObj;

        [SerializeField]
        GameObject audioListenerObj;

        [SerializeField]
        List<AudioClip> microgameMusic = new List<AudioClip>();
        public List<AudioClip> GetMicrogameMusic() { return microgameMusic; }

        [SerializeField]
        List<Sprite> microgameBackgrounds = new List<Sprite>();
        public List<Sprite> GetMicrogameBackgrounds() { return microgameBackgrounds; }

        // don't allow the player to move multiple spaces at once
        private bool isMoving = false;
        public bool IsMoving
        {
            get { return isMoving; }
        }

        // when we click a non-adjacent level, we need to keep track of our path
        private List<int> pathToLevel = new List<int>();

        private const float PLAYER_X_OFFSET = 1.0f;
        private const float PLAYER_Y_OFFSET = 0.6f;

        private void Awake()
        {
            audioManager = GetComponent<AudioManager>();

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                PlayOverworldMusic();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            audioListenerObj = Instance.audioListenerObj;
            eventSystemObj = Instance.eventSystemObj;
            overworldMap = Instance.overworldMap;
            curLevel = Instance.curLevel;
            playerObj = Instance.playerObj;
            playerObj.SetActive(true);
            overworldUIManager.UpdateUI();
        }

        private void Start()
        {
            // set fps so players don't have varying speeds
            Application.targetFrameRate = 60;

            ReadyScene(true);

            if (playerInventory == null)
            {
                playerInventory = new PlayerInventory();
            }
        }

        private void Update()
        {
            // tilde
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                if (isDebugMode)
                {
                    overworldUIManager.OnStopDebuggingPressed();
                }
                else
                {
                    overworldUIManager.EnableDebugging();
                }
            }
        }

        public void AddLevel(OverworldLevel newLevel)
        {
            if(!overworldMap.ContainsKey(newLevel.LevelID))
            {
                overworldMap.Add(newLevel.LevelID, newLevel);
            }

            // we need an initial starting level
            if (curLevel == null && newLevel.LevelType == DataManager.OverworldLevelType.Start)
            {
                SetCurLevelById(newLevel.LevelID);
            }

            // we want to show the level as locked if we've not completed it and it's locked
            if (newLevel.Locked && !newLevel.Completed)
            {
                newLevel.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            }
        }

        public void ReadyScene(bool enabled)
        {
            Time.timeScale = 1.0f;
            audioManager.ResetPitch(true);

            if (enabled)
            {
                EnableCamera();
                EnableEventSystem();
                OverworldUIManager.Instance.SetCanvasEnabled(true);
                //EnableAudioListener();

                // move player to cur level (this will be null on start)
                if (curLevel != null)
                {
                    playerObj.transform.position = new Vector3(curLevel.transform.position.x - PLAYER_X_OFFSET, curLevel.transform.position.y - PLAYER_Y_OFFSET, curLevel.transform.position.z);
                }

                // only do this on trash/boss/infinite levels because those have their own audio
                if (curLevel != null && (curLevel.LevelType == DataManager.OverworldLevelType.Trash || curLevel.LevelType == DataManager.OverworldLevelType.Boss
                    || curLevel.LevelType == DataManager.OverworldLevelType.Infinite))
                {
                    // give the audio manager time
                    Invoke("PlayOverworldMusic", 0.1f);
                }
            }
            else
            {
                // don't stop the music because we won't be starting any new music here (yet at least)
                if (curLevel.LevelType != DataManager.OverworldLevelType.Tutorial && curLevel.LevelType != DataManager.OverworldLevelType.Merchant)
                {
                    StopMusic();
                }

                DisableCamera();
                DisableEventSystem();
                //DisableAudioListener();
            }
        }

        public bool IsLevelLocked(int levelID)
        {
#if UNITY_EDITOR
            return false;
#endif

            OverworldLevel overworldLevel = GetOverworldLevelByID(levelID);
            if(overworldLevel != null)
            {
                return overworldLevel.Locked;
            }

            return false;
        }

        public void CompleteLevel(int completedLevelID)
        {
            OverworldLevel overworldLevel = GetOverworldLevelByID(completedLevelID);
            if (overworldLevel != null)
            {
                overworldLevel.Completed = true;

                // now unlock the next level(s)
                foreach (int idToUnlock in overworldLevel.LevelIDsToUnlock)
                {
                    OverworldLevel overworldLevelToUnlock = GetOverworldLevelByID(idToUnlock);
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
            OverworldLevel newOverworldLevel = GetOverworldLevelByID(levelID);
            if (newOverworldLevel != null)
            {
                bool force = false;
#if UNITY_EDITOR
                force = true;
#endif
                // in the beginning our curLevel will be null before it's set
                if(curLevel == null)
                {
                    curLevel = newOverworldLevel;
                }

                if ((!newOverworldLevel.Locked && !isMoving) || force)
                {
                    if (!isMoving)
                    {
                        pathToLevel.Clear();
                        List<int> visitedList = new List<int>();

                        visitedList.Add(curLevel.LevelID);
                        GeneratePathToLevel(curLevel.LevelID, newOverworldLevel.LevelID, ref visitedList);
                        pathToLevel.Reverse();

                        StartCoroutine(FollowPathToLevel());
                    }

                    overworldUIManager.UpdateUI();
                }
                else
                {
                    Debug.Log("Level not unlocked");
                }
            }
        }

        private IEnumerator MovePlayerToLevel(OverworldLevel targetOverworldLevel)
        {
            isMoving = true;

            curLevel = targetOverworldLevel;

            // be at the left and center of the level object
            Vector3 targetPos = new Vector3(curLevel.transform.position.x - PLAYER_X_OFFSET, curLevel.transform.position.y - PLAYER_Y_OFFSET, curLevel.transform.position.z);
            while (Vector3.Distance(playerObj.transform.position, targetPos) > 0.1f)
            {
                playerObj.transform.position = Vector3.MoveTowards(playerObj.transform.position, targetPos, 5 * Time.deltaTime);
                yield return 0;
            }

            isMoving = false;
        }
        
        private bool GeneratePathToLevel(int lookedAtLevelID, int targetLevelID, ref List<int> visited)
        {
            if(lookedAtLevelID == targetLevelID)
            {
                return true;
            }

            OverworldLevel level = GetOverworldLevelByID(lookedAtLevelID);
            foreach (int connectingLevelID in level.AdjacentMapLevels)
            {
                if(visited.Contains(connectingLevelID))
                {
                    continue;
                }

                visited.Add(connectingLevelID);                
                if(GeneratePathToLevel(connectingLevelID, targetLevelID, ref visited))
                {
                    pathToLevel.Add(connectingLevelID);
                    return true;
                }
            }

            pathToLevel.Remove(lookedAtLevelID);
            return false;
        }

        IEnumerator FollowPathToLevel()
        {
            isMoving = true;
            overworldUIManager.DisableStartButton();

            foreach (int levelID in pathToLevel)
            {
                OverworldLevel level = GetOverworldLevelByID(levelID);
                StartCoroutine(MovePlayerToLevel(level));

                // we're going to maybe be moving in MovePlayerToLevel. wait until we're done before going to the next level node
                while (isMoving)
                {
                    yield return null;
                }
            }

            isMoving = false;
            overworldUIManager.EnableStartButton();

            overworldUIManager.UpdateUI();
        }

        public OverworldLevel GetOverworldLevelByID(int levelID)
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

            // don't let them enter a level while moving, this is far safety even though we removed it in the ui
            if(isMoving)
            {
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

            playerObj.SetActive(false);

            // only stop if we enter a trash/boss/infinite level
            if(isTrashOrBoss || level.LevelType == DataManager.OverworldLevelType.Infinite)
            {
                audioManager.StopAudio();
            }

            OverworldUIManager.Instance.SetCanvasEnabled(false);
            ReadyScene(false);
            SceneManager.LoadScene((int)level.SceneIDToLoad, LoadSceneMode.Additive);
        }

        private void PlayOverworldMusic()
        {
           audioManager.PlayMusicClip(DataManager.AudioEffectTypes.Overworld, .25f);
        }

        public void RandomEventContinuePressed()
        {            
            // hide the player
            // todo - fix this, it doesn't work. for whatever reason it only works when attached to a debugger
            playerObj.SetActive(false);

            audioManager.StopAudio();
        }

        public void PlayMusicClip(AudioClip audioClip, DataManager.AudioEffectTypes audioType, float volumeScale = 1)
        {
            audioManager.PlayMusicClip(audioClip, audioType);
        }

        public void StopMusic()
        {
            audioManager.StopAudio();
        }

        public void EnableCamera()
        {
            cameraObj.SetActive(true);
        }

        public void DisableCamera()
        {
            cameraObj.SetActive(false);
        }

        private void EnableEventSystem()
        {
            eventSystemObj.SetActive(true);
        }

        private void DisableEventSystem()
        {
            eventSystemObj.SetActive(false);
        }

        private void EnableAudioListener()
        {
            audioListenerObj.SetActive(true);
        }

        private void DisableAudioListener()
        {
            audioListenerObj.SetActive(false);
        }

        public AudioClip GetMicrogameAudioClipFromIndex(int index)
        {
            if (index < microgameMusic.Count)
            {
                return microgameMusic[index];
            }

            return null;
        }

        public void ResetAudioPitch()
        {
            audioManager.ResetPitch();
        }
    }
}
