using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class ReappearBlock : MonoBehaviour
    {
        [Header("Block Properties")]
        [SerializeField] private bool startHidden;
        [SerializeField] private Vector3 size = Vector3.one;
        [SerializeField] private Color blockColor = new Color(0.0f, 1.0f, 1.0f, 0.75f);
       
        [Header("Pattern")]
        [SerializeField] private bool alternate = true;
        [SerializeField] private float alternateTimer = 1f;

        [Header("Sound")]
        [SerializeField] private EventReference appearSound;
        [SerializeField] private EventReference disappearSound;

        private Vector3 _lastSize = Vector3.one;
        private Transform lightBlock;

        private readonly List<Transform> _pipes = new();
        private readonly float[] _pipeRotations = { -90, 180, -90, 180, 0, 90, 0, 90 };

        private readonly List<Transform> _joints = new();

        [SerializeField] private Material materialTemplate;

        private Material _lightMaterial = null;

        private BoxCollider _col;

        private bool _hidden;
        private float _timer;
        
        private void Start()
        {
            Setup();

            _hidden = startHidden;

            if (_hidden)
                Hide();
            else
                Show();

            VisualUpdate();
        }

        private void OnValidate()
        {
            Setup();
            
            VisualUpdate();
        }

        private void Setup()
        {
            lightBlock = transform.Find("lightBlock");

            MeshRenderer meshRenderer = lightBlock.GetComponent<MeshRenderer>();

            if (_lightMaterial == null)
            {
                _lightMaterial = new Material(materialTemplate);
            }

            meshRenderer.sharedMaterial = _lightMaterial;

            Transform pipesTransform = transform.Find("pipes");
            foreach (Transform pipe in pipesTransform)
            {
                if (!_pipes.Contains(pipe))
                    _pipes.Add(pipe);
            }

            Transform jointsTransform = transform.Find("joints");
            foreach (Transform joint in jointsTransform)
            {
                if (!_joints.Contains(joint))
                    _joints.Add(joint);
            }

            if (_col == null)
                _col = GetComponent<BoxCollider>();
        }

        private void VisualUpdate()
        {
            if (_lightMaterial != null)
            {
                _lightMaterial.color = blockColor;
                _lightMaterial.SetColor("_EmissiveColor", new Color(blockColor.r, blockColor.g, blockColor.b));
            }

            Vector3 abSize = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));

            // Half-size offsets
            Vector3 halfSize = abSize * 0.5f;

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

            lightBlock.localScale = abSize;
            _lastSize = abSize;

            float margin = 0.2f;
            _col.size = abSize + new Vector3(margin, margin, margin);
        }

        private void Update()
        {
            if (alternate)
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
            {
                RuntimeManager.PlayOneShot(appearSound, transform.position);
            }
        }

        public void Hide()
        {
            if (_lightMaterial == null)
                return;

            StartCoroutine(HideCoroutine());

            if (Time.timeSinceLevelLoad > 0)
            {
                RuntimeManager.PlayOneShot(disappearSound, transform.position);
            }
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
