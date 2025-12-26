using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SurgeEngine.Source.Code.Core.Character.CameraSystem;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Config;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterBase : MonoBehaviour, IPointMarkerLoader
    {
        public new Transform transform => Rigidbody.transform;
        
        [Header("Components")]
        [SerializeField] private CharacterInput input;
        [SerializeField] private CharacterSounds sounds; 
        [SerializeField] private new CharacterCamera camera;
        [SerializeField] private new CharacterAnimation animation;
        [SerializeField] private CharacterEffects effects;
        [SerializeField] private CharacterModel model;
        [SerializeField] private CharacterFlags flags;
        [SerializeField] private CharacterKinematics kinematics;
        [SerializeField] private CharacterLife life;
        public CharacterInput Input => input;
        public CharacterSounds Sounds => sounds;
        public CharacterCamera Camera => camera; 
        public CharacterAnimation Animation => animation;
        public CharacterEffects Effects => effects;
        public CharacterModel Model => model;
        public CharacterFlags Flags => flags;
        public CharacterKinematics Kinematics => kinematics;
        public CharacterLife Life => life;

        [Header("Config")]
        [SerializeField] private PhysicsConfig config;
        public PhysicsConfig Config => config;
        
        private readonly Dictionary<Type, ScriptableObject> _configs = new();
        
        private StartData _startData;

        public FStateMachine StateMachine { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            StateMachine = new FStateMachine();
            Rigidbody = GetComponent<Rigidbody>();
            
            CharacterContext.Set(this);
            
            InitializeConfigs();
            AddStates();
            
            Input.Set(this);
            Sounds.Set(this);
            Camera.Set(this);
            Animation.Set(this);
            Effects.Set(this);
            Model.Set(this);
            Flags.Set(this);
            Kinematics.Set(this);
            Life.Set(this);
        }

        private void Update()
        {
            StateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick(Time.fixedDeltaTime);
        }

        protected virtual void AddStates()
        {
            StateMachine.AddState(new FStateStart(this));
            StateMachine.AddState(new FStateIdle(this));
            StateMachine.AddState(new FStateGround(this));
            StateMachine.AddState(new FStateSlip(this));
            StateMachine.AddState(new FStateBrake(this));
            StateMachine.AddState(new FStateBrakeTurn(this));
            StateMachine.AddState(new FStateAir(this));
            StateMachine.AddState(new FStateSkydive(this));
            StateMachine.AddState(new FStateSpecialJump(this));
            StateMachine.AddState(new FStateSit(this));
            StateMachine.AddState(new FStateJump(this));
            StateMachine.AddState(new FStateGrind(this));
            StateMachine.AddState(new FStateGrindJump(this));
            StateMachine.AddState(new FStateGrindSquat(this));
            StateMachine.AddState(new FStateRailSwitch(this));
            StateMachine.AddState(new FStateJumpSelector(this));
            StateMachine.AddState(new FStateJumpSelectorLaunch(this));
            StateMachine.AddState(new FStateJumpSelectorMissLand(this));
            StateMachine.AddState(new FStateSwing(this));
            StateMachine.AddState(new FStateSwingJump(this));
            StateMachine.AddState(new FStateDamage(this));
            StateMachine.AddState(new FStateDamageLand(this));
            StateMachine.AddState(new FStateUpreel(this));
            StateMachine.AddState(new FStatePulley(this));
            StateMachine.AddState(new FStateTrickJump(this));
            StateMachine.AddState(new FStateTrick(this));
            StateMachine.AddState(new FStateSpring(this));
            StateMachine.AddState(new FStateJumpPanel(this));
            StateMachine.AddState(new FStateDashRing(this));
            StateMachine.AddState(new FStateDead(this));
            StateMachine.AddState(new FStateGoal(this));
            StateMachine.AddState(new FStateStumble(this));
        }

        public void SetStart(StartData data)
        {
            _startData = data;
            
            if (data.startType != StartType.None)
            {
                StateMachine.GetState<FStateStart>().SetData(data);
                StateMachine.SetState<FStateStart>();
            }
            else
            {
                StateMachine.SetState<FStateIdle>();
            }
            
            Model.Root.forward = transform.forward;
        }

        protected virtual void InitializeConfigs()
        {
            AddConfig(Config);
        }
        
        protected void AddConfig(ScriptableObject obj)
        {
            _configs.Add(obj.GetType(), obj);
        }
        
        public void TryGetConfig<T>(out T request) where T : ScriptableObject
        {
            if (_configs.TryGetValue(typeof(T), out var result))
            {
                request = (T)result;
                return;
            }

            request = null;
        }
        
        public virtual void Load()
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Animation.StateAnimator.TransitionToState("Idle", 0f);
            Flags.AddFlag(new Flag(FlagType.OutOfControl, true, 0.5f));
            Input.playerInput.enabled = true;

            if (StateMachine.CurrentState is IPointMarkerLoader stateLoader)
            {
                stateLoader.Load();
            }
            
            StateMachine.SetState<FStateIdle>();
        }

        public StartData GetStartData() => _startData;
    }
}