using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    game structure:
    each raid is composed of multiple bosses
        contains a list of bosses and a curBossIndex
    each boss is composed of multiple mechanics
        contains a list of scene IDs and a curSceneId
        each boss has health
    each mechanic is a scene - a microgame
        each mechanic contains a list of effects to be ran that can modify the following either positively or negatively:
            modify player hp
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

/*
-- Clicking Mechanics --
Cast Frostbolt - Click the frostbolt button out of three options
Taunt The Adds - Click x amount of adds running around the screen.
Perform The Rotation - Click the shown abilities in the correct order

-- 2.5D Character Movement --
Dodge The Fireball - Dodge a fireball coming at you
Equalize The Two Groups -  Move to the group that will make the party member numbers even
Get Out Of Fire - Get out of the fire
Intercept The Fireball -  Run into the fireball before it hits the party
Interrupt The Ability - Hit the interrupt ability at the proper timing as the boss is casting
Knockbacked Safely - Get knocked backwards into a safe zone
Match Polarity - Move to the party members of the same electric positive or negative sign as you
Spread Out For Meteor - Spread out from everyone else for a meteor attack
Sidestep The Fireball - Sidestep a fireball coming at you
Stack Up For Meteor - Stack up on everyone for a meteor attack
Stack Powerstones - Collect powerups around the area
Steal The Chest - Collect the chest before your team members get it
*/

namespace ShrugWare
{
    public class BossGameManager : MonoBehaviour
    {
        public static BossGameManager Instance;
        
        [SerializeField]
        private BossUIManager bossUIManager;

        [SerializeField]
        Camera bossSceneCamera = null;

        private AudioManager audioManager;

        private float curTimeScale = 1.0f;
        public float GetCurTimeScale() { return curTimeScale; }
        public void SetCurTimeScale(float newTimeScale) { curTimeScale = newTimeScale; }

        // for now until we find something better, hold this so we know when to transition to our next microgame - TODO MAKE THIS BETTER
        private float timeInBossScene = 0.0f;
        public float GetTimeInBossScene() { return timeInBossScene; }

        // this is bad
        [SerializeField]
        string bossName;

        private Boss curBoss;
        public Boss CurBoss { get; set; }

        PlayerInventory playerInventory;

        public struct PlayerInfo
        {
            public PlayerInfo(int cur, int max, int lives)
            {
                curPlayerHealth = cur;
                maxPlayerHealth = max;
                livesLeft = lives;
            }

            // 0 is still alive, it's your last life
            public int curPlayerHealth;
            public int maxPlayerHealth;
            public int livesLeft;
        }

        PlayerInfo playerInfo = new PlayerInfo(DataManager.PLAYER_START_HP, DataManager.PLAYER_MAX_HP, DataManager.PLAYER_STARTING_LIVES);
        public PlayerInfo GetPlayerInfo() { return playerInfo; }

        private List<DataManager.StatEffect> previouslyRanEffects = new List<DataManager.StatEffect>();
        public List<DataManager.StatEffect> GetPreviouslyRanEffects() { return previouslyRanEffects; }

        public enum GameState
        {
            BossScreen, // the top level of the boss loop hierarchy - in between games
            Paused,
            InMicrogame,
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
                curBoss = BossGameManager.Instance.curBoss;
                curTimeScale = BossGameManager.Instance.curTimeScale;
                playerInfo.livesLeft = BossGameManager.Instance.playerInfo.livesLeft;
                timeInBossScene = 0.0f;
                EnableBossCamera(true);
            }

            if(OverworldManager.Instance != null)
            {
                playerInventory = OverworldManager.Instance.GetPlayerInventory();
                if(playerInventory != null)
                {
                    playerInventory.RecalculateStats();
                }
                else
                {
                    playerInventory = new PlayerInventory();
                }
            }

            // bad
            // set our boss
            if(curBoss == null && bossName == "Warboss Kard")
            {
                curBoss = new WarbossKard();
            }
            else if (curBoss == null && bossName == "Tuzi")
            {
                curBoss = new Tuzi();
            }

            audioManager = GetComponent<AudioManager>();
            EnableBossCamera(true);
        }

        private void Start()
        {
            Time.timeScale = curTimeScale;
            audioManager.PlayAudioClip(DataManager.AudioEffectTypes.MainMenu, .175f);

            // our raid and boss data needs to be populated by this point
            UpdateGameUI();
        }

        private void Update()
        {
            // we will come back here whenever we load back from a microgame to the main scene
            // we need to keep playing, which means to pick and start a new microgame from our raid and boss if we're not dead
            if (gameState == GameState.BossScreen)
            {
                timeInBossScene += Time.deltaTime;

                bossUIManager.UpdateBetweenMicrogameText();
                if (playerInfo.livesLeft >= 0 && timeInBossScene >= DataManager.SECONDS_BETWEEN_MICROGAMES && !(curBoss is null) && !curBoss.isDead)
                {
                    DataManager.Scenes nextScene = curBoss.PickNextMicrogame();
                    bossUIManager.SetBossUICanvasEnabled(false);
                    LoadScene((int)nextScene);
                }
            }
        }

        public void MicrogameCompleted(bool wonMicrogame)
        {
            UpdateGameUI();
            HandleFromMicrogameTransition();
            bossUIManager.SetBossUICanvasEnabled(true);
            BossGameManager.Instance.LoadScene((int)OverworldManager.Instance.CurLevel.SceneIDToLoad);
        }

        private void HandleFromMicrogameTransition()
        {
            gameState = GameState.BossScreen;
            audioManager.PlayAudioClip(DataManager.AudioEffectTypes.BetweenMicrogame, .3f);
            if (!(curBoss is null))
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
            if (curBoss.curHealth <= 0)
            {
                curBoss.isDead = true;

                // pause the game and wait for the player to hit the continue button
                PauseGame();
                bossUIManager.HandleWinGame();
            }
            else if(playerInfo.livesLeft < 0)
            {
                PauseGame();
                bossUIManager.HandleGameOver();
            }
        }

        // would be nice to get some kind of transition/animation for this to be smooth, something like warioware curtains opening and closing
        public void LoadScene(int sceneIndex)
        {
            timeInBossScene = 0.0f;
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
            gameState = GameState.BossScreen;
            bossUIManager.ToggleConsumableVisibility(false);
            audioManager.StopAudio();
        }

        public void PauseGame(bool consumablesVisible = false)
        {
            gameState = GameState.Paused;
            bossUIManager.ToggleConsumableVisibility(consumablesVisible);
        }

        public void TakePlayerRaidDamage(float amount)
        {
#if UNITY_EDITOR
            float totalAmount = amount * 5;
#else
            float totalAmount = amount;
#endif
            float mitigationModifier = 0;
            if(OverworldManager.Instance) playerInventory.GetMitigation();
            if(mitigationModifier > 0)
            {
                totalAmount = totalAmount * (mitigationModifier / 100);
            }

            playerInfo.curPlayerHealth -= (int)totalAmount;
            if(playerInfo.curPlayerHealth < 0)
            {
                --playerInfo.livesLeft;

                if (playerInfo.livesLeft > 0)
                {
                    playerInfo.curPlayerHealth = playerInfo.maxPlayerHealth;
                }
                else
                {
                    playerInfo.curPlayerHealth = 0;
                }
            }

            UpdateGameUI();
        }

        public bool HealPlayerRaid(int amount)
        {
            if (playerInfo.curPlayerHealth < playerInfo.maxPlayerHealth)
            {
                playerInfo.curPlayerHealth += amount;
                if (playerInfo.curPlayerHealth > playerInfo.maxPlayerHealth)
                {
                    playerInfo.curPlayerHealth = playerInfo.maxPlayerHealth;
                }

                return true;
            }

            return false;
        }

        public void DamageBoss(float amount)
        {
            if (!(curBoss is null))
            {
#if UNITY_EDITOR
                curBoss.TakeDamage(amount * 5);
#else
                curBoss.TakeDamage(amount);
#endif
            }
        }

        public void SetTimescale(float newTimescale)
        {
            curTimeScale = newTimescale;
            Time.timeScale = curTimeScale;
            bossUIManager.SetTimescaleInputFieldText("Time Scale: " + curTimeScale.ToString("F3"));
        }

        public void ModifyTimeScale(float amount)
        {
            curTimeScale += amount;
            Time.timeScale = curTimeScale;
            bossUIManager.SetTimescaleInputFieldText("Time Scale: " + curTimeScale.ToString("F3"));
        }

        public void AddPreviouslyRanEffect(DataManager.StatEffect effect)
        {
            previouslyRanEffects.Add(effect);
        }

        public void AddToMaxHP(int amount)
        {
            playerInfo.maxPlayerHealth += amount;
        }

        public void ResetMaxHP()
        {
            playerInfo.maxPlayerHealth = DataManager.PLAYER_MAX_HP;
        }

        public void UpdateGameUI()
        {
            bossUIManager.UpdateHealthBars();
            bossUIManager.FillGameInfoText(curBoss, playerInfo);
        }

        public void UseConsumableItem(int templateId)
        {
            playerInventory.UseConsumableItem(templateId);
        }

        public void EnableBossCamera(bool enabled)
        {
            bossSceneCamera.gameObject.SetActive(enabled);
        }

        public void ResetScene()
        {
            playerInfo = new PlayerInfo(DataManager.PLAYER_START_HP, DataManager.PLAYER_MAX_HP, DataManager.PLAYER_STARTING_LIVES);
            gameState = GameState.BossScreen;
            curBoss = null;
        }
    }
}