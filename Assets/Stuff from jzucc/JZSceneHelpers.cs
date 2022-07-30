using UnityEngine.SceneManagement;

public static class JZSceneHelpers
{
    /// <summary>
    /// <para>Determines scene name based off of the scene index</para>
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <returns></returns>
    public static string GetSceneNameFromIndex(int sceneIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
        string withExtension = path.Substring(path.LastIndexOf('/') + 1);
        string withoutExtension = withExtension.Substring(0, withExtension.LastIndexOf('.'));
        return withoutExtension;
    }

    /// <summary>
    /// <para>Determines scene index based off of the scene name</para>
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static int GetSceneIndexFromName(string sceneName)
    {
        for(int ii = 0; ii < SceneManager.sceneCountInBuildSettings; ii++)
        {
            string currentName = GetSceneNameFromIndex(ii);
            if(currentName != sceneName) continue;
            return ii;
        }

        return -1;
    }
}
