using SurgeEngine.Code.Gameplay.CommonObjects.Sensors;
using SurgeEngine.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGState : FEState
    {
        protected readonly EggFighter eggFighter;
        protected readonly VisionSensor sensor;

        public EGState(EnemyBase enemy) : base(enemy)
        {
            eggFighter = (EggFighter)enemy;
            sensor = eggFighter.Sensor;
        }
    }
}