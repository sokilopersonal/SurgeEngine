using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class Bomb : StageObject, IPointMarkerLoader
    {
        [SerializeField] private ParticleSystem explosion;

        private ParticleSystem _currentExplosion;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            var life = context.Life;
            life.TakeDamage(life.WillDie);
            
            context.Rigidbody.AddForce(Vector3.up * 15, ForceMode.Impulse);

            if (!life.WillDie)
            {
                context.StateMachine.SetState<FStateStumble>(true).SetNoControlTime(0);
            }
            
            _currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(_currentExplosion.gameObject, 3f);
            
            gameObject.SetActive(false);
        }

        public void Load()
        {
            gameObject.SetActive(true);
            
            if (_currentExplosion != null)
            {
                Destroy(_currentExplosion.gameObject);
            }
        }
    }
}