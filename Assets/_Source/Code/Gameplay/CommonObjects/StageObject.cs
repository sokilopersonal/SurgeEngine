using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    [SelectionBase, DisallowMultipleComponent]
    public class StageObject : MonoBehaviour
    {
        [field: SerializeField] public long SetID { get; set; } // SetObjectID is required for HE1Importer.
        
        public Action<StageObject> OnContact;

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