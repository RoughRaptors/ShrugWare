using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Microgame : MonoBehaviour
    {
        [Header ("Microgame Text")]
        [SerializeField] private string victoryText = "";
        [SerializeField] private string defaultDefeatText = "";
        protected string startText = ""; //Set after opening delay time. Currently only used by rotation microgame

        [Header ("Microgame Specific")]
        [SerializeField] private float postTimerWait = 0f; //Delay between timer ending and result being given. Currently only used by knockback

        // used to restrict player input and introduce a delay to the beginning of a microgame
        protected float microGameTime = 0;

        // the effects to trigger if you win or lose
        protected List<DataManager.StatEffect> winEffects = new List<DataManager.StatEffect>();
        protected List<DataManager.StatEffect> lossEffects = new List<DataManager.StatEffect>();

        // temp hack to allow for a brief pause in between microgame timers running out
        private bool hasRunEndCondition = false;
        public event Action<string> MicrogameStart;
        public event Action<float> MicrogameTick;
        public event Action<bool, string> MicrogameEnd;
        private bool endTextSet = false;


        protected virtual void Awake() { }
        protected virtual void Start()
        {
            // will be null if individually loading scenes
            if (BossGameManager.Instance)
            {
                BossGameManager.Instance.SetGameState(BossGameManager.GameState.InMicrogame);
                Time.timeScale = BossGameManager.Instance.GetCurTimeScale();
            }

            microGameTime = DataManager.MICROGAME_DURATION_SECONDS;
            if (BossGameManager.Instance != null) //If debuggind and we don't have the game manager
            {
                microGameTime *= BossGameManager.Instance.GetCurTimeScale();
            }

            StartCoroutine(PlayMicrogame());
        }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        private IEnumerator PlayMicrogame()
        {
            //Wait initial delay time
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            MicrogameStart?.Invoke(startText);
            OnMyGameStart();

            //Wait game duration
            float timeLeft = microGameTime;
            yield return new WaitUntil(() =>
            {
                timeLeft -= Time.deltaTime;
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
        
        protected virtual void OnMyGameStart() { }
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
        }

        protected void SetMicrogameEndText(bool victory)
        {
            string text = victory ? victoryText : defaultDefeatText;
            SetMicrogameEndText(victory, text);
        }

        protected void SetMicrogameEndText(bool victory, string text)
        {
            if(endTextSet) return;
            MicrogameEnd?.Invoke(victory, text);
            endTextSet = true;
        }

        public IEnumerator LoadScene(bool wonMicrogame)
        {
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            
            BossGameManager.Instance.MicrogameCompleted(wonMicrogame);
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
                BossGameManager.Instance.ModifyTimeScale(effect.amount);
            }
        }
    }
}