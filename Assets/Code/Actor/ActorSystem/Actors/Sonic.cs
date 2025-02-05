using NaughtyAttributes;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem.Actors
{
    public class Sonic : Actor
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
    }
}