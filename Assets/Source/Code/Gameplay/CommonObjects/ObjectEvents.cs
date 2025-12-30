using System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public static class ObjectEvents
    {
        public static Action<StageObject> OnObjectTriggered;
        public static Action<EnemyBase> OnEnemyDied;

        public static Action<TrickJumper> OnTrickJumperTriggered;
        public static Action<ReactionPlate> OnReactionPanelTriggered;
    }
}