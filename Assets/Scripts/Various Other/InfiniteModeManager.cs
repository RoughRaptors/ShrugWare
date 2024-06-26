using ShrugWare;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfiniteModeManager : MonoBehaviour
{
    public static InfiniteModeManager Instance;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    TextMeshProUGUI livesLeftText;

    [SerializeField]
    Button startButton;

    [SerializeField]
    TextMeshProUGUI timeToNextMicrogameText;

    [SerializeField]
    Camera cameraObj;

    private const int STARTING_LIVES = 3;
    private int score = 0;
    private int livesLeft = STARTING_LIVES;
    private bool started = false;

    private float curTimeScale = 1.0f;
    public float GetCurTimeScale() { return curTimeScale; }

    private List<DataManager.Scenes> scenes = new List<DataManager.Scenes>();

    private float timeInBossScene = 0.0f;

    private AudioManager audioManager;

    public enum GameState
    {
        Inactive, // if we're not in a game
        BossScreen, // the top level of the boss loop hierarchy - in between games
        Paused,
        InMicrogame,
    }

    private GameState gameState = GameState.Inactive;

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

            curTimeScale = InfiniteModeManager.Instance.curTimeScale;
            livesLeft = InfiniteModeManager.Instance.livesLeft;
            livesLeftText = InfiniteModeManager.Instance.livesLeftText;
            score = InfiniteModeManager.Instance.score;
            scoreText = InfiniteModeManager.Instance.scoreText;
            timeToNextMicrogameText = InfiniteModeManager.Instance.timeToNextMicrogameText;
            cameraObj = InfiniteModeManager.Instance.cameraObj;
            gameState = InfiniteModeManager.Instance.gameState;
            startButton = InfiniteModeManager.Instance.startButton;
        }

        // on first go around of this, it'll be inactive
        if(gameState == GameState.Inactive)
        {
            timeToNextMicrogameText.gameObject.SetActive(false);
            startButton.enabled = true;
            startButton.gameObject.SetActive(true);
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
            scoreText.gameObject.SetActive(true);
            scoreText.text = "Score: " + score.ToString();
            livesLeftText.gameObject.SetActive(true);
            timeToNextMicrogameText.gameObject.SetActive(false);
            SetTimescale(1);
            livesLeftText.text = "Lives Left: " + livesLeft.ToString() + "\nTimescale: " + curTimeScale.ToString();
            gameState = GameState.BossScreen;
            PopulateMechanicsList();
        }

        gameState = GameState.BossScreen;
        cameraObj.gameObject.SetActive(true);

        if (OverworldManager.Instance != null)
        {
            audioManager = OverworldManager.Instance.GetComponent<AudioManager>();
        }
    }

    private void Update()
    {
        // we will come back here whenever we load back from a microgame to the main scene
        // we need to keep playing, which means to pick and start a new microgame from our raid and boss if we're not dead
        if (gameState == GameState.BossScreen)
        {            
            if(started && livesLeft >= 0)
            {
                timeInBossScene += Time.deltaTime;
                float timeLeft = DataManager.SECONDS_BETWEEN_MICROGAMES - timeInBossScene;
                timeToNextMicrogameText.text = "Time to next microgame: " + timeLeft.ToString("F2");

                if (timeInBossScene >= DataManager.SECONDS_BETWEEN_MICROGAMES)
                {
                    PickNextMicrogame();
                }
            }
        }
    }

    private void PopulateMechanicsList()
    {
        for (int sceneIndex = (int)DataManager.Scenes.MICROGAME_START; sceneIndex <= (int)DataManager.Scenes.MICROGAME_END; ++sceneIndex)
        {
            scenes.Add((DataManager.Scenes)sceneIndex);
        }
    }

    public void OnStartPressed()
    {
        started = true;
        startButton.gameObject.SetActive(false);
        if (livesLeft >= 0)
        {
            gameState = GameState.BossScreen;
            timeToNextMicrogameText.gameObject.SetActive(true);
        }
        else
        {
            // we're dead, back out
            timeToNextMicrogameText.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
            livesLeftText.gameObject.SetActive(false);
            cameraObj.gameObject.SetActive(false);
            livesLeft = STARTING_LIVES;
            score = 0;
            SetTimescale(1);
            started = false;
            gameState = GameState.Inactive;
            SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        }
    }

    private void PickNextMicrogame()
    {
        if (scenes.Count == 0)
        {
            PopulateMechanicsList();
        }

        scoreText.gameObject.SetActive(false);
        livesLeftText.gameObject.SetActive(false);
        timeToNextMicrogameText.gameObject.SetActive(false);

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        int microgameSceneIndex = UnityEngine.Random.Range(0, scenes.Count);
        DataManager.Scenes scene = scenes[microgameSceneIndex];
        scenes.RemoveAt(microgameSceneIndex);

        timeInBossScene = 0;
        gameState = GameState.InMicrogame;
        SceneManager.LoadScene((int)scene);
    }

    public void MicrogameCompleted(bool won)
    {
        gameState = GameState.BossScreen;
        
        scoreText.gameObject.SetActive(true);
        livesLeftText.gameObject.SetActive(true);
        timeToNextMicrogameText.gameObject.SetActive(true);

        if (won)
        {
            ++score;
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            --livesLeft;
            livesLeftText.text = "Lives Left: " + livesLeft.ToString() + "\nTimescale: " + curTimeScale.ToString();
        }

        if(livesLeft >= 0)
        {
            audioManager.PlayAudioClip(DataManager.AudioEffectTypes.BetweenMicrogame, .3f);
            AddToTimeScale(0.1f);
        }
        else
        {
            HandleDied();
        }

        SceneManager.LoadScene((int)OverworldManager.Instance.CurLevel.SceneIDToLoad);
    }

    private void HandleDied()
    {
        timeToNextMicrogameText.gameObject.SetActive(false);
        started = false;
        score = 0;
        livesLeftText.text = "You Died";
        startButton.gameObject.SetActive(true);
        startButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Back to overworld";
    }

    public void SetTimescale(float newTimescale)
    {
        curTimeScale = newTimescale;
        Time.timeScale = curTimeScale;
    }

    public void AddToTimeScale(float amount)
    {
        curTimeScale += amount;
        Time.timeScale = curTimeScale;
        livesLeftText.text = "Lives Left: " + livesLeft.ToString() + "\nTimescale: " + curTimeScale.ToString();
    }
}
