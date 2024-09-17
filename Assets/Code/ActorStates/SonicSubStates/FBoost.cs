using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.ActorStates.SonicSubStates
{
    public class FBoost : FActorSubState
    {
        public float turnSpeedReduction;
        public float maxSpeedMultiplier;
        public float startForce;
        public float airStartForce;
        public bool canAirBoost;
        public float boostForce;

        private void Awake()
        {
            canAirBoost = true;
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateEnter += OnStateEnter;
        }
        
        private void OnDisable()
        {
            actor.stateMachine.OnStateEnter -= OnStateEnter;
        }

        private void OnStateEnter(FState obj)
        {
            if (obj is FStateGround)
            {
                canAirBoost = true;
            }
        }
    }
}