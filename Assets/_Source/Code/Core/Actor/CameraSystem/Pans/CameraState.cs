using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraState : FState
    {
        protected readonly ActorBase _actor;
        protected readonly ActorCamera _master;
        protected readonly CameraStateMachine _stateMachine;

        public Vector3 StatePosition { get; protected set; }
        public Quaternion StateRotation { get; protected set; }
        public float StateFOV { get; protected set; }

        protected CameraState(ActorBase owner)
        {
            _actor = owner;
            _stateMachine = owner.Camera.StateMachine;
            _master= _stateMachine.Master;
        }
    }
}