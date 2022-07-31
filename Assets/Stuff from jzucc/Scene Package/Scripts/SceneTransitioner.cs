using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>Handles scene transitions with animations</para>
    /// <para>Place in the initialization scene</para>
    /// </summary>
    public class SceneTransitioner : MonoBehaviour
    {
        #region //Animation Variables
        private TransitionAnimation[] animations = new TransitionAnimation[0];
        private List<Animator> animators = new List<Animator>();
        private Animator currentAnimator = null;
        #endregion

        #region //Transition Variables
        public static event Action StartTransitionOut;
        public static event Action StartTransitionIn;
        float preLoadBuffer = 0.35f;
        float postLoadBuffer = 0.35f;
        private bool transitioning = false;
        #endregion


        #region //Monobehaviour
        private void Awake() 
        {
            if(FindObjectsOfType<SceneTransitioner>().Length > 1)
                Destroy(gameObject);
            else DontDestroyOnLoad(gameObject);

            animations = GetComponentsInChildren<TransitionAnimation>(); 
            foreach(var animation in animations)
                animators.Add(animation.GetComponent<Animator>());
        }

        private void OnEnable() 
        {
            foreach(var animation in animations)
                animation.OnTransitionFinished += TransitionFinished;
        }

        private void OnDisable() 
        {
            foreach(var animation in animations)
                animation.OnTransitionFinished -= TransitionFinished;
        }
        #endregion

        #region //Transitions
        //Public
        public bool IsTransitioning()
        {
            return transitioning;
        }

        public void TransitionToScene(SceneTransitionData data)
        {
            if(IsTransitioning()) return;
            currentAnimator = animators[(int)data.animationType];
            StartCoroutine(TransitionCoroutine(data));
        }

        //Private
        private IEnumerator TransitionCoroutine(SceneTransitionData data)
        {
            //Transition out
            transitioning = true;
            currentAnimator.SetTrigger("TransitionOut");
            StartTransitionOut?.Invoke();

            //Transition
            while(currentAnimator.GetCurrentAnimatorClipInfoCount(0) == 0) yield return null;
            float animationDuration = currentAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration;
            yield return new WaitForSecondsRealtime(animationDuration + preLoadBuffer);
            yield return StartCoroutine(LoadNextScene(data));
            yield return new WaitForSecondsRealtime(postLoadBuffer);

            //Transition in
            currentAnimator.SetTrigger("TransitionIn");
            StartTransitionIn?.Invoke();
            transitioning = false;
            Time.timeScale = 1;
        }

        private IEnumerator LoadNextScene(SceneTransitionData data)
        {
            if(!data.additiveLoad)
            {
                yield return SceneManager.LoadSceneAsync(data.targetSceneIndex);
            }
            else
            {
                yield return StartCoroutine(AdditiveHelpers.AdditiveTransition(data));
            }
        }

        private void TransitionFinished()
        {
            currentAnimator = null;
        }
        #endregion
    }
}