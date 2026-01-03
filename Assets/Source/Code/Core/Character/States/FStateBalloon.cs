using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateBalloon : FStateAir
    {
        float _airTime = 0f;
        public FStateBalloon(CharacterBase owner) : base(owner)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();
            _airTime = 0f;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            _airTime += dt;

            if (_airTime >= 1f)
                StateMachine.SetState<FStateAir>();
        }

        public override void Load()
        {
            base.Load();
        }
    }
}