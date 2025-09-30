using System;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public class DisableCollision
    {
        private readonly CharacterBase _character;
        private readonly Collider _collider;
        private readonly Type _type;
        
        public DisableCollision(CharacterBase character, Collider collider, Type type)
        {
            _character = character;
            _collider = collider;
            _type = type;
            
            Physics.IgnoreCollision(_collider, _character.Model.collision, true);
            Debug.Log("Ignoring collision");

            _character.StateMachine.OnStateAssign += OnState;
        }

        private void OnState(FState obj)
        {
            if (obj.GetType() != _type)
            {
                Physics.IgnoreCollision(_collider, _character.Model.collision, false);
                _character.StateMachine.OnStateAssign -= OnState;
                Debug.Log("Stop collision");
            }
        }
    }
}