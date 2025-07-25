using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        [Header("Destroy")]
        [SerializeField] protected DestroyedPiece destroyPiece;

        public virtual void TakeDamage(Component sender) { }
    }
}