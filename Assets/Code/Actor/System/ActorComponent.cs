using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorComponent : MonoBehaviour
    {
        protected Actor actor => ActorContext.Context;
    }
}