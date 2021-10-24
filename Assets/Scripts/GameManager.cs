using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* to add a new minigame: 
1: copy an existing minigame scene and rename it
2: create/copy a script for the new minigame and open the scene
3: in unity -> file -> build settings -> add open scene
4: in DataManager.cs -> add the new scene name to the enum Scenes and update the MINIGAME_END value
*/

namespace ShrugWare
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        InputField timeScaleInputField;

        [SerializeField]
        Text timeToNextMinigameText;

        [SerializeField]
        GameObject debugObject;

        public static GameManager Instance;

        private float curTimeScale = 1.0f;
        public float GetCurTimeScale() { return curTimeScale; }

        private int numMinigamesAttempted = 0;
        private int numMinigamesWon = 0;
        private int numMinigamesLost = 0;

        // keep track of this so we can always know our scene index from anywhere - maybe useful later
        private int curSceneIndex = 0;
        public int GetCurSceneIndex() { return curSceneIndex; }

        // we could use a queue for this, but that makes randomization a bit more involved
        // for now, we can make this a stupid random and just add all of our minigame indices to it
        // to pick a minigame, pick an index at random and remove that index from the list (a little expensive with large lists, meh)
        // if our list is empty when we go to pick our next minigame, we reconstruct the list
        private List<DataManager.Scenes> minigameList = new List<DataManager.Scenes>();

        private bool gameStarted = false;

        // for now until we find something better, hold this so we know when to transition to our next minigame - TODO MAKE THIS BETTER
        private float timeInMainScene = 0.0f;

        // 0 is still alive, it's your last life
        private int livesLeft = 3;

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
                numMinigamesAttempted = GameManager.Instance.numMinigamesAttempted;
                numMinigamesWon = GameManager.Instance.numMinigamesWon;
                numMinigamesLost = GameManager.Instance.numMinigamesLost;
                minigameList = GameManager.Instance.minigameList;
                gameStarted = GameManager.Instance.gameStarted;
                livesLeft = GameManager.Instance.livesLeft;
                timeInMainScene = 0.0f;
            }

            curSceneIndex = (int)DataManager.Scenes.MainScene;
        }

        private void Start()
        {
            timeToNextMinigameText.enabled = false;
            timeScaleInputField.text = "Time Scale: " + curTimeScale.ToString("F3");
            Time.timeScale = curTimeScale;

            // initialize our minigame queue if this is our first time starting
            if (minigameList.Count == 0)
            {
                PopulateMinigameList();
            }
        }

        private void Update()
        {            
            // we will come back here whenever we load back from a minigame to the main scene
            // we need to keep playing, which means to pick and start a new minigame if we're not dead
            if (gameStarted && curSceneIndex == (int)DataManager.Scenes.MainScene)
            {
                timeInMainScene += Time.deltaTime;
                timeToNextMinigameText.text = "Next Level In: " + (DataManager.SECONDS_BETWEEN_MINIGAMES - timeInMainScene).ToString("F2") + "s";
                if (livesLeft > 0 && timeInMainScene > DataManager.SECONDS_BETWEEN_MINIGAMES)
                {
                    PickAndStartNextMinigame();
                }
                else
                {
                    // dead
                }
            }
        }

        private void PopulateMinigameList()
        {
            // add all of our minigames to a list,
            DataManager.Scenes minigameStart= DataManager.Scenes.MainScene + 1;
            for(; minigameStart <= DataManager.Scenes.MINIGAME_END; ++minigameStart)
            {
                minigameList.Add(minigameStart);
            }
        }

        private void PickAndStartNextMinigame()
        {
            // populate our minigame list if it's empty
            if(minigameList.Count == 0)
            {
                PopulateMinigameList();
            }

            if (minigameList.Count > 0)
            {
                timeInMainScene = 0.0f;

                int minigameSceneIndex = Random.Range(0, minigameList.Count);
                DataManager.Scenes scene = minigameList[minigameSceneIndex];
                minigameList.RemoveAt(minigameSceneIndex);
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

        public void MinigameSucceeded()
        {
            ++numMinigamesAttempted;
            ++numMinigamesWon;
        }

        public void MinigameFailed()
        {
            ++numMinigamesAttempted;
            ++numMinigamesLost;
        }

        // would be nice to get some kind of transition/animation for this to be smooth, something like warioware curtains opening and closing
        public void LoadScene(int sceneIndex)
        {
            curSceneIndex = sceneIndex;
            SceneManager.LoadScene(sceneIndex);
        }

        public void StartGame()
        {
            timeToNextMinigameText.enabled = true;
            debugObject.SetActive(false);
            
            gameStarted = true;
            PickAndStartNextMinigame();
        }
    }
}