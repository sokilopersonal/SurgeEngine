using System;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public class DisableCollision
    {
        private CharacterBase _character;
        private Collider _collider;
        private Type _type;
        
        public void Disable(CharacterBase character, Collider collider, Type type)
        {
            _character = character;
            _collider = collider;
            _type = type;
            
            Physics.IgnoreCollision(_collider, _character.Model.Collision, true);
            Debug.Log("Ignoring collision");

            _character.StateMachine.OnStateAssign += OnState;
        }

        private void OnState(FState obj)
        {
            if (obj.GetType() != _type)
            {
                Physics.IgnoreCollision(_collider, _character.Model.Collision, false);
                _character.StateMachine.OnStateAssign -= OnState;
                Debug.Log("Stop collision");
            }
        }
    }
}