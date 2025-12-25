using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.System
{
    [RequireComponent(typeof(BoxCollider))]
    public class PointMarker : StageObject
    {
        [SerializeField, Range(0.5f, 5f)] private float length = 2f;
        public float Length => length;
        
        [Inject] private CharacterBase _character;

        private EventReference _soundEvent;
        private List<IPointMarkerLoader> _loaders = new();

        private bool _triggered;

        private void Awake()
        {
            _soundEvent = RuntimeManager.PathToEventReference("event:/CommonObjects/PointMarker");

            _loaders = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
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
            Vector3 newRotation = new Vector3(0f, currentRotation.y, 0f);
            Quaternion rotation = Quaternion.Euler(newRotation);
            
            _character.Rigidbody.position = transform.position;
            _character.Rigidbody.rotation = rotation;
            _character.Kinematics.Snap(transform.position, Vector3.up);

            var euler = rotation.eulerAngles;
            _character.Camera.StateMachine.SetDirection(euler.y, euler.x);
            _character.Camera.StateMachine.ClearVolumes();
            Physics.SyncTransforms();
            foreach (var volume in Utility.GetVolumesInBounds(_character.Rigidbody.position))
            {
                _character.Camera.StateMachine.RegisterVolume(volume);
            }
            
            foreach (var loader in _loaders)
            {
                loader.Load();
            }
        }
    }

    public interface IPointMarkerLoader
    {
        void Load();
    }
}