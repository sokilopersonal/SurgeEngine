using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class EventCollision : StageObject, IPointMarkerLoader, IHE1Importable
    {
        [SerializeField] private int defaultStatus;
        [SerializeField] private bool oneTime = true;
        [SerializeField] private int durability;
        [SerializeField] private UnityEvent eventOnContact;
        [SerializeField] private UnityEvent<CharacterBase> eventOnCharacterContact;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _collider.enabled = defaultStatus == 0;
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            eventOnContact.Invoke();
            eventOnCharacterContact.Invoke(context);

            if (durability == 0)
            {
                gameObject.SetActive(false);
            }
            else if (oneTime)
            {
                _collider.enabled = false;
            }
        }

        public void Load()
        {
            _collider.enabled = defaultStatus == 0;
            gameObject.SetActive(true);
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            var box = GetComponent<BoxCollider>();
            float w = HE1Helper.GetFloat(elem, "Collision_Width");
            float h = HE1Helper.GetFloat(elem, "Collision_Height");
            float l = HE1Helper.GetFloat(elem, "Collision_Length");
            box.size = new Vector3(w, h, l);
            
            defaultStatus = HE1Helper.GetInt(elem, "DefaultStatus");
            durability = HE1Helper.GetInt(elem, "Durability");
        }
    }
}