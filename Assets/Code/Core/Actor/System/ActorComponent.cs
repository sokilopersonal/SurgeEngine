using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorComponent : MonoBehaviour
    {
        public ActorBase Actor { get; private set; }

        internal virtual void Set(ActorBase actor) => Actor = actor;
    }
}