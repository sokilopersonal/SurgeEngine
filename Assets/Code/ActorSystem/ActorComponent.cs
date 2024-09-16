using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorComponent : MonoBehaviour
    {
        protected Actor actor;
        
        public void SetOwner(Actor actor)
        {
            this.actor = actor;
        }
    }
}