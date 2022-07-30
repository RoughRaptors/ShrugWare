using System;
using UnityEngine;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>Alerts the SceneTransitioner when the attached animator is finished</para>
    /// </summary>
    
    [RequireComponent(typeof(Animator))]
    public class TransitionAnimation : MonoBehaviour
    {
        public event Action OnTransitionFinished;

        public void TransitionFinished()
        {
            OnTransitionFinished?.Invoke();
        }
    }
}
