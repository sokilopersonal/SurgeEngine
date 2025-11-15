using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class Bomb : StageObject, IPointMarkerLoader
    {
        [SerializeField] private ParticleSystem explosion;

        private ParticleSystem _currentExplosion;
        
        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            var life = context.Life;
            if (!life.WillDie)
            {
                context.StateMachine.SetState<FStateStumble>(true).SetNoControlTime(0);
            }
            
            life.TakeDamage(life.WillDie);
            
            context.Rigidbody.AddForce(Vector3.up * 15, ForceMode.Impulse);
            
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