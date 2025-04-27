using System;
using SurgeEngine.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public static class ObjectEvents
    {
        public static Action<ContactBase> OnObjectCollected;
        public static Action<EnemyBase> OnEnemyDied;
    }
}