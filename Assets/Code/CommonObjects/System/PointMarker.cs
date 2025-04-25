using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects.System
{
    [RequireComponent(typeof(BoxCollider))]
    public class PointMarker : ContactBase
    {
        [SerializeField, Min(0)] private int id;
        [SerializeField, Range(0.5f, 5f)] private float length = 2f;
        public float Length => length;
        public int ID => id;

        private EventReference _soundEvent;
        private List<IPointMarkerLoader> _loaders = new List<IPointMarkerLoader>();

        private bool _triggered;

        protected override void Awake()
        {
            base.Awake();

            _soundEvent = RuntimeManager.PathToEventReference("event:/CommonObjects/PointMarker");

            _loaders = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<IPointMarkerLoader>()
                .ToList();
        }

        public override void Contact(Collider msg)
        {
            var currentMarker = Stage.Instance.CurrentPointMarker;
            if (currentMarker != null && ID < currentMarker.ID)
            {
                Debug.LogWarning("[PointMarker] Point Marker skipped.");
                return;
            }
            
            if (!_triggered)
            {
                base.Contact(msg);
                RuntimeManager.PlayOneShot(_soundEvent, transform.position);
                _triggered = true; // We can't active the same marker twice
            }
        }

        public void Load()
        {
            foreach (var loader in _loaders)
            {
                loader.Load(transform.position + transform.up, transform.rotation);
            }
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
        void Load(Vector3 loadPosition, Quaternion loadRotation);
    }
}