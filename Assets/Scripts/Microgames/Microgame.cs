using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShrugWare
{
    public abstract class Microgame : MonoBehaviour
    {
        [Header("Microgame Text")]
        [SerializeField] private string victoryText = "";
        [SerializeField] private string defaultDefeatText = "";
        [SerializeField] private float postTimerWait = 0f; //Delay between timer ending and result being given. Currently only used by knockback
        protected string startText = ""; //Set after opening delay time. Currently only used by rotation microgame

        [Header("Microgame Specific")]
        [SerializeField] private List<MicrogameEffect> effects = new List<MicrogameEffect>();

        // used to restrict player input and introduce a delay to the beginning of a microgame
        protected float microGameTime = 0;

        // the effects to trigger if you win or lose
        private List<DataManager.StatEffect> winEffects = new List<DataManager.StatEffect>();
        private List<DataManager.StatEffect> lossEffects = new List<DataManager.StatEffect>();

        // temp hack to allow for a brief pause in between microgame timers running out
        private bool hasRunEndCondition = false;
        public event Action<string> MicrogameStartText;
        public event Action MicrogameStarted;
        public event Action<float> MicrogameTick;
        public event Action MicrogameEnded;
        public event Action<bool, string> MicrogameEndText;
        private bool endTextSet = false;

        [Serializable]
        public enum ControlScheme
        {
            Keyboard = 0,
            Mouse
        }

        [SerializeField] private ControlScheme controlScheme;

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
            // show the control scheme
            

            //Wait initial delay time
            yield return new WaitForSeconds(DataManager.SECONDS_TO_START_MICROGAME);
            MicrogameStartText?.Invoke(startText);
            MicrogameStarted?.Invoke();
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

        protected virtual void OnMyGameAwake() 
        { 

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
            
            BossGameManager.Instance.MicrogameCompleted(wonMicrogame);
        }

        private void SetEffects()
        {
            foreach(MicrogameEffect effect in effects)
            {
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
                BossGameManager.Instance.ModifyTimeScale(effect.amount);
            }
        }
    }
}