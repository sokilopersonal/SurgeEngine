using FMODUnity;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.PhysicsObjects
{
    [SelectionBase]
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        [Header("Destroy")]
        [SerializeField] protected DestroyedPiece destroyPiece;
        [SerializeField] protected EventReference destroyEvent;

        public virtual void TakeDamage(Component sender)
        {
            var piece = Instantiate(destroyPiece, transform.position, transform.rotation, null);
            piece.ApplyDirectionForce(sender.GetComponentInChildren<Rigidbody>().linearVelocity, 1.1f);
            
            RuntimeManager.PlayOneShot(destroyEvent, transform.position);
            Destroy(gameObject);
        }
    }
}