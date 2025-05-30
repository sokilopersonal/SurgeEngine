using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class Sonic : ActorBase
    {
        public BoostConfig boostConfig;
        public DriftConfig driftConfig;
        public HomingConfig homingConfig;
        public StompConfig stompConfig;
        public SlideConfig slideConfig;
        public QuickStepConfig quickstepConfig;
        public CrawlConfig crawlConfig;
        public SweepConfig sweepKickConfig;

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
            
            stateMachine.AddState(new FStateAirBoost(this, Rigidbody));
            stateMachine.AddState(new FStateStomp(this, Rigidbody));
            stateMachine.AddState(new FStateStompLand(this, Rigidbody));
            stateMachine.AddState(new FStateHoming(this, Rigidbody));
            stateMachine.AddState(new FStateAfterHoming(this));
            stateMachine.AddState(new FStateDrift(this, Rigidbody));
            stateMachine.AddState(new FStateSlide(this, Rigidbody));
            stateMachine.AddState(new FStateRunQuickstep(this, Rigidbody));
            stateMachine.AddState(new FStateQuickstep(this, Rigidbody));
            stateMachine.AddState(new FStateCrawl(this, Rigidbody));
            stateMachine.AddState(new FStateSweepKick(this, Rigidbody));

            stateMachine.AddSubState(new FBoost(this));
            stateMachine.AddSubState(new FSweepKick(this));
        }

        public override void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            var boost = stateMachine.GetSubState<FBoost>();
            boost.Active = false;
            boost.BoostEnergy = 0;
            (Effects as SonicEffects)?.BoostAura.Clear();
            
            base.Load(loadPosition, loadRotation);
        }
    }
}