using NaughtyAttributes;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.CameraSystem;
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
        
        [Foldout("Base Physics")]
        public BaseActorConfig config;
        
        [HideInInspector] public FStateMachine stateMachine;

        public virtual void Initialize()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            var body = GetComponent<Rigidbody>();
            stateMachine = new FStateMachine();
            
            stateMachine.AddState(new FStateStart(this, body));
            stateMachine.AddState(new FStateIdle(this, body));
            stateMachine.AddState(new FStateGround(this, body));
            stateMachine.AddState(new FStateAir(this, body));
            stateMachine.AddState(new FStateSpecialJump(this, body));
            stateMachine.AddState(new FStateSit(this, body));
            stateMachine.AddState(new FStateJump(this, body));
            stateMachine.AddState(new FStateGrind(this, body));
            stateMachine.AddState(new FStateGrindJump(this, body));
            stateMachine.AddState(new FStateGrindSquat(this, body));
            stateMachine.AddState(new FStateJumpSelector(this, body));
            stateMachine.AddState(new FStateJumpSelectorLaunch(this, body));
            
            var boost = new FBoost(this);
            stateMachine.AddSubState(boost);
            
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
    }
}