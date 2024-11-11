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

        protected object _data;

        public CState(Actor owner)
        {
            _actor = owner;

            _stateMachine = owner.camera.stateMachine;
        }
        
        public void SetData(object data)
        {
            _data = data;
        }
    }
}