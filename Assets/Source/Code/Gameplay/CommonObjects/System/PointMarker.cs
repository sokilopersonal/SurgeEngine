using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.System
{
    [RequireComponent(typeof(BoxCollider))]
    public class PointMarker : StageObject, IHE1Importable
    {
        [SerializeField, Min(0)] private float length = 2f;
        public float Length => length;
        
        [Inject] private CharacterBase _character;

        private EventReference _soundEvent;
        private List<IPointMarkerLoader> _loaders = new();

        private bool _triggered;

        private void Awake()
        {
            _soundEvent = RuntimeManager.PathToEventReference("event:/CommonObjects/PointMarker");

            _loaders = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include)
                .OfType<IPointMarkerLoader>()
                .ToList();
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            if (!_triggered)
            {
                base.OnEnter(msg, context);
                RuntimeManager.PlayOneShot(_soundEvent, transform.position);
                _triggered = true;
            }
        }

        public void Load()
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            Quaternion rotation = Quaternion.Euler(0f, currentRotation.y, 0f);

            Vector3 goal = transform.position;
            _character.Rigidbody.position = goal;
            _character.Rigidbody.rotation = rotation;
            Physics.SyncTransforms();

            _character.Kinematics.Snap(goal, Vector3.up);
            
            var euler = rotation.eulerAngles;
            _character.Camera.StateMachine.SetDirection(euler.y, euler.x);
            _character.Camera.StateMachine.ClearVolumes();
            foreach (var volume in Utility.GetVolumesInBounds(_character.Rigidbody.position))
            {
                _character.Camera.StateMachine.RegisterVolume(volume);
            }
            
            foreach (var loader in _loaders)
            {
                loader.Load();
            }
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            length = HE1Helper.GetFloat(elem, "Width");
        }
    }

    public interface IPointMarkerLoader
    {
        void Load();
    }
}