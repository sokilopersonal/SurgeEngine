using SurgeEngine.Code.Custom;

namespace SurgeEngine.Code.Enemy.AeroCannon.States
{
    public class ACStateIdle : ACState
    {
        public ACStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = config.idleTime;
        }

        public override void OnTick(float dt)
        {
            if (Common.TickTimer(ref timer, config.idleTime, false))
            {
                if (IsInSight(out var target))
                {
                    stateMachine.SetState<ACStatePrepare>();
                }
            }
        }
    }
}