using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Source.Code.Rendering
{
    [ExecuteAlways]
    public class RingsGPURenderer : MonoBehaviour
    {
        public static RingsGPURenderer Instance;

        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        private List<Transform> _rings = new();
        private List<Matrix4x4> _matrices = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (FindAnyObjectByType<RingsGPURenderer>()) return;
            
            var prefab = Resources.Load<GameObject>("RingsGPURenderer");
            Instantiate(prefab);
        }

        private void Awake()
        {
            Instance = this;

            if (material)
                material.enableInstancing = true;

            _rings ??= new List<Transform>();
        }

        public void Register(Transform ring)
        {
            if (!_rings.Contains(ring))
                _rings.Add(ring);
        }

        public void Unregister(Transform ring)
        {
            if (_rings.Contains(ring)) 
                _rings.Remove(ring);
        }

        private void Update()
        {
            if (!mesh || !material) return;

            _matrices.Clear();

            for (int i = 0; i < _rings.Count; i++)
            {
                var t = _rings[i];
                if (!t) continue;

                _matrices.Add(Matrix4x4.TRS(
                    t.position,
                    t.rotation,
                    t.lossyScale
                ));
            }

            if (_matrices.Count > 0)
                Graphics.DrawMeshInstanced(mesh, 0, material, _matrices);
        }
    }
}
