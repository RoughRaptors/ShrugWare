namespace JZ.SCENE
{
    /// <summary>
    /// <para>Loads the next scene in the build index on press</para>
    /// </summary>
    public class LoadNextLevelButton : SceneButtonFunction
    {
        protected override void Awake()
        {
            base.Awake();
            transitionData.targetSceneIndex = gameObject.scene.buildIndex + 1;
        }
    }
}