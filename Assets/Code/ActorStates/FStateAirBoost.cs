using SurgeEngine.Code.ActorSystem;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAirBoost : FStateMove
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (actor.stats.boost.canAirBoost)
            {
                animation.TransitionToState("Air Boost", 0.5f);
                _rigidbody.linearVelocity = _rigidbody.transform.forward * actor.stats.boost.airStartForce;
                stats.boost.canAirBoost = false;
            }
            
            stateMachine.SetState<FStateAir>();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.SetAction(false);
        }
    }
}