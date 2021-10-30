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
        GameObject debugObject = null;

        [SerializeField]
        Canvas mainCanvas = null;

        [SerializeField]
        Button startGameButton = null;

        [SerializeField]
        Text scoreText = null;

        public static GameManager Instance;

        private float curTimeScale = 1.0f;
        public float GetCurTimeScale() { return curTimeScale; }

        private int numMicrogamesWon = 0;
        private int numMicrogamesLost = 0;

        // keep track of this so we can always know our scene index from anywhere - maybe useful later
        private int curSceneIndex = 0;
        public int GetCurSceneIndex() { return curSceneIndex; }

        // we could use a queue for this, but that makes randomization a bit more involved
        // for now, we can make this a stupid random and just add all of our microgame indices to it
        // to pick a microgame, pick an index at random and remove that index from the list (a little expensive with large lists, meh)
        // if our list is empty when we go to pick our next microgame, we reconstruct the list
        private List<DataManager.Scenes> microgameList = new List<DataManager.Scenes>();

        private bool gameStarted = false;

        // for now until we find something better, hold this so we know when to transition to our next microgame - TODO MAKE THIS BETTER
        private float timeInMainScene = 0.0f;

        // 0 is still alive, it's your last life
        private float health;
        private int livesLeft = 3;

        List<Raid> raidList = new List<Raid>();

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
                numMicrogamesWon = GameManager.Instance.numMicrogamesWon;
                numMicrogamesLost = GameManager.Instance.numMicrogamesLost;
                microgameList = GameManager.Instance.microgameList;
                gameStarted = GameManager.Instance.gameStarted;
                livesLeft = GameManager.Instance.livesLeft;
                timeInMainScene = 0.0f;
            }

            curSceneIndex = (int)DataManager.Scenes.MainScene;
        }

        private void Start()
        {
            EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
            if (sceneEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            timeToNextMicrogameText.enabled = false;
            scoreText.enabled = false;
            timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");
            Time.timeScale = curTimeScale;

            // initialize our microgame queue if this is our first time starting
            if (microgameList.Count == 0)
            {
                PopulateMicrogameList();
            }

            // initialize our raids if this is our first time starting
            if (raidList.Count == 0)
            {
                PopulateRaidList();
            }
        }

        private void Update()
        {            
            // we will come back here whenever we load back from a microgame to the main scene
            // we need to keep playing, which means to pick and start a new microgame if we're not dead
            if (gameStarted && curSceneIndex == (int)DataManager.Scenes.MainScene)
            {
                timeInMainScene += Time.deltaTime;
                timeToNextMicrogameText.text = "Next Level In: " + (DataManager.SECONDS_BETWEEN_MICROGAMES - timeInMainScene).ToString("F2") + "s";
                if (livesLeft > 0 && timeInMainScene > DataManager.SECONDS_BETWEEN_MICROGAMES)
                {
                    PickAndStartNextMicrogame();
                }
                else
                {
                    // dead
                }
            }
        }

        private void PopulateMicrogameList()
        {
            // add all of our microgames to a list,
            DataManager.Scenes microgameStart= DataManager.Scenes.MainScene + 1;
            for(; microgameStart <= DataManager.Scenes.MICROGAME_END; ++microgameStart)
            {
                microgameList.Add(microgameStart);
            }
        }

        private void PopulateRaidList()
        {
            DauntingInferno DauntingInferno = new DauntingInferno();
            raidList.Add(DauntingInferno);
        }

        private void PickAndStartNextMicrogame()
        {
            // populate our microgame list if it's empty
            if(microgameList.Count == 0)
            {
                PopulateMicrogameList();
            }

            if (microgameList.Count > 0)
            {
                timeInMainScene = 0.0f;

                int microgameSceneIndex = Random.Range(0, microgameList.Count);
                DataManager.Scenes scene = microgameList[microgameSceneIndex];
                microgameList.RemoveAt(microgameSceneIndex);

                mainCanvas.enabled = false;
                LoadScene((int)scene);
            }
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
            if(wonMicrogame)
            {
                ++numMicrogamesWon;
            }
            else
            {
                ++numMicrogamesLost;
            }

            scoreText.text = "Microgames Won: " + numMicrogamesWon.ToString() + " Microgames Lost: " + numMicrogamesLost.ToString();
            mainCanvas.enabled = true;
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

            curSceneIndex = sceneIndex;
            SceneManager.LoadScene(sceneIndex);
        }

        public void StartGame()
        {
            scoreText.enabled = true;
            timeToNextMicrogameText.enabled = true;
            debugObject.SetActive(false);
            startGameButton.gameObject.SetActive(false);
            
            gameStarted = true;
            PickAndStartNextMicrogame();
        }

        public void TakeDamage(float amount)
        {
            health -= amount;
            if(health < 0)
            {
                --livesLeft;
            }

            if(livesLeft <= 0)
            {
                // ¯\_(ツ)_/¯
            }
        }

        public void ModifyTimeScale(float amount)
        {
            curTimeScale += amount;
            Time.timeScale = curTimeScale;
            timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");
        }
    }
}