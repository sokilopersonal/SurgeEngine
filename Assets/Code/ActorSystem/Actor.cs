using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.CameraSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    [DefaultExecutionOrder(-1000)]
    public class Actor : MonoBehaviour
    {
        public ActorInput input;
        public new ActorCamera camera;
        public ActorStats stats;
        public new ActorAnimation animation;
        
        public FStateMachine stateMachine;
        public FActorState[] states;
        public FActorSubState[] subStates;

        private void Awake()
        {
            stateMachine = new FStateMachine();
            foreach (var state in states)
            {
                state.SetOwner(this);
                stateMachine.AddState(state);
            }

            foreach (var subState in subStates)
            {
                subState.SetOwner(this);
                stateMachine.AddSubState(subState);
            }
            
            stateMachine.SetState<FStateIdle>();
            
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            input?.SetOwner(this);
            camera?.SetOwner(this);
            stats?.SetOwner(this);
            animation?.SetOwner(this);
        }
        
        private void Update()
        {
            stateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            stateMachine.LateTick(Time.deltaTime);
        }
    }
}