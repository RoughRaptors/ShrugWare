using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShrugWare
{
    public abstract class Microgame : MonoBehaviour
    {
        [Header("Microgame Text")]
        [SerializeField] private string victoryText = "";
        [SerializeField] private string defaultDefeatText = "";
        [SerializeField] private float postTimerWait = 0f; //Delay between timer ending and result being given.
        protected string startText = ""; //Set after opening delay time. Currently only used by rotation microgame

        [Header("Microgame Specific")]
        [SerializeField] private List<MicrogameEffect> effects = new List<MicrogameEffect>();

        // used to restrict player input and introduce a delay to the beginning of a microgame
        protected float microGameTime = 0;

        protected float timeElapsed = 0;

        // the effects to trigger if you win or lose
        private List<DataManager.StatEffect> winEffects = new List<DataManager.StatEffect>();
        private List<DataManager.StatEffect> lossEffects = new List<DataManager.StatEffect>();

        // allow for a brief pause in between microgame timers running out
        private bool hasRunEndCondition = false;
        public event Action<string> MicrogameStartText;
        public event Action MicrogameStarted;
        public event Action<float> MicrogameTick;
        public event Action MicrogameEnded;
        public event Action<bool, string> MicrogameEndText;
        private bool endTextSet = false;

        protected virtual void Awake()
        {
            OnMyGameAwake();
        }

        protected virtual void Start()
        {
            // will be null if individually loading scenes
            if (BossGameManager.Instance)
            {
                BossGameManager.Instance.SetGameState(BossGameManager.GameState.InMicrogame);
                Time.timeScale = BossGameManager.Instance.GetCurTimeScale();
            }

            microGameTime = DataManager.MICROGAME_DURATION_SECONDS;

            SetEffects();
            StartCoroutine(PlayMicrogame());
        }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        private IEnumerator PlayMicrogame()
        {
            //Wait initial delay time
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            MicrogameStartText?.Invoke(startText);
            MicrogameStarted?.Invoke();
            OnMyGameStart();

            //Wait game duration
            float timeLeft = microGameTime;
            yield return new WaitUntil(() =>
            {
                // comment this line to have infinite time to test a microgame
                timeLeft -= Time.deltaTime;
                timeElapsed += Time.deltaTime;
                float timePercentLeft = timeLeft / microGameTime;
                OnMyGameTick(timePercentLeft);
                MicrogameTick?.Invoke(timePercentLeft);
                return timeLeft <= 0;
            });

            //Time out
            TimeOut();
            yield return new WaitForSeconds(postTimerWait);
            HandleMicrogameEnd(VictoryCheck());
        }

        protected virtual void OnMyGameAwake() 
        { 

        }

        protected virtual void OnMyGameStart()
        {
            if (BossGameManager.Instance != null)
            {
                int randMusicIndex = UnityEngine.Random.Range(0, BossGameManager.Instance.GetMicrogameMusic().Count);
                AudioClip audioClip = BossGameManager.Instance.GetAudioClipFromIndex(randMusicIndex);
                BossGameManager.Instance.GetAudioManager().PlayMusicClip(audioClip, DataManager.AudioEffectTypes.MicrogameMusic);
            }
            else if(InfiniteModeManager.Instance != null)
            {
                int randMusicIndex = UnityEngine.Random.Range(0, InfiniteModeManager.Instance.GetMicrogameMusic().Count);
                AudioClip audioClip = InfiniteModeManager.Instance.GetAudioClipFromIndex(randMusicIndex);
                InfiniteModeManager.Instance.GetAudioManager().PlayMusicClip(audioClip, DataManager.AudioEffectTypes.MicrogameMusic);
                InfiniteModeManager.Instance.DisableCamera();
            }
        }

        protected virtual void OnMyGameTick(float timePercentLeft) { }
        protected virtual void TimeOut() { }
        protected abstract bool VictoryCheck();

        private void HandleMicrogameEnd(bool wonMicrogame)
        {
            SetMicrogameEndText(wonMicrogame);

            // should only be null if running the microgame scene on its own
            if (BossGameManager.Instance && !hasRunEndCondition)
            {
                hasRunEndCondition = true;
                if (wonMicrogame)
                {
                    RunEffects(winEffects);
                }
                else
                {
                    RunEffects(lossEffects);
                }

                // defer this a bit to allow the player to see the results at the end of the time limit
                StartCoroutine(LoadScene(wonMicrogame));
            }
            else if(OverworldManager.Instance != null && OverworldManager.Instance.IsDebugMode)
            {
                SceneManager.LoadScene((int)DataManager.Scenes.OverworldScene);
            }
            else if(InfiniteModeManager.Instance != null)
            {
                StartCoroutine(LoadScene(wonMicrogame));
            }
        }

        protected void SetMicrogameEndText(bool victory)
        {
            string text = defaultDefeatText;
            if(victory)
            {
                text = victoryText;
            }

            SetMicrogameEndText(victory, text);
        }

        protected void SetMicrogameEndText(bool victory, string text)
        {
            if (endTextSet)
            {
                return;
            }

            MicrogameEndText?.Invoke(victory, text);
            MicrogameEnded?.Invoke();
            endTextSet = true;
        }

        public IEnumerator LoadScene(bool wonMicrogame)
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);

            if(InfiniteModeManager.Instance != null)
            {
                InfiniteModeManager.Instance.MicrogameCompleted(wonMicrogame);
            }
            else
            {
                BossGameManager.Instance.MicrogameCompleted(wonMicrogame);
            }            
        }

        private void SetEffects()
        {
            for(int i = 0; i < effects.Count; ++i)
            {
                MicrogameEffect effect = effects[i];
                
                // double our timescale increase for now
                if (effect.effect.effectType == DataManager.StatModifierType.Timescale)
                {
                    effect.effect.amount *= 2;
                }

                if (effect.addOnWin)
                {
                    winEffects.Add(effect.effect);
                }

                if (effect.addOnLoss)
                {
                    lossEffects.Add(effect.effect);
                }
            }
        }

        private void RunEffects(List<DataManager.StatEffect> effects)
        {
            BossGameManager.Instance.ClearPreviousEffects();
            foreach (DataManager.StatEffect effect in effects)
            {
                RunEffect(effect);
            }
        }

        private void RunEffect(DataManager.StatEffect effect)
        {
            BossGameManager.Instance.AddPreviouslyRanEffect(effect);

            if (effect.effectType == DataManager.StatModifierType.PlayerCurHealth)
            {
                BossGameManager.Instance.TakePlayerRaidDamage(effect.amount);
            }
            else if (effect.effectType == DataManager.StatModifierType.BossCurHealth)
            {
                BossGameManager.Instance.DamageBoss(effect.amount);
            }
            else if(effect.effectType == DataManager.StatModifierType.Timescale)
            {
                BossGameManager.Instance.AddToTimeScale(effect.amount);
            }
        }
    }
}