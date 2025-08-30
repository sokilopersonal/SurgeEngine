using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace SurgeEngine._Source.Code.Scenes
{
    public class Disclaimer : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private PlayableAsset confirmAsset;

        private bool _startedLoading;
        
        private void OnStart()
        {
            director.Play(confirmAsset);
        }

        public void LoadIntoMenu()
        {
            if (!_startedLoading)
            {
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                _startedLoading = true;
            }
        }
    }
}