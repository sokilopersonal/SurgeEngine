using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Enemy.EggFighter.States
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