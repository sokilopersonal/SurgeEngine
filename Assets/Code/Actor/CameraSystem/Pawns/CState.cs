using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CState : FState
    {
        protected readonly Actor _actor;
        protected readonly ActorCamera _master;
        protected readonly MasterCamera _stateMachine;

        protected object _data;

        public CState(Actor owner)
        {
            _actor = owner;

            _stateMachine = owner.camera.stateMachine;
            _master = _stateMachine.master;
        }
        
        public void SetData(object data)
        {
            _data = data;
        }
    }
}