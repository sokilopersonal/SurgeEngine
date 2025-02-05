using SurgeEngine.Code.Enemy;

namespace SurgeEngine.Code.StateMachine
{
    public interface IEnemyComponent
    {
        EnemyBase enemyBase { get; set; }

        void SetOwner(EnemyBase owner)
        {
            enemyBase = owner;
        }
    }
}