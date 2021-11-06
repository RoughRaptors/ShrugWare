using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField]
        InputField timeScaleInputField = null;

        [SerializeField]
        Text timeToNextMicrogameText = null;

        [SerializeField]
        Canvas mainCanvas = null;

        [SerializeField]
        Button startGameButton = null;

        [SerializeField]
        Text gameInfoText = null;

        public static GameManager Instance;

        private float curTimeScale = 1.0f;
        public float GetCurTimeScale() { return curTimeScale; }

        // keep track of this so we can always know our scene index from anywhere - maybe useful later
        private int curSceneIndex = 0;
        public int GetCurSceneIndex() { return curSceneIndex; }

        private bool gameStarted = false;

        // for now until we find something better, hold this so we know when to transition to our next microgame - TODO MAKE THIS BETTER
        private float timeInMainScene = 0.0f;

        // 0 is still alive, it's your last life
        private float curRaidHealth = 100.0f;
        private float maxRaidHealth = 100.0f;
        private int livesLeft = 3;

        private int curRaidListIndex = -1;
        public int GetCurRaidListIndex() { return curRaidListIndex; }
        private List<Raid> raidList = new List<Raid>();

        private Raid curRaid = null;

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
                timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");

                gameStarted = GameManager.Instance.gameStarted;
                livesLeft = GameManager.Instance.livesLeft;
                timeInMainScene = 0.0f;
            }

            curSceneIndex = (int)DataManager.Scenes.MainScene;
        }

        private void Start()
        {
            Screen.SetResolution(1920, 1080, false);

            EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
            if (sceneEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            gameInfoText.enabled = false;
            timeToNextMicrogameText.enabled = false;
            timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");
            Time.timeScale = curTimeScale;

            // initialize our data if this is our first time starting
            if (raidList.Count == 0)
            {
                PopulateData();
            }

            // our raid and boss data needs to be populated by this point
            FillBossInfoText();
        }

        private void Update()
        {            
            // we will come back here whenever we load back from a microgame to the main scene
            // we need to keep playing, which means to pick and start a new microgame from our raid and boss if we're not dead
            if (gameStarted && curSceneIndex == (int)DataManager.Scenes.MainScene)
            {
                timeInMainScene += Time.deltaTime;
                timeToNextMicrogameText.text = "Next Level In: " + (DataManager.SECONDS_BETWEEN_MICROGAMES - timeInMainScene).ToString("F2") + "s";
                if (livesLeft > 0 && timeInMainScene >= DataManager.SECONDS_BETWEEN_MICROGAMES && !(curRaid is null) && !(curRaid.curBoss is null))
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

        public void ConfirmTimeScaleButtonClicked()
        {
            float newTimeScale = 1.0f;
            float.TryParse(timeScaleInputField.text, out newTimeScale);
            if (curTimeScale != newTimeScale && newTimeScale != 0)
            {
                curTimeScale = newTimeScale;
            }

            timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");
        }

        public void MicrogameCompleted(bool wonMicrogame)
        {
            FillBossInfoText();
            mainCanvas.enabled = true;

            CheckAndHandleEndCondition();
        }

        private void FillBossInfoText()
        {
            if (!(curRaid is null) && !(curRaid.curBoss is null))
            {
                gameInfoText.text = curRaid.raidName + "\n" + curRaid.curBoss.bossName + "\n"
                    + "Health: " + curRaid.curBoss.curHealth.ToString() + " / " + curRaid.curBoss.maxHealth + "\n \n"
                    + "Raid Health: " + curRaidHealth.ToString() + " / " + maxRaidHealth.ToString() + "\n"
                    + "Rezzes Left: " + livesLeft.ToString();
            }
        }

        private void CheckAndHandleEndCondition()
        {
            // todo - scale this up
            if (curRaid.curBoss.curHealth <= 0)
            {
                timeToNextMicrogameText.enabled = false;
                gameInfoText.text += "\n \n CONGLADURATION. YOU ARE WIN";

                // temp hack to stop the game when you win/lose
                gameStarted = false;
            }
            else if(livesLeft == 0)
            {
                timeToNextMicrogameText.enabled = false;
                gameInfoText.text += "\n \n 50 DKP MINUS!";

                // temp hack to stop the game when you win/lose
                gameStarted = false;
            }
        }

        // would be nice to get some kind of transition/animation for this to be smooth, something like warioware curtains opening and closing
        public void LoadScene(int sceneIndex)
        {            
            if (sceneIndex == (int)DataManager.Scenes.MainScene)
            {
                mainCanvas.enabled = true;
            }
            else
            {
                mainCanvas.enabled = false;
            }
            
            timeInMainScene = 0.0f;

            curSceneIndex = sceneIndex;
            SceneManager.LoadScene(sceneIndex);
        }

        public void StartGame()
        {
            timeToNextMicrogameText.enabled = true;
            startGameButton.gameObject.SetActive(false);
            gameInfoText.enabled = true;

            gameStarted = true;
        }

        public void TakeDamage(float amount)
        {
            curRaidHealth -= amount;
            if(curRaidHealth < 0)
            {
                --livesLeft;

                if (livesLeft > 0)
                {
                    curRaidHealth = maxRaidHealth;
                }
            }
        }

        public void DamageBoss(float amount)
        {
            if (!(curRaid is null) && !(curRaid.curBoss is null))
            {
                curRaid.curBoss.TakeDamage(amount);
            }
        }

        public void ModifyTimeScale(float amount)
        {
            curTimeScale += amount;
            Time.timeScale = curTimeScale;
            timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");
        }

        public Raid GetRaidAtIndex(int index)
        {
            if(index < raidList.Count)
            {
                return raidList[index];
            }

            return null;
        }
    }
}