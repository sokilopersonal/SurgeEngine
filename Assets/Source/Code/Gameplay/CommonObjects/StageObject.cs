using System;
using Alchemy.Inspector;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    [SelectionBase, DisallowMultipleComponent]
    public class StageObject : MonoBehaviour
    {
        [field: SerializeField, ReadOnly] public long SetID { get; set; } // SetObjectID is required for HE1Importer.
        
        public Action<StageObject> OnContact;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase ctx))
            {
                OnEnter(other, ctx);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase ctx))
            {
                OnExit(other, ctx);
            }
        }

        public virtual void OnEnter(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnObjectTriggered?.Invoke(this);
            
            OnContact?.Invoke(this);
        }

        public virtual void OnExit(Collider msg, CharacterBase context)
        {
            
        }

        public virtual void OnImport()
        {
            
        }
    }
}