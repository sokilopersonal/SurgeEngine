using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorComponent : MonoBehaviour
    {
        protected Actor Actor;
        
        internal virtual void Set(Actor actor) => Actor = actor;
    }
}