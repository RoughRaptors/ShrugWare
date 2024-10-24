using ShrugWare;
using System.Collections;
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

    [SerializeField]
    List<AudioClip> microgameMusic = new List<AudioClip>();

    [SerializeField]
    List<Sprite> microgameBackgrounds = new List<Sprite>();

    [SerializeField]
    Animator sceneTransitionAnim;

    [SerializeField]
    Image sceneTransitionLeftImage;

    [SerializeField]
    Image sceneTransitionRightImage;

    [SerializeField]
    GameObject backButtonObj;

    private OptionsMenu optionsMenu;

    public List<Sprite> GetMicrogameBackgrounds() { return microgameBackgrounds; }

    public List<AudioClip> GetMicrogameMusic() { return microgameMusic; }

    private const int STARTING_LIVES = 0;
    private int score = 0;
    private int livesLeft = STARTING_LIVES;
    private bool started = false;

    private float curTimeScale = 1.0f;
    public float GetCurTimeScale() { return curTimeScale; }

    private List<DataManager.Scenes> scenes = new List<DataManager.Scenes>();

    private float timeInBossScene = 0.0f;

    public enum GameState
    {
        Inactive, // if we're not in a game
        MainScreen,
        Waiting,
        InMicrogame,
        Paused,
    }

    private GameState gameState = GameState.Inactive;
    public GameState GetGameState() { return gameState; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            backButtonObj.SetActive(true);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        curTimeScale = InfiniteModeManager.Instance.curTimeScale;
        livesLeft = InfiniteModeManager.Instance.livesLeft;
        livesLeftText = InfiniteModeManager.Instance.livesLeftText;
        score = InfiniteModeManager.Instance.score;
        scoreText = InfiniteModeManager.Instance.scoreText;
        timeToNextMicrogameText = InfiniteModeManager.Instance.timeToNextMicrogameText;
        cameraObj = InfiniteModeManager.Instance.cameraObj;
        gameState = InfiniteModeManager.Instance.gameState;
        startButton = InfiniteModeManager.Instance.startButton;

        // on first go around of this, it'll be inactive
        if (gameState == GameState.Inactive)
        {
            optionsMenu = OverworldManager.Instance.GetOptionsMenu();
            UnequipAccessory();
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
            PopulateMechanicsList();
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }

        gameState = GameState.MainScreen;
        EnableCamera();

        AudioManager.Instance.LoopMusic(false);

        DontDestroyOnLoad(gameObject);

        sceneTransitionAnim.speed = 0.0f;
    }

    private void Update()
    {
        // we will come back here whenever we load back from a microgame to the main scene
        // we need to keep playing, which means to pick and start a new microgame from our raid and boss if we're not dead
        if (gameState == GameState.MainScreen || gameState == GameState.Waiting)
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

    private void PlayBetweenMicrogameTimerDing()
    {
        AudioManager.Instance.PlayAudioClip(DataManager.AudioType.MicrogameTimerDing);
    }

    public void OnStartPressed()
    {
        started = true;
        startButton.gameObject.SetActive(false);
        if (livesLeft >= 0)
        {
            gameState = GameState.Waiting;
            timeToNextMicrogameText.gameObject.SetActive(true);
            backButtonObj.SetActive(false);

            PlayBetweenMicrogameTimerDing();
            Invoke("PlayBetweenMicrogameTimerDing", 1.5f);
            Invoke("PlayBetweenMicrogameTimerDing", 2.5f);
            Invoke("PlayBetweenMicrogameTimerDing", 3.5f);
        }
        else
        {
            // we're dead, back out
            OnBackPressed();
        }
    }

    public void OnBackPressed()
    {
        TryEquipAccessory();
        SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
        Destroy(gameObject);
        Instance = null;
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
        StartCoroutine(LoadLevel((int)scene));
        //SceneManager.LoadScene((int)scene);
    }


    private IEnumerator LoadLevel(int sceneId)
    {
        sceneTransitionLeftImage.color = new Color(sceneTransitionLeftImage.color.r, sceneTransitionLeftImage.color.b, sceneTransitionLeftImage.color.g, 1.0f);
        sceneTransitionRightImage.color = new Color(sceneTransitionRightImage.color.r, sceneTransitionRightImage.color.b, sceneTransitionRightImage.color.g, 1.0f);

        sceneTransitionAnim.speed = 1.0f;
        yield return new WaitForSeconds(1);

        // only play a transition if it's a microgame
        if (sceneId >= (int)DataManager.Scenes.MICROGAME_START && sceneId <= (int)DataManager.Scenes.MICROGAME_END)
        {
            AudioManager.Instance.PlayAudioClip(DataManager.AudioType.MicrogameOutro);
            sceneTransitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(sceneId);
            sceneTransitionAnim.SetTrigger("Start");
        }
        else
        {
            SceneManager.LoadScene(sceneId);
        }
    }

    public void MicrogameCompleted(bool won)
    {
        gameState = GameState.MainScreen;
        
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
            AudioManager.Instance.PlayAudioClip(DataManager.AudioType.BetweenMicrogame);

            AddToTimeScale(0.1f);
        }
        else
        {
            HandleDied();
        }

        SceneManager.LoadScene((int)DataManager.Scenes.InfiniteModeScene);
    }

    private void HandleDied()
    {
        gameState = GameState.MainScreen;
        backButtonObj.SetActive(true);
        timeToNextMicrogameText.gameObject.SetActive(false);
        started = false;
        //score = 0;
        livesLeftText.text = "You Died";
        startButton.gameObject.SetActive(true);
        startButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Back to Overworld";
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

    public void EnableCamera()
    {
        cameraObj.enabled = true;
    }

    public void DisableCamera()
    {
        cameraObj.enabled = false;
    }

    private void UnequipAccessory()
    {
        if (OverworldManager.Instance != null)
        {
            PlayerInventory inventory = OverworldManager.Instance.PlayerInventory;
            if (inventory != null)
            {
                inventory.UnequipArmorSlot(DataManager.ArmorSlot.Accessory);
            }
        }
    }

    private void TryEquipAccessory()
    {
        // only equip if casual mode is toggled
        if (!optionsMenu.GetIsCasualMode())
        {
            return;
        }

        if (OverworldManager.Instance != null)
        {
            PlayerInventory inventory = OverworldManager.Instance.PlayerInventory;
            if (inventory != null)
            {
                inventory.EquipItemInArmorSlot((int)DataManager.ItemTemplateIds.Accessory, DataManager.ArmorSlot.Accessory);
            }
        }
    }
}
