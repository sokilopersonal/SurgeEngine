using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace SurgeEngine.Source.Code.Scenes
{
    public class Disclaimer : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private PlayableAsset confirmAsset;

        private bool _startedLoading;
        
        private void OnStart()
        {
            if (!_startedLoading)
            {
                director.Play(confirmAsset);
                _startedLoading = true;
            }
        }

        public void LoadIntoMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}