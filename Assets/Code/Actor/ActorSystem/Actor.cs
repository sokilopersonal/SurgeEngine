using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.CameraSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    [DefaultExecutionOrder(-1000)]
    public class Actor : MonoBehaviour, IActor
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

        [HideInInspector] public new Rigidbody rigidbody;

        private void Awake()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            rigidbody = GetComponent<Rigidbody>();

            stateMachine = new FStateMachine();
            
            stateMachine.AddState(new FStateStart(this, rigidbody));
            stateMachine.AddState(new FStateIdle(this, rigidbody));
            stateMachine.AddState(new FStateGround(this, rigidbody));
            stateMachine.AddState(new FStateAir(this, rigidbody));
            stateMachine.AddState(new FStateAirBoost(this, rigidbody));
            stateMachine.AddState(new FStateStomp(this, rigidbody));
            stateMachine.AddState(new FStateHoming(this, rigidbody));
            stateMachine.AddState(new FStateAfterHoming(this));
            stateMachine.AddState(new FStateDrift(this, rigidbody));
            stateMachine.AddState(new FStateSpecialJump(this, rigidbody));
            stateMachine.AddState(new FStateSit(this, rigidbody));
            stateMachine.AddState(new FStateSliding(this, rigidbody));
            stateMachine.AddState(new FStateJump(this, rigidbody));
            stateMachine.AddState(new FStateQuickstep(this, rigidbody));
            stateMachine.AddState(new FStateGrind(this, rigidbody));
            stateMachine.AddState(new FStateGrindJump(this, rigidbody));
            stateMachine.AddState(new FStateGrindSquat(this, rigidbody));
            stateMachine.AddState(new FStateJumpSelector(this, rigidbody));
            stateMachine.AddState(new FStateJumpSelectorLaunch(this, rigidbody));
            
            var boost = new FBoost(this);
            stateMachine.AddSubState(boost);
            
            InitializeComponents();
        }

        public void InitializeComponents()
        {
            foreach (var component in new IActorComponent[] { input, stats, sounds, effects, camera, animation, model, flags, kinematics })
            {
                component?.SetOwner(this);
            }

            var startData = GetComponentInParent<ActorStartDefiner>().startData;
            if (startData.startType != StartType.None)
            {
                stateMachine.SetState<FStateStart>().SetData(startData);
            }
            else
            {
                stateMachine.SetState<FStateIdle>();
            }
        }
        
        private void Update()
        {
            stateMachine?.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine?.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            stateMachine?.LateTick(Time.deltaTime);
        }

        public void AddImpulse(Vector3 impulse)
        {
            rigidbody.AddForce(impulse, ForceMode.Impulse);
        }
    }
}