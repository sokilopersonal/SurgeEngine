using UnityEngine;

namespace SurgeEngine.Source.Code.UI.MainMenu
{
    public class MainMenuExiter : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}