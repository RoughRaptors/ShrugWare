using UnityEngine;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>Changes to player specified scene on press</para>
    /// </summary>
    public class SceneChangeButton : SceneButtonFunction
    {
        [SerializeField] string targetScene;

        protected override void Awake()
        {
            base.Awake();
            transitionData.targetSceneIndex = JZSceneHelpers.GetSceneIndexFromName(targetScene);
        }
    }
}