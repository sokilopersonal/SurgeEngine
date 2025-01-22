using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class WoodBox : MonoBehaviour, IDamageable
    {
        [Header("Destroy")]
        [SerializeField] private DestroyedPiece destroyPiece;
        
        [Header("Sound")]
        [SerializeField] private EventReference BoxDestroySound;
        
        public void TakeDamage(object sender, float damage)
        {
            var piece = Instantiate(destroyPiece, transform.position, transform.rotation, null);
            piece.ApplyDirectionForce((sender as MonoBehaviour).GetComponentInParent<Rigidbody>().linearVelocity, 1.1f);
            
            RuntimeManager.PlayOneShot(BoxDestroySound, transform.position);
            Destroy(gameObject);
        }
    }
}