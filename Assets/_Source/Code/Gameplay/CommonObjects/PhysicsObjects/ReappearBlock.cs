using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    [ExecuteAlways]
    public class ReappearBlock : MonoBehaviour
    {
        [SerializeField] private bool startHidden;
        [SerializeField] private Vector3 size = Vector3.one;
        [SerializeField] private bool alternate = true;
        [SerializeField] private float alternateTimer = 1f;
        [SerializeField] private EventReference appearSound;
        [SerializeField] private EventReference disappearSound;

        private Vector3 _lastSize = Vector3.one;

        private readonly List<Transform> _pipes = new();
        private readonly float[] _pipeRotations = { -90, 180, -90, 180, 0, 90, 0, 90 };

        private readonly List<Transform> _joints = new();
        private Transform _lightBlock;

        private Material _lightMaterial;

        private BoxCollider _col;

        private bool _initialized;
        private bool _hidden;
        private float _timer;
        
        private void Awake()
        {
            foreach (Transform pipe in transform.Find("pipes"))
                _pipes.Add(pipe);

            foreach (Transform joint in transform.Find("joints"))
                _joints.Add(joint);

            _lightBlock = transform.Find("lightBlock");

            _col = GetComponent<BoxCollider>();
            _lightMaterial = new Material(_lightBlock.GetComponent<MeshRenderer>().sharedMaterial);
            _lightBlock.GetComponent<MeshRenderer>().sharedMaterial = _lightMaterial;

            _hidden = startHidden;

            if (_hidden)
                Hide();
            else
                Show();

            VisualUpdate();
        }

        private void VisualUpdate()
        {
            // Half-size offsets
            Vector3 halfSize = size * 0.5f;

            // Vertex positions relative to the origin
            Vector3[] vertices = {
                new(-halfSize.x, -halfSize.y, -halfSize.z), // Bottom-left-back
                new( halfSize.x, -halfSize.y, -halfSize.z), // Bottom-right-back
                new(-halfSize.x,  halfSize.y, -halfSize.z), // Top-left-back
                new( halfSize.x,  halfSize.y, -halfSize.z), // Top-right-back
                new(-halfSize.x, -halfSize.y,  halfSize.z), // Bottom-left-front
                new( halfSize.x, -halfSize.y,  halfSize.z), // Bottom-right-front
                new(-halfSize.x,  halfSize.y,  halfSize.z), // Top-left-front
                new( halfSize.x,  halfSize.y,  halfSize.z)  // Top-right-front
            };

            float[] pipeLengths = {
                Vector3.Distance(vertices[0], vertices[1]),
                Vector3.Distance(vertices[1], vertices[5]),
                Vector3.Distance(vertices[2], vertices[3]),
                Vector3.Distance(vertices[3], vertices[7]),
                Vector3.Distance(vertices[4], vertices[0]),
                Vector3.Distance(vertices[5], vertices[4]),
                Vector3.Distance(vertices[6], vertices[2]),
                Vector3.Distance(vertices[7], vertices[6]),
                Vector3.Distance(vertices[0], vertices[2]),
                Vector3.Distance(vertices[1], vertices[3]),
                Vector3.Distance(vertices[0], vertices[2]),
                Vector3.Distance(vertices[1], vertices[3])
            };

            // Assign positions to joints
            for (int i = 0; i < _joints.Count; i++)
            {
                _joints[i].localPosition = vertices[i];
                _joints[i].localRotation = Quaternion.Euler(0, _pipeRotations[i], 0);
            }

            // Assign positions, rotations, and scales to pipes
            for (int i = 0; i < _pipes.Count; i++)
            {
                if (i < _joints.Count)
                {
                    _pipes[i].localPosition = _joints[i].localPosition;
                    _pipes[i].localRotation = Quaternion.Euler(0, _pipeRotations[i], 0);
                    _pipes[i].localScale = new Vector3(1, 1, pipeLengths[i]);
                }
                else
                {
                    _pipes[i].localPosition = _joints[i - 8].localPosition;
                    _pipes[i].localRotation = Quaternion.Euler(90, _pipeRotations[i - 8], 0);
                    _pipes[i].localScale = new Vector3(1, 1, pipeLengths[i]);

                    if (i > 9)
                        _pipes[i].localPosition = Vector3.Scale(_pipes[i].localPosition, new Vector3(1, -1, -1));
                }
            }

            _lightBlock.localScale = size;
            _lastSize = size;

            float margin = 0.2f;
            _col.size = size + new Vector3(margin, margin, margin);
        }

        private void Update()
        {
            if (alternate && Application.isPlaying)
            {
                _timer += Time.deltaTime;
                if (_timer > alternateTimer)
                {
                    _timer = 0f;
                    Toggle();
                }
            }

            if (_lastSize == size)
                return;

            VisualUpdate();
        }

        public void Toggle()
        {
            _hidden = !_hidden;

            if (_hidden)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            if (_lightMaterial == null)
                return;

            StartCoroutine(ShowCoroutine());
            
            if (Time.timeSinceLevelLoad > 0)
                RuntimeManager.PlayOneShot(appearSound, transform.position);
        }

        public void Hide()
        {
            if (_lightMaterial == null)
                return;

            StartCoroutine(HideCoroutine());
            
            if (Time.timeSinceLevelLoad > 0)
                RuntimeManager.PlayOneShot(disappearSound, transform.position);
        }

        private IEnumerator ShowCoroutine()
        {
            float time = 0f;
            float duration = 0.5f;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                var offset = Mathf.Lerp(-0.5f, 0f, SineOut(t));
                
                _lightMaterial.mainTextureOffset = new Vector2(offset, 0);
                _lightMaterial.SetTextureOffset("_EmissiveColorMap", new Vector2(offset, 0));

                if (offset > -0.4f)
                    _col.isTrigger = false;
                
                yield return null;
            }
        }

        private IEnumerator HideCoroutine()
        {
            float time = 0f;
            float duration = 0.5f;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                var offset = Mathf.Lerp(0f, -0.5f, SineOut(t));
                
                _lightMaterial.mainTextureOffset = new Vector2(offset, 0);
                _lightMaterial.SetTextureOffset("_EmissiveColorMap", new Vector2(offset, 0));

                if (offset < -0.4f)
                    _col.isTrigger = true;
                
                yield return null;
            }
        }

        private static float SineOut(float t)
        {
            return Mathf.Sin(t * Mathf.PI / 2);
        }
    }
}
