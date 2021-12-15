using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/*
    game structure:
    each raid is composed of multiple bosses
        contains a list of bosses and a curBossIndex
    each boss is composed of multiple mechanics
        contains a list of scene IDs and a curSceneId
        each boss has health
    each mechanic is a scene - a microgame
        each mechanic contains a list of effects to be ran that can modify the following either positively or negatively:
            modify player raid hp
            modify boss hp
            modify timescale
    progress through that bosses minigames and perform well enough to not wipe and defeat the boss
    progress through each raid by defeating all bosses

    you are able to leave raids in between bosses to visit a merchant
        to reach the merchant, you must successfully pass a number of meta microgames
        merchant is currency based - each raid has their own currency type
        sells armor and consumables
*/

/* 
    to add a new microgame: 
    1: copy an existing microgame scene and rename it
    2: create/copy a script for the new microgame and open the scene
    3: in unity -> file -> build settings -> add open scene
    4: in DataManager.cs -> add the new scene name to the enum Scenes and update the MICROGAME_END value
*/

namespace ShrugWare
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField]
        private UIManager uiManager;

        private float curTimeScale = 1.0f;
        public float GetCurTimeScale() { return curTimeScale; }
        public void SetCurTimeScale(float newTimeScale) { curTimeScale = newTimeScale; }

        // keep track of this so we can always know our scene index from anywhere - maybe useful later
        private int curSceneIndex = 0;
        public int GetCurSceneIndex() { return curSceneIndex; }

        private bool gameRunning = false;

        // for now until we find something better, hold this so we know when to transition to our next microgame - TODO MAKE THIS BETTER
        private float timeInMainScene = 0.0f;
        public float GetTimeInMainScene() { return timeInMainScene; }

        public struct PlayerInfo
        {
            public PlayerInfo(int cur, int max, int lives)
            {
                curRaidHealth = cur;
                maxRaidHealth = max;
                livesLeft = lives;
            }

            // 0 is still alive, it's your last life
            public int curRaidHealth;
            public int maxRaidHealth;
            public int livesLeft;
        }

        PlayerInfo playerInfo = new PlayerInfo(DataManager.PLAYER_RAID_START_HP, DataManager.PLAYER_RAID_MAX_HP, DataManager.PLAYER_RAID_STARTING_LIVES);
        public PlayerInfo GetPlayerInfo() { return playerInfo; }

        private int curRaidListIndex = -1;
        public int GetCurRaidListIndex() { return curRaidListIndex; }
        private List<Raid> raidList = new List<Raid>();

        private Raid curRaid = null;
        public Raid GetCurRaid() { return curRaid; }

        private List<DataManager.StatEffect> previouslyRanEffects = new List<DataManager.StatEffect>();
        public List<DataManager.StatEffect> GetPreviouslyRanEffects() { return previouslyRanEffects; }

        private PlayerInventory inventory = new PlayerInventory();
        public PlayerInventory GetPlayerInventory() { return inventory; }

        public enum GameState
        {
            MainLoop = 0,
            Paused,
            Microgame,
            Merchant
        }

        private GameState gameState = GameState.Paused;
        public GameState GetGameState() { return gameState; }
        public void SetGameState(GameState gState) { gameState = gState; }

        private void Awake()
        {
            // this will be called every time we swap back to our main scene
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);

                // set all of our shit back - figure out a better solution later if there is one - TODO MAKE THIS BETTER
                curTimeScale = GameManager.Instance.curTimeScale;
                gameRunning = GameManager.Instance.gameRunning;
                playerInfo.livesLeft = GameManager.Instance.playerInfo.livesLeft;
                timeInMainScene = 0.0f;
            }

            curSceneIndex = (int)DataManager.Scenes.MainScene;

            EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
            if (sceneEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        private void Start()
        {
            Screen.SetResolution(1920, 1080, false);
            Time.timeScale = curTimeScale;

            // initialize our data if this is our first time starting
            if (raidList.Count == 0)
            {
                PopulateData();
            }

            if(inventory == null)
            {
                inventory = new PlayerInventory();
            }

            // our raid and boss data needs to be populated by this point
            UpdateGameInfoText();
        }

        private void Update()
        {
            // we will come back here whenever we load back from a microgame to the main scene
            // we need to keep playing, which means to pick and start a new microgame from our raid and boss if we're not dead
            if (gameRunning && curSceneIndex == (int)DataManager.Scenes.MainScene)
            {
                timeInMainScene += Time.deltaTime;

                UIManager.Instance.UpdateBetweenMicrogameText();
                if (playerInfo.livesLeft > 0 && timeInMainScene >= DataManager.SECONDS_BETWEEN_MICROGAMES && !(curRaid is null) && !(curRaid.curBoss is null))
                {
                    DataManager.Scenes nextScene = curRaid.curBoss.PickNextMicrogame(); 
                    LoadScene((int)nextScene);
                }
            }
        }

        private void PopulateData()
        {
            curRaidListIndex = 0;
            DauntingInferno dauntingInferno = new DauntingInferno();
            raidList.Add(dauntingInferno);
            curRaid = dauntingInferno;
        }

        public void MicrogameCompleted(bool wonMicrogame)
        {
            UpdateGameInfoText();
            HandleFromMicrogameTransition();
            GameManager.Instance.LoadScene((int)DataManager.Scenes.MainScene);
        }

        private void HandleFromMicrogameTransition()
        {
            gameState = GameState.MainLoop;
            UIManager.Instance.SetCanvasEnabled(true);            
            if (!(curRaid is null) && !(curRaid.curBoss is null))
            {
                CheckAndHandleEndCondition();
            }
            else
            {
                Debug.Log("Raid or Boss null in HandleFromMicrogameTransition");
            }
        }

        private void CheckAndHandleEndCondition()
        {
            if (curRaid.curBoss.curHealth <= 0)
            {
                // go to next boss
                curRaid.curBoss.isDead = true;
                curRaid.curBoss = curRaid.GetNextBoss();

                if (curRaid.IsComplete)
                {
                    // pause the game and wait for the player to hit the continue button
                    PauseGame();
                    UIManager.Instance.HandleWinGame();
                }
                else
                {
                    // boss is dead but raid is not complete, wait for the player to move on
                    PauseGame();
                    UIManager.Instance.HandlePauseGame();
                    UIManager.Instance.SetMerchantButtonActive(true);
                }
            }
            else if(playerInfo.livesLeft == 0)
            {
                PauseGame();
                UIManager.Instance.HandleGameOver();
            }
        }

        // would be nice to get some kind of transition/animation for this to be smooth, something like warioware curtains opening and closing
        public void LoadScene(int sceneIndex)
        {
            bool mainCanvasEnabled = false;
            if (sceneIndex == (int)DataManager.Scenes.MainScene)
            {
                mainCanvasEnabled = true;
            }

            UIManager.Instance.SetCanvasEnabled(mainCanvasEnabled);
            
            timeInMainScene = 0.0f;

            curSceneIndex = sceneIndex;
            SceneManager.LoadScene(sceneIndex);
        }

        public string GetPreviousEffectInfoString()
        {
            float raidDamageTaken = 0.0f;
            float bossDamageTaken = 0.0f;
            float timeScaleModification = 0.0f;

            foreach(DataManager.StatEffect effect in previouslyRanEffects)
            {
                if (effect.effectType == DataManager.StatModifierType.PlayerCurHealth)
                {
                    raidDamageTaken += effect.amount;
                }
                else if (effect.effectType == DataManager.StatModifierType.BossCurHealth)
                {
                    bossDamageTaken += effect.amount;
                }
                else if (effect.effectType == DataManager.StatModifierType.Timescale)
                {
                    timeScaleModification += effect.amount;
                }
            }

            string timescaleModStr = "\nTimescaleMod: +";
            if(timeScaleModification < 0.0f)
            {
                timescaleModStr = "\nTimescaleMod: -";
            }

            string effectInfoStr = "Raid Damage: " + raidDamageTaken.ToString() + 
                "\nBoss Damage: " + bossDamageTaken.ToString() + timescaleModStr + timeScaleModification.ToString() + "\n";

            return effectInfoStr;
        }

        public void ClearPreviousEffects()
        {
            previouslyRanEffects.Clear();
        }

        public void ContinueGame()
        {
            gameState = GameState.MainLoop;
            gameRunning = true;
        }

        public void PauseGame()
        {
            gameState = GameState.Paused;
            gameRunning = false;
        }

        public void EnterMerchant()
        {
            gameState = GameState.Merchant;
            gameRunning = false;   
        }

        public void TakePlayerRaidDamage(float amount)
        {
            float totalAmount = amount;
            float mitigationModifier = inventory.GetMitigation();
            if(mitigationModifier > 0)
            {
                totalAmount = totalAmount * (mitigationModifier / 100);
            }

            playerInfo.curRaidHealth -= (int)totalAmount;
            if(playerInfo.curRaidHealth < 0)
            {
                --playerInfo.livesLeft;

                if (playerInfo.livesLeft > 0)
                {
                    playerInfo.curRaidHealth = playerInfo.maxRaidHealth;
                }
                else
                {
                    playerInfo.curRaidHealth = 0;
                }
            }

            UIManager.Instance.FillBossInfoText(curRaid, playerInfo);
        }

        public void HealPlayerRaid(int amount)
        {
            playerInfo.curRaidHealth += amount;
            if (playerInfo.curRaidHealth > playerInfo.maxRaidHealth)
            {
                playerInfo.curRaidHealth = playerInfo.maxRaidHealth;
            }
        }

        public void DamageBoss(float amount)
        {
            if (!(curRaid is null) && !(curRaid.curBoss is null))
            {
                curRaid.curBoss.TakeDamage(amount);
            }
        }

        public void SetTimescale(float newTimescale)
        {
            curTimeScale = newTimescale;
            Time.timeScale = curTimeScale;
            UIManager.Instance.SetTimescaleInputFieldText("Time Scale: " + curTimeScale.ToString("F3"));
        }

        public void ModifyTimeScale(float amount)
        {
            curTimeScale += amount;
            Time.timeScale = curTimeScale;
            UIManager.Instance.SetTimescaleInputFieldText("Time Scale: " + curTimeScale.ToString("F3"));
        }

        public Raid GetRaidAtIndex(int index)
        {
            if(index < raidList.Count)
            {
                return raidList[index];
            }

            return null;
        }

        public void AddPreviouslyRanEffect(DataManager.StatEffect effect)
        {
            previouslyRanEffects.Add(effect);
        }

        public void AddToMaxHP(int amount)
        {
            playerInfo.maxRaidHealth += amount;
        }

        public void ResetMaxHP()
        {
            playerInfo.maxRaidHealth = DataManager.PLAYER_RAID_MAX_HP;
        }

        public void UpdateGameInfoText()
        {
            UIManager.Instance.FillBossInfoText(curRaid, playerInfo);
        }
    }
}