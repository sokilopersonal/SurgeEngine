using NaughtyAttributes;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
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

        public override void Initialize()
        {
            base.Initialize();
            
            Rigidbody body = GetComponent<Rigidbody>();
            
            stateMachine.AddState(new FStateAirBoost(this, body));
            stateMachine.AddState(new FStateStomp(this, body));
            stateMachine.AddState(new FStateHoming(this, body));
            stateMachine.AddState(new FStateAfterHoming(this));
            stateMachine.AddState(new FStateDrift(this, body));
            stateMachine.AddState(new FStateSlide(this, body));
            stateMachine.AddState(new FStateQuickstep(this, body));
            stateMachine.AddState(new FStateCrawl(this, body));
        }
    }
}