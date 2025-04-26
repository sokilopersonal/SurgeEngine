using System;
using SurgeEngine.Code.Enemy;

namespace SurgeEngine.Code.CommonObjects
{
    public static class ObjectEvents
    {
        public static Action<ContactBase> OnObjectCollected;
        public static Action<EnemyBase> OnEnemyDied;
    }
}