using NaughtyAttributes;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Actor.System.Actors
{
    public class Sonic : ActorBase
    {
        [Foldout("Boost")]
        [Expandable] public BoostConfig boostConfig;
        
        [Foldout("Drift")]
        [Expandable] public DriftConfig driftConfig;

        [Foldout("Homing")]
        [Expandable] public HomingConfig homingConfig;
        
        [Foldout("Stomp")]
        [Expandable] public StompConfig stompConfig;
        
        [Foldout("Slide")]
        [Expandable] public SlideConfig slideConfig;
        
        [Foldout("Quickstep")]
        [Expandable] public QuickStepConfig quickstepConfig;

        [Foldout("Crawl")]
        [Expandable] public CrawlConfig crawlConfig;

        [Foldout("SweepKick")]
        [Expandable] public SweepConfig sweepKickConfig;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void InitializeConfigs()
        {
            base.InitializeConfigs();
            
            AddConfig(boostConfig);
            AddConfig(driftConfig);
            AddConfig(homingConfig);
            AddConfig(stompConfig);
            AddConfig(slideConfig);
            AddConfig(quickstepConfig);
            AddConfig(crawlConfig);
            AddConfig(sweepKickConfig);
        }

        protected override void AddStates()
        {
            base.AddStates();
            
            Rigidbody body = GetComponent<Rigidbody>();
            
            stateMachine.AddState(new FStateAirBoost(this, body));
            stateMachine.AddState(new FStateStomp(this, body));
            stateMachine.AddState(new FStateStompLand(this, body));
            stateMachine.AddState(new FStateHoming(this, body));
            stateMachine.AddState(new FStateAfterHoming(this));
            stateMachine.AddState(new FStateDrift(this, body));
            stateMachine.AddState(new FStateSlide(this, body));
            stateMachine.AddState(new FStateRunQuickstep(this, body));
            stateMachine.AddState(new FStateQuickstep(this, body));
            stateMachine.AddState(new FStateCrawl(this, body));
            stateMachine.AddState(new FStateSweepKick(this, body));

            stateMachine.AddSubState(new FBoost(this));
            stateMachine.AddSubState(new FSweepKick(this));
        }

        public override void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            stateMachine.GetSubState<FBoost>().Active = false;
            
            base.Load(loadPosition, loadRotation);
        }
    }
}