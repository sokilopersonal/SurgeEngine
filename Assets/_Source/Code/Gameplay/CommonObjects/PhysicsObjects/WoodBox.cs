using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class WoodBox : BreakableObject
    {
        [Header("Sound")]
        [SerializeField] private EventReference BoxDestroySound;
        
        public override void TakeDamage(Component sender)
        {
            base.TakeDamage(sender);
            
            var piece = Instantiate(destroyPiece, transform.position, transform.rotation, null);
            piece.ApplyDirectionForce(sender.GetComponentInChildren<Rigidbody>().linearVelocity, 1.1f);
            
            RuntimeManager.PlayOneShot(BoxDestroySound, transform.position);
            Destroy(gameObject);
        }
    }
}