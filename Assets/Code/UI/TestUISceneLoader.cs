using SurgeEngine.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class TestUISceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneLoader.Instance.LoadScene(sceneName);
        }
    }
}