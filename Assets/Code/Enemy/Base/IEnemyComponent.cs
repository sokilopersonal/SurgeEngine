namespace SurgeEngine.Code.Enemy
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