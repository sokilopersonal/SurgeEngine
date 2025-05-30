using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class Sonic : ActorBase
    {
        [SerializeField] private BoostConfig boostConfig;
        [SerializeField] private DriftConfig driftConfig;
        [SerializeField] private HomingConfig homingConfig;
        [SerializeField] private StompConfig stompConfig;
        [SerializeField] private SlideConfig slideConfig;
        [SerializeField] private QuickStepConfig quickstepConfig;
        [SerializeField] private CrawlConfig crawlConfig;
        [SerializeField] private SweepConfig sweepKickConfig;

        public BoostConfig BoostConfig => boostConfig;
        public DriftConfig DriftConfig => driftConfig; 
        public HomingConfig HomingConfig => homingConfig;
        public StompConfig StompConfig => stompConfig;
        public SlideConfig SlideConfig => slideConfig;
        public QuickStepConfig QuickstepConfig => quickstepConfig;
        public CrawlConfig CrawlConfig => crawlConfig;
        public SweepConfig SweepKickConfig => sweepKickConfig;

        protected override void InitializeConfigs()
        {
            base.InitializeConfigs();
            
            AddConfig(BoostConfig);
            AddConfig(DriftConfig);
            AddConfig(HomingConfig);
            AddConfig(StompConfig);
            AddConfig(SlideConfig);
            AddConfig(QuickstepConfig);
            AddConfig(CrawlConfig);
            AddConfig(SweepKickConfig);
        }

        protected override void AddStates()
        {
            base.AddStates();
            
            StateMachine.AddState(new FStateAirBoost(this, Rigidbody));
            StateMachine.AddState(new FStateStomp(this, Rigidbody));
            StateMachine.AddState(new FStateStompLand(this, Rigidbody));
            StateMachine.AddState(new FStateHoming(this, Rigidbody));
            StateMachine.AddState(new FStateAfterHoming(this));
            StateMachine.AddState(new FStateDrift(this, Rigidbody));
            StateMachine.AddState(new FStateSlide(this, Rigidbody));
            StateMachine.AddState(new FStateRunQuickstep(this, Rigidbody));
            StateMachine.AddState(new FStateQuickstep(this, Rigidbody));
            StateMachine.AddState(new FStateCrawl(this, Rigidbody));
            StateMachine.AddState(new FStateSweepKick(this, Rigidbody));

            StateMachine.AddSubState(new FBoost(this));
            StateMachine.AddSubState(new FSweepKick(this));
        }

        public override void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            var boost = StateMachine.GetSubState<FBoost>();
            boost.Active = false;
            boost.BoostEnergy = 0;
            (Effects as SonicEffects)?.BoostAura.Clear();
            
            base.Load(loadPosition, loadRotation);
        }
    }
}