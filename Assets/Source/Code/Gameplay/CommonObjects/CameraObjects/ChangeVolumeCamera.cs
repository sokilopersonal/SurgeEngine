using System.Collections.Generic;
using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ChangeVolumeCamera : StageObject, IHE1Importable, IHE1TargetResolvable
    {
        [SerializeField] private ObjCameraBase target;
        [SerializeField] private float easeTimeEnter = 1f;
        [SerializeField] private float easeTimeLeave = 1f;
        [SerializeField] private int priority;
        public ObjCameraBase Target => target;
        public float EaseTimeEnter => easeTimeEnter;
        public float EaseTimeLeave => easeTimeLeave;
        public int Priority => priority;

        private CharacterBase _character;

        private void OnDisable()
        {
            Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            if (target && other.transform.TryGetComponent(out CharacterBase character))
            {
                _character = character;
                _character.Camera.StateMachine.RegisterVolume(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (target && other.transform.TryGetComponent(out CharacterBase character))
            {
                Clear();
            }
        }

        private void Clear()
        {
            if (_character != null)
            {
                _character.Camera.StateMachine.UnregisterVolume(this);
                _character = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            easeTimeEnter = HE1Helper.GetFloat(elem, "Ease_Time_Enter");
            easeTimeLeave = HE1Helper.GetFloat(elem, "Ease_Time_Exit");
            priority = HE1Helper.GetInt(elem, "Priority");
            
            var box = GetComponent<BoxCollider>();
            float w = HE1Helper.GetFloat(elem, "Collision_Width");
            float h = HE1Helper.GetFloat(elem, "Collision_Height");
            float l = HE1Helper.GetFloat(elem, "Collision_Length");
            box.size = new Vector3(w, h, l);
        }
        
        public void ResolveTarget(XElement elem, Dictionary<long, StageObject> sceneObjects)
        {
            if (sceneObjects.TryGetValue(HE1Helper.GetTargetID(elem), out var value))
                target = value.GetComponent<ObjCameraBase>();    
        }
    }
}