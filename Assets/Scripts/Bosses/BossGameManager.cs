using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
    to add a new microgame: 
    1: copy an existing microgame scene and rename it
    2: create/copy a script for the new microgame and open the scene
    3: in DataManager.cs -> add the new scene name to the enum Scenes and update the MICROGAME_END value
    4: in unity -> file -> build settings -> add open scene and move it into the proper place
    5: make sure the overworld levels didn't get screwed up and have the wrong level set
*/

/*
-- Clicking Mechanics --
Attack The Boss Timing - Click the attack button in the light green
Cast Frostbolt - Click the frostbolt button out of three options
Craft The Item - Drag items into slots
Taunt The Adds - Click x amount of adds running around the screen.
Perform The Rotation - Click the shown abilities in the correct order

-- 2.5D Character Movement --
Avoid The Lasers - Avoid a spinning laser
Buy Cheap/Expensive Item - Purchase the proper item at a merchant
Colored Platforms - Get on the green platform
Dodge The Fireball - Dodge a fireball coming at you
Equalize The Two Groups -  Move to the group that will make the party member numbers even
Get Out Of Fire - Get out of the fire
Intercept The Fireball -  Run into the fireball before it hits the party
Interrupt The Ability - Hit the interrupt ability at the proper timing as the boss is casting
Knockbacked Safely - Get knocked backwards into a safe zone
Laser Line of Sight Diagonal - LOS the lasers
Laser Line of Sight Horizontal - LOS the lasers
Laser Line of Sight Vertical + Alternative - LOS the lasers
Line of Sight - LOS the boss in time
Match Polarity - Move to the party members of the same electric positive or negative sign as you
Melee The Boss - hit boss with your sword (just collide)
Navigate The Maze - Navigate a maze to get to the end
Run In - Leeroy run into the boss room
Shoot The Boss - Aim with mouse, shoot enemy(s)
Sidestep The Fireball - Sidestep a fireball coming at you
Spread Out For Meteor - Spread out from everyone else for a meteor attack
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

        // this is bad
        [SerializeField]
        string bossName;

        private Boss curBoss;
        public Boss CurBoss { get; set; }

        [SerializeField]
        List<AudioClip> microgameMusic = new List<AudioClip>();

        public List<AudioClip> GetMicrogameMusic() { return microgameMusic; }

        private AudioManager audioManager;
        public AudioManager GetAudioManager() { return audioManager; }

        private float curTimeScale = 1.0f;
        public float GetCurTimeScale() { return curTimeScale; }
        public void SetCurTimeScale(float newTimeScale) { curTimeScale = newTimeScale; }

        private float timeInBossScene = 0.0f;
        public float GetTimeInBossScene() { return timeInBossScene; }

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

        // events and armor can modifier our hp. we pull from DataManger.PLAYER_START_HP, so we need an offset to calculate our actual hp
        private int hpOffset;

        private bool hasCalculatedStats = false;

        PlayerInfo playerInfo = new PlayerInfo(DataManager.PLAYER_START_HP_BOSSENCOUNTER,
            DataManager.PLAYER_MAX_HP_BOSSENCOUNTER, DataManager.PLAYER_STARTING_LIVES_BOSSENCOUNTER);
        public PlayerInfo GetPlayerInfo() { return playerInfo; }

        private List<DataManager.StatEffect> previouslyRanEffects = new List<DataManager.StatEffect>();
        public List<DataManager.StatEffect> GetPreviouslyRanEffects() { return previouslyRanEffects; }

        public enum GameState
        {
            Inactive, // if we're not in a game
            BossScreen, // the top level of the boss loop hierarchy - in between games
            Paused,
            InMicrogame,
        }

        private GameState gameState = GameState.Inactive;
        public GameState GetGameState() { return gameState; }
        public void SetGameState(GameState gState) { gameState = gState; }

        private int curSceneIndex;
        public float GetCurSceneIndex() { return curSceneIndex; }

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

            // bad
            // set our boss
            if (curBoss == null && bossName == "Warboss Kard")
            {
                curBoss = new WarbossKard();
            }
            else if (curBoss == null && bossName == "Tuzi")
            {
                curBoss = new Tuzi();
            }

            if (OverworldManager.Instance != null)
            {
                audioManager = OverworldManager.Instance.GetComponent<AudioManager>();
            }

            EnableBossCamera(true);
        }

        private void Start()
        {
            Time.timeScale = curTimeScale;

            if (OverworldManager.Instance != null)
            {
                playerInventory = OverworldManager.Instance.PlayerInventory;
                if (playerInventory != null && !hasCalculatedStats)
                {
                    playerInventory.RecalculateStats();
                }
                else
                {
                    playerInventory = new PlayerInventory();
                }

                hasCalculatedStats = true;
            }

            // add mitigation to hp

            // apply the modifiers from our random event, if we have one
            ApplyRandomEventModifiers();

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
                    DataManager.Scenes nextScene = DataManager.Scenes.ShootTheBoss; //curBoss.PickNextMicrogame();
                    bossUIManager.SetBossUICanvasEnabled(false);
                    EnableBossCamera(false);
                    LoadScene((int)nextScene);
                }
            }
        }

        public void MicrogameCompleted(bool wonMicrogame)
        {
            UpdateGameUI();
            HandleFromMicrogameTransition();
            bossUIManager.SetBossUICanvasEnabled(true);
            LoadScene((int)OverworldManager.Instance.CurLevel.SceneIDToLoad);
        }

        private void HandleFromMicrogameTransition()
        {
            gameState = GameState.BossScreen;
            EnableBossCamera(true);

            if (!curBoss.isDead)
            {
                audioManager.PlayAudioClip(DataManager.AudioEffectTypes.BetweenMicrogame, .3f);
            }

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

                // award loot
                DataManager.Currencies lootCurrency = DataManager.Currencies.DauntingInferno;
                int lootAmount = 1500;
                OverworldManager.Instance.PlayerInventory.AddCurrency(lootCurrency, lootAmount);
                
                SetTimescale(1);
                bossUIManager.HandleBeatBoss(lootCurrency, lootAmount);
            }
            else if(playerInfo.livesLeft < 0)
            {
                SetTimescale(1);
                bossUIManager.HandleGameOver();
            }
        }

        // would be nice to get some kind of transition/animation for this to be smooth, something like warioware curtains opening and closing
        public void LoadScene(int sceneIndex)
        {
            timeInBossScene = 0.0f;
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

            string effectInfoStr = "\n";
            if (raidDamageTaken > 0)
            {
                effectInfoStr += "Your raid took " + raidDamageTaken.ToString() + " damage\n";
            }
            
            if(bossDamageTaken > 0)
            {
                effectInfoStr += curBoss.bossName + " took " + bossDamageTaken.ToString() + " damage\n";
            }

            if(timeScaleModification > 0)
            {
                effectInfoStr += "The timescale increased by " + timeScaleModification.ToString();
            }

            return effectInfoStr;
        }

        public void ClearPreviousEffects()
        {
            previouslyRanEffects.Clear();
        }

        public void ContinueGame()
        {
            SetTimescale(1);
            gameState = GameState.BossScreen;
            if (audioManager != null)
            {
                audioManager.StopAudio();
            }
        }

        public void TakePlayerRaidDamage(float amount)
        {
            float totalAmount = amount;// * 5;
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
                //amount = CurBoss.curHealth;
#endif
                curBoss.TakeDamage(100);
            }
        }

        public void SetTimescale(float newTimescale)
        {
            curTimeScale = newTimescale;
            Time.timeScale = curTimeScale;
            bossUIManager.SetTimescaleInputFieldText("Time Scale: " + curTimeScale.ToString("F3"));
            audioManager.ResetPitch();
        }

        public void AddToTimeScale(float amount)
        {
            curTimeScale += amount;
            Time.timeScale = curTimeScale;
            bossUIManager.SetTimescaleInputFieldText("Time Scale: " + curTimeScale.ToString("F3"));
        }

        public void AddPreviouslyRanEffect(DataManager.StatEffect effect)
        {
            previouslyRanEffects.Add(effect);
        }

        public void AddToPlayerRaidMaxHP(int amount)
        {
            playerInfo.maxPlayerHealth += amount;
            playerInfo.curPlayerHealth += amount;
        }

        public void ResetPlayerRaidMaxHP()
        {
            playerInfo.maxPlayerHealth = DataManager.PLAYER_MAX_HP_BOSSENCOUNTER;
        }

        public void SetToMaxHP()
        {
            playerInfo.curPlayerHealth = DataManager.PLAYER_MAX_HP_BOSSENCOUNTER + hpOffset;
        }

        public void UpdateGameUI()
        {
            bossUIManager.UpdateHealthBars();
        }

        public void UseConsumableItem(int templateId)
        {
            playerInventory.UseConsumableItem(templateId);
        }

        public void EnableBossCamera(bool enabled)
        {
            bossSceneCamera.gameObject.SetActive(enabled);
        }

        private void ApplyRandomEventModifiers()
        {
            if (OverworldManager.Instance != null && OverworldManager.Instance.CurRandomEvent != null)
            {
                foreach (DataManager.StatEffect effect in OverworldManager.Instance.CurRandomEvent.eventEffects)
                {
                    if (effect.effectType == DataManager.StatModifierType.PlayerMaxHealth)
                    {
                        if (effect.asPercentage)
                        {
                            // until we find a better solution, just hack it a *25 since microgames have more hp
                            hpOffset = (int)(DataManager.PLAYER_START_HP_BOSSENCOUNTER * (effect.amount / 100));
                            AddToPlayerRaidMaxHP(hpOffset);
                        }
                        else
                        {
                            // nothing yet
                        }
                    }
                    else if (effect.effectType == DataManager.StatModifierType.Timescale)
                    {
                        AddToTimeScale(effect.amount);
                    }
                }

                // kill it so we don't run it again
                OverworldManager.Instance.CurRandomEvent = null;
            }
        }

        public AudioClip GetAudioClipFromIndex(int index)
        {
            if(index < microgameMusic.Count)
            {
                return microgameMusic[index];
            }

            return null;
        }
    }
}