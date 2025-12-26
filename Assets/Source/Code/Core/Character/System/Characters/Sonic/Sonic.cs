using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic.Actions;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic
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
        [SerializeField] private LightSpeedDashConfig lightSpeedDashConfig;
        [SerializeField] private SkydiveConfig skydiveConfig;

        public BoostConfig BoostConfig => boostConfig;
        public DriftConfig DriftConfig => driftConfig; 
        public HomingConfig HomingConfig => homingConfig;
        public StompConfig StompConfig => stompConfig;
        public SlideConfig SlideConfig => slideConfig;
        public QuickStepConfig QuickstepConfig => quickstepConfig;
        public CrawlConfig CrawlConfig => crawlConfig;
        public SweepConfig SweepKickConfig => sweepKickConfig;
        public LightSpeedDashConfig LightSpeedDashConfig => lightSpeedDashConfig;
        public SkydiveConfig SkyDiveConfig => skydiveConfig;

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
            AddConfig(LightSpeedDashConfig);
            AddConfig(SkyDiveConfig);
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
            StateMachine.AddState(new FStateLightSpeedDash(this));

            StateMachine.AddSubState(new FBoost(this));
            StateMachine.AddSubState(new FSweepKick(this));
            StateMachine.AddSubState(new FRingDashSearch(this));

            _ = new SonicIdleActions(this);
            _ = new SonicGroundActions(this);
            _ = new SonicAirActions(this);
        }

        public override void Load()
        {
            if (StateMachine.GetState(out FBoost boost))
            {
                boost.Active = false;
                boost.BoostEnergy = 0;
                (Effects as SonicEffects)?.BoostAura.Clear();
            }

            base.Load();
        }
    }
}