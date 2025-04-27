using FMODUnity;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Physics
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