using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class ActorSound : MonoBehaviour
    {
        protected Actor actor => ActorContext.Context;

        public virtual void Initialize()
        {
            
        }
    }
}