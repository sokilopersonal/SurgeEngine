using System;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class PlayerDamageObject : Entity
    {
        [SerializeField] private Transform damagePoint;
        [SerializeField] private Vector3 size = Vector3.one;
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            HurtBox.CreateAttached(this, damagePoint, size, HurtBoxTarget.Player);
        }

        private void OnDrawGizmos()
        {
            if (damagePoint == null)
                return;
            
            Gizmos.matrix = damagePoint.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, size * 2);
        }
    }
}