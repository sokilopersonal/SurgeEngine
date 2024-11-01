using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.CameraSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.Splines;

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
        public ActorFlags flags;
        public ActorKinematics kinematics;
        
        public FStateMachine stateMachine;

        public int ID { get; private set; }

        [HideInInspector] public new Rigidbody rigidbody;
        [HideInInspector] public PathData pathData;
        [HideInInspector] public SplineContainer container;

        private void Awake()
        {
            ID = gameObject.GetInstanceID();
            
            rigidbody = GetComponent<Rigidbody>();
            pathData = null;
            container = null;
            
            stateMachine = new FStateMachine();
            
            stateMachine.AddState(new FStateIdle(this, rigidbody));
            stateMachine.AddState(new FStateGround(this, rigidbody));
            stateMachine.AddState(new FStateAir(this, rigidbody));
            stateMachine.AddState(new FStateAirBoost(this, rigidbody));
            stateMachine.AddState(new FStateStomp(this, rigidbody));
            stateMachine.AddState(new FStateHoming(this, rigidbody));
            stateMachine.AddState(new FStateDrift(this, rigidbody));
            stateMachine.AddState(new FStateSpecialJump(this, rigidbody));
            stateMachine.AddState(new FStateSit(this, rigidbody));
            stateMachine.AddState(new FStateSliding(this, rigidbody));
            stateMachine.AddState(new FStateJump(this, rigidbody));
            
            var boost = new FBoost(this);
            stateMachine.AddSubState(boost);
            
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
            flags?.SetOwner(this);
            kinematics?.SetOwner(this);
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

        public void AddImpulse(Vector3 impulse)
        {
            rigidbody.AddForce(impulse, ForceMode.Impulse);
        }
    }
}