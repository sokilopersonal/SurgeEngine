using System;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : MonoBehaviour
    {
        [field: SerializeField, NaughtyAttributes.ReadOnly] public long SetID { get; set; }
        
        public Action<ContactBase> OnContact;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetCharacter(out CharacterBase ctx))
            {
                Contact(other, ctx);
            }
        }

        public virtual void Contact(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnObjectTriggered?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
    }
}