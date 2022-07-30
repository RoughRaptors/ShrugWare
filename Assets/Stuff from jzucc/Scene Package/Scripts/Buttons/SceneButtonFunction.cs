using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>Parent class for buttons that are to cause scene transitions</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class SceneButtonFunction : MonoBehaviour
    {
        private SceneTransitioner sceneTransitioner => FindObjectOfType<SceneTransitioner>();
        [SerializeField] protected SceneTransitionData transitionData;
        private bool animateTransition => transitionData.animationType != AnimType.none;
        private Button myButton = null;


        #region //Monobehvaiour
        protected virtual void Awake()
        {
            transitionData.mySceneIndex = gameObject.scene.buildIndex;
            transitionData.mySceneName = gameObject.scene.name;
            myButton = GetComponent<Button>();
        }

        protected virtual void OnEnable() 
        {
            myButton.onClick.AddListener(OnClick);
        }

        protected virtual void OnDisable()
        {
            myButton.onClick.RemoveListener(OnClick);
        }
        #endregion

        #region //Loading
        protected virtual void OnClick()
        {
            if(animateTransition)
            {
                sceneTransitioner.TransitionToScene(transitionData);
            }
            else if(transitionData.additiveLoad)
            {
                StartCoroutine(AdditiveHelpers.AdditiveTransition(transitionData));
            }
            else
            {
                SceneManager.LoadScene(transitionData.targetSceneIndex);
            }
        }
        #endregion
    }
}