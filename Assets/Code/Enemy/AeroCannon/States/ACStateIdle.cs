namespace SurgeEngine.Code.Enemy.AeroCannon.States
{
    public class ACStateIdle : ACState
    {
        public ACStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnTick(float dt)
        {
            if (IsInSight(out var target))
            {
                stateMachine.SetState<ACStatePrepare>();
            }
        }
    }
}