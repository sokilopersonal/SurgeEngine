using NaughtyAttributes;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.CameraSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class Actor : MonoBehaviour
    {
        [Foldout("Components")] public ActorInput input;
        [Foldout("Components")] public ActorStats stats;
        [Foldout("Components")] public ActorSounds sounds;
        [Foldout("Components")] public new ActorCamera camera;
        [Foldout("Components")] public new ActorAnimation animation;
        [Foldout("Components")] public ActorEffects effects;
        [Foldout("Components")] public ActorModel model;
        [Foldout("Components")] public ActorFlags flags;
        [Foldout("Components")] public ActorKinematics kinematics;
        [Foldout("Damage")] public LayerMask damageLayer;
        
        [Foldout("Base Physics")]
        public BaseActorConfig config;
        
        [HideInInspector] public FStateMachine stateMachine;
        
        private StartData _startData;

        public virtual void Initialize()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            Rigidbody body = GetComponent<Rigidbody>();
            stateMachine = new FStateMachine();

            body.centerOfMass -= Vector3.up * 0.5f;
            
            stateMachine.AddState(new FStateStart(this, body));
            stateMachine.AddState(new FStateIdle(this, body));
            stateMachine.AddState(new FStateGround(this, body));
            stateMachine.AddState(new FStateBrake(this, body));
            stateMachine.AddState(new FStateBrakeTurn(this, body));
            stateMachine.AddState(new FStateAir(this, body));
            stateMachine.AddState(new FStateSpecialJump(this, body));
            stateMachine.AddState(new FStateSit(this, body));
            stateMachine.AddState(new FStateJump(this, body));
            stateMachine.AddState(new FStateGrind(this, body));
            stateMachine.AddState(new FStateGrindJump(this, body));
            stateMachine.AddState(new FStateGrindSquat(this, body));
            stateMachine.AddState(new FStateJumpSelector(this, body));
            stateMachine.AddState(new FStateJumpSelectorLaunch(this, body));
            stateMachine.AddState(new FStateSwing(this, body));
            stateMachine.AddState(new FStateSwingJump(this, body));
            stateMachine.AddState(new FStateStompLand(this, body));
            
            FBoost boost = new FBoost(this);
            stateMachine.AddSubState(boost);

            FSweepKick sweepKick = new FSweepKick(this);
            stateMachine.AddSubState(sweepKick);
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

        public void SetStart(StartData data)
        {
            _startData = data;
            
            if (data.startType != StartType.None)
            {
                stateMachine.SetState<FStateStart>().SetData(data);
            }
            else
            {
                stateMachine.SetState<FStateIdle>();
            }
        }
        
        public StartData GetStartData() => _startData;
    }
}