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
            piece.ApplyExplosionForce(10f, (sender as MonoBehaviour).transform.position, 3f);
            
            RuntimeManager.PlayOneShot(BoxDestroySound, transform.position);
            Destroy(gameObject);
        }
    }
}