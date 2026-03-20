using SurgeEngine.Source.Code.Gameplay.CommonObjects.Sensors;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine.AI;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States
{
    public abstract class EGState : FEState
    {
        protected readonly EggFighter EggFighter;
        protected readonly NavMeshAgent Agent;
        protected readonly VisionSensor Sensor;

        protected EGState(EnemyBase enemy) : base(enemy)
        {
            EggFighter = (EggFighter)enemy;
            Agent = EggFighter.Agent;
            Sensor = EggFighter.Sensor;
        }

        protected bool IsNavMeshValid()
        {
            var path = new NavMeshPath();
            bool result = Agent.CalculatePath(Transform.position, path);
            if (result)
                return path.status == NavMeshPathStatus.PathComplete;
            return false;
        }
    }
}