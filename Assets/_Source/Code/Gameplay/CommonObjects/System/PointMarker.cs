using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine._Source.Code.UI;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.System
{
    [RequireComponent(typeof(BoxCollider))]
    public class PointMarker : ContactBase
    {
        [SerializeField, Min(0)] private int id;
        [SerializeField, Range(0.5f, 5f)] private float length = 2f;
        public float Length => length;
        public int ID => id;

        [SerializeField] private PointMarkerLoadingScreen loadingScreenPrefab;
        [Inject] private CharacterBase _character;

        private EventReference _soundEvent;
        private List<IPointMarkerLoader> _loaders = new List<IPointMarkerLoader>();

        private bool _triggered;

        private void Awake()
        {
            _soundEvent = RuntimeManager.PathToEventReference("event:/CommonObjects/PointMarker");

            _loaders = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<IPointMarkerLoader>()
                .ToList();
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            var currentMarker = Stage.Instance.CurrentPointMarker;
            if (currentMarker != null && ID < currentMarker.ID)
            {
                Debug.LogWarning("[PointMarker] Point Marker skipped.");
                return;
            }
            
            if (!_triggered)
            {
                base.Contact(msg, context);
                RuntimeManager.PlayOneShot(_soundEvent, transform.position);
                _triggered = true; // We can't active the same marker twice
            }
        }

        public void Load()
        {
            StartCoroutine(LoadRoutine());
        }

        private IEnumerator LoadRoutine()
        {
            var canvas = Instantiate(loadingScreenPrefab);
            yield return canvas.Play();
            
            Vector3 currentRotation = transform.rotation.eulerAngles;
            Vector3 newRotation = new Vector3(0f, currentRotation.y, 0f);
            Quaternion rotation = Quaternion.Euler(newRotation);
            
            _character.Rigidbody.position = transform.position;
            _character.Rigidbody.rotation = rotation;
            _character.Kinematics.Snap(transform.position, Vector3.up);
            _character.Model.root.transform.rotation = rotation;
            
            _character.Camera.StateMachine.ClearVolumes();
            Physics.SyncTransforms();
            foreach (var volume in FindObjectsByType<ChangeCameraVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (volume.GetComponent<Collider>().bounds.Contains(_character.Rigidbody.position))
                {
                    _character.Camera.StateMachine.RegisterVolume(volume);
                }
            }
            
            foreach (var loader in _loaders)
            {
                loader.Load();
            }

            yield return canvas.Hide();
        }
        
        public void AddLoader(IPointMarkerLoader loader) => _loaders.Add(loader);
        public void RemoveLoader(IPointMarkerLoader loader) => _loaders.Remove(loader);
    }

    public interface IPointMarkerLoader
    {
        /// <summary>
        /// Load the player at the specified position and rotation.
        /// Make it virtual to, for example, disable boost on load.
        /// </summary>
        void Load();
    }
}