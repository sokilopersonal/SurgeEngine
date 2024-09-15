using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorComponent : MonoBehaviour
    {
        protected Actor owner;
        
        public void SetOwner(Actor actor)
        {
            owner = actor;
        }
    }
}