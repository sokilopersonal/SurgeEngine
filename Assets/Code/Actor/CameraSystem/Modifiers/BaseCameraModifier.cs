using System;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.CameraSystem.Modifiers
{
    public abstract class BaseCameraModifier : MonoBehaviour
    {
        public virtual void Set(Actor actor)
        {
            
        }
    }
}