using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurgeEngine.Code.CommonObjects
{
    public class Stage : MonoBehaviour
    {
        private static Stage _instance;
        
        public static Stage Instance => _instance;

        public StageData data;

        private void Awake()
        {
            _instance = this;
            
            data = new StageData()
            {
                stageName = SceneManager.GetActiveScene().name,
                ringCount = 0
            };
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectCollected += OnObjectCollected;
        }
        
        private void OnDisable()
        {
            ObjectEvents.OnObjectCollected -= OnObjectCollected;
        }

        private void OnObjectCollected(ContactBase obj)
        {
            if (obj is Ring)
            {
                data.ringCount += 1;
            }
        }
    }
    
    public class StageData
    {
        public string stageName;
        public int ringCount;
    }
}