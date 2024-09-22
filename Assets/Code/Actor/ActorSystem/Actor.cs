using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.CameraSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    [DefaultExecutionOrder(-1000)]
    public class Actor : MonoBehaviour
    {
        public ActorInput input;
        public ActorStats stats;
        public ActorSounds sounds;
        public new ActorCamera camera;
        public new ActorAnimation animation;
        public ActorEffects effects;
        public ActorModel model;
        
        public FStateMachine stateMachine;
        public FActorState[] states;
        public FActorSubState[] subStates;

        public int ID { get; private set; }

        [HideInInspector] public Rigidbody _rigidbody;

        private void Awake()
        {
            ID = gameObject.GetInstanceID();
            
            _rigidbody = GetComponent<Rigidbody>();
            
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
            stats?.SetOwner(this);
            sounds?.SetOwner(this);
            camera?.SetOwner(this);
            animation?.SetOwner(this);
            effects?.SetOwner(this);
            model?.SetOwner(this);
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