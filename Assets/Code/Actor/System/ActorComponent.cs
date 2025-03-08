using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorComponent : MonoBehaviour
    {
        public Actor Actor { get; private set; }

        internal virtual void Set(Actor actor) => Actor = actor;
    }
}