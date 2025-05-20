using System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public static class ObjectEvents
    {
        public static Action<ContactBase> OnObjectCollected;
        public static Action<EnemyBase> OnEnemyDied;

        public static Action<TrickJumper> OnTrickJumperTriggered;
    }
}