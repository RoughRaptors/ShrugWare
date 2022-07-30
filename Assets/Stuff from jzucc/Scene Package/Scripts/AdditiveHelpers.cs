using System.Collections;
using UnityEngine.SceneManagement;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>More specific Scene helpers</para>
    /// </summary>
    public static class AdditiveHelpers
    {    
        /// <summary>
        /// <para>Does an additive transition with scene transition data</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerator AdditiveTransition(SceneTransitionData data)
        {
            foreach(var scene in data.scenesToUnload)
            {
                if(data.unloadMyScene && scene == data.mySceneName) continue;
                yield return SceneManager.UnloadSceneAsync(scene);
            }

            if(data.unloadMyScene)
                yield return SceneManager.UnloadSceneAsync(data.mySceneName);

            yield return SceneManager.LoadSceneAsync(data.targetSceneIndex, LoadSceneMode.Additive);
        }
    }
}