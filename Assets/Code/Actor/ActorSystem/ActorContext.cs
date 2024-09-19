using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    [DefaultExecutionOrder(-2000)]
    public class ActorContext : MonoBehaviour
    {
        private static ActorContext _instance;
        public static Actor Context => _instance._actor;

        private Actor _actor;

        private void Awake()
        {
            _instance = this;
            
            _actor = GetComponent<Actor>();
        }
    }
}