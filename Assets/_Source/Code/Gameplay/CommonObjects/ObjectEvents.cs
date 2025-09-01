using System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine._Source.Code.Gameplay.Enemy.Base;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public static class ObjectEvents
    {
        public static Action<ContactBase> OnObjectTriggered;
        public static Action<EnemyBase> OnEnemyDied;

        public static Action<TrickJumper> OnTrickJumperTriggered;
    }
}