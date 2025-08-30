using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.States
{
    public abstract class FStateObject : FCharacterState
    {
        private float _ignoranceTime;
        protected bool Ignore => _ignoranceTime > 0;
        
        public FStateObject(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0.2f);
            _ignoranceTime = 0.2f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _ignoranceTime -= dt;
        }
    }
}