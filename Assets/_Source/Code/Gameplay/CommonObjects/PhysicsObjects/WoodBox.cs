using FMODUnity;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class WoodBox : MonoBehaviour, IDamageable
    {
        [Header("Destroy")]
        [SerializeField] private DestroyedPiece destroyPiece;
        
        [Header("Sound")]
        [SerializeField] private EventReference BoxDestroySound;
        
        public void TakeDamage(Component sender)
        {
            var piece = Instantiate(destroyPiece, transform.position, transform.rotation, null);
            piece.ApplyDirectionForce(sender.GetComponentInChildren<Rigidbody>().linearVelocity, 1.1f);
            
            RuntimeManager.PlayOneShot(BoxDestroySound, transform.position);
            Destroy(gameObject);
        }
    }
}