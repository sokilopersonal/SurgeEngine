using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic.Actions;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic
{
    public class Sonic : CharacterBase
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
            
            StateMachine.AddState(new FStateAirBoost(this));
            StateMachine.AddState(new FStateStomp(this));
            StateMachine.AddState(new FStateStompLand(this));
            StateMachine.AddState(new FStateHoming(this));
            StateMachine.AddState(new FStateAfterHoming(this));
            StateMachine.AddState(new FStateDrift(this));
            StateMachine.AddState(new FStateSlide(this));
            StateMachine.AddState(new FStateQuickstep(this));
            StateMachine.AddState(new FStateCrawl(this));
            StateMachine.AddState(new FStateSweepKick(this));

            StateMachine.AddSubState(new FBoost(this));
            StateMachine.AddSubState(new FSweepKick(this));

            _ = new SonicIdleActions(this);
            _ = new SonicGroundActions(this);
            _ = new SonicAirActions(this);
        }

        public override void Load()
        {
            var boost = StateMachine.GetSubState<FBoost>();
            boost.Active = false;
            boost.BoostEnergy = 0;
            (Effects as SonicEffects)?.BoostAura.Clear();
            
            base.Load();
        }
    }
}