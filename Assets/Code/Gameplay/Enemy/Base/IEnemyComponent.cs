namespace SurgeEngine.Code.Gameplay.Enemy.Base
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