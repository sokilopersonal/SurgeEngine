using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    /// <summary>
    /// Provides functionality to disable collision between a character and specified colliders.
    /// </summary>
    public class DisableCollision<T> where T : FState
    {
        private CharacterBase _character;
        private Collider _collider;

        public void Disable(CharacterBase character, Collider collider)
        {
            _character = character;
            _collider = collider;

            SetCollisionIgnored(true);
            Debug.Log("[Disable Collision] Ignoring");

            _character.StateMachine.OnStateAssign += OnStateAssigned;
        }

        private void OnStateAssigned(FState state)
        {
            if (state is T)
            {
                return;
            }

            SetCollisionIgnored(false);
            _character.StateMachine.OnStateAssign -= OnStateAssigned;
            Debug.Log("[Disable Collision] Stopped ignoring");
        }

        private void SetCollisionIgnored(bool ignored)
        {
            Physics.IgnoreCollision(_collider, _character.Model.Collision, ignored);
        }
    }
}