using SurgeEngine.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGState : FEState
    {
        protected EggFighter eggFighter;

        public EGState(EnemyBase enemy) : base(enemy)
        {
            eggFighter = (EggFighter)enemy;
        }
    }
}