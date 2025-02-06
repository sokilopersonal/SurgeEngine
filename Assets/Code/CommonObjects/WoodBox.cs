using FMODUnity;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class WoodBox : MonoBehaviour, IDamageable
    {
        [Header("Destroy")]
        [SerializeField] private DestroyedPiece destroyPiece;
        
        [Header("Sound")]
        [SerializeField] private EventReference BoxDestroySound;
        
        public void TakeDamage(Entity sender, float damage)
        {
            var piece = Instantiate(destroyPiece, transform.position, transform.rotation, null);
            piece.ApplyDirectionForce(sender.GetComponentInParent<Rigidbody>().linearVelocity, 1.1f);
            
            RuntimeManager.PlayOneShot(BoxDestroySound, transform.position);
            Destroy(gameObject);
        }
    }
}