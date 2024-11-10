using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CState : FState
    {
        protected Actor _actor;
        
        protected MasterCamera _stateMachine;

        public CState(Camera camera, Transform transform, Actor owner)
        {
            _actor = owner;

            _stateMachine = owner.camera.stateMachine;
        }
    }
}