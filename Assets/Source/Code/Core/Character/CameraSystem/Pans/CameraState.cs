using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans
{
    public class CameraState : FState
    {
        protected readonly CharacterBase Character;
        protected readonly CharacterCamera _master;
        protected readonly CameraStateMachine _stateMachine;

        public Vector3 StatePosition { get; protected set; }
        public Quaternion StateRotation { get; protected set; }
        public float StateFOV { get; protected set; }

        protected CameraState(CharacterBase owner)
        {
            Character = owner;
            _stateMachine = owner.Camera.StateMachine;
            _master= _stateMachine.Master;
        }
    }
}