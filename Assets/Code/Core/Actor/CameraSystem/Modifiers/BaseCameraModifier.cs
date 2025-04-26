using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Actor.CameraSystem.Modifiers
{
    public abstract class BaseCameraModifier : MonoBehaviour
    {
        protected ActorBase Actor;
        
        [SerializeField] protected float multiplier = 2;
        
        public virtual void Set(ActorBase actor)
        {
            Actor = actor;
        }
    }
}