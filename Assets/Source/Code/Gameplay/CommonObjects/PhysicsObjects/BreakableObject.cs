using FMODUnity;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    [SelectionBase, RequireComponent(typeof(Rigidbody))]
    public class BreakableObject : StageObject, IDamageable
    {
        [Header("Destroy")]
        [SerializeField] protected DestroyedPiece destroyPiece;
        [SerializeField] protected EventReference destroyEvent;
        
        private Rigidbody _rigidbody;
        private bool _destroyed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void TakeDamage(Component sender)
        {
            Destroy(sender.GetComponentInChildren<Rigidbody>().linearVelocity);
        }

        private void Destroy(Vector3 piecesForce)
        {
            if (!_destroyed)
            {
                var piece = Instantiate(destroyPiece, transform.position, transform.rotation, null);
                piece.ApplyDirectionForce(piecesForce, 1.1f);
            
                RuntimeManager.PlayOneShot(destroyEvent, transform.position);
                Destroy(gameObject);
                _destroyed = true;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            
        }
    }
}