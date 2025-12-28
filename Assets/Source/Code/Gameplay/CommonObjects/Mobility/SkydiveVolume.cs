using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;


namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class SkydiveVolume : StageObject
    {
        [SerializeField] private bool isFinish = false;
        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (isFinish)
            {
                if (context.StateMachine.CurrentState is FStateSkydive)
                    context.StateMachine.SetState<FStateAir>();
            }
            else
            {
                if (!(context.StateMachine.CurrentState is FStateSkydive))
                    context.StateMachine.SetState<FStateSkydive>();
            }
        }
    }
}
