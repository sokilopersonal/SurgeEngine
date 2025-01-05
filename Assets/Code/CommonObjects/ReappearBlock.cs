using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

namespace SurgeEngine.Code.CommonObjects
{
    [ExecuteAlways]
    public class ReappearBlock : MonoBehaviour
    {
        public bool startHidden = false;

        [Space(10)]

        public Vector3 size = Vector3.one;

        [Space(10)]

        public bool alternate = true;
        public float alternateTimer = 1f;

        [Space(10)]
        public EventReference appearSound;

        private Vector3 _lastSize = Vector3.one;

        private List<Transform> _pipes = new List<Transform>();
        private float[] _pipeRotations = new float[] { -90, 180, -90, 180, 0, 90, 0, 90 };

        private List<Transform> _joints = new List<Transform>();
        private Transform _lightBlock;

        private Material _lightMaterial;

        private BoxCollider _col;

        private bool _hidden = false;
        private float _timer;

        private void Start()
        {
            foreach (Transform pipe in transform.Find("pipes"))
                _pipes.Add(pipe);

            foreach (Transform joint in transform.Find("joints"))
                _joints.Add(joint);

            _lightBlock = transform.Find("lightBlock");

            _col = GetComponent<BoxCollider>();

            if (Application.isPlaying)
            {
                _lightMaterial = new Material(_lightBlock.GetComponent<MeshRenderer>().sharedMaterial);
                _lightBlock.GetComponent<MeshRenderer>().sharedMaterial = _lightMaterial;
            }

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
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), // Bottom-left-back
                new Vector3( halfSize.x, -halfSize.y, -halfSize.z), // Bottom-right-back
                new Vector3(-halfSize.x,  halfSize.y, -halfSize.z), // Top-left-back
                new Vector3( halfSize.x,  halfSize.y, -halfSize.z), // Top-right-back
                new Vector3(-halfSize.x, -halfSize.y,  halfSize.z), // Bottom-left-front
                new Vector3( halfSize.x, -halfSize.y,  halfSize.z), // Bottom-right-front
                new Vector3(-halfSize.x,  halfSize.y,  halfSize.z), // Top-left-front
                new Vector3( halfSize.x,  halfSize.y,  halfSize.z)  // Top-right-front
            };

            float[] pipeLengths = new float[]
            {
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

            float offset = -0.5f;
            DOTween.To(() => offset, x => offset = x, 0, 0.5f).SetEase(Ease.OutSine).OnUpdate(() =>
            {
                _lightMaterial.mainTextureOffset = new Vector2(offset, 0);
                _lightMaterial.SetTextureOffset("_EmissiveColorMap", new Vector2(offset, 0));

                if (offset > -0.4f)
                    _col.isTrigger = false;
            });

            if (Time.timeSinceLevelLoad > 0)
                RuntimeManager.PlayOneShot(appearSound, transform.position);
        }

        public void Hide()
        {
            if (_lightMaterial == null)
                return;

            float offset = 0;
            DOTween.To(() => offset, x => offset = x, -0.5f, 0.5f).SetEase(Ease.OutSine).OnUpdate(() =>
            {
                _lightMaterial.mainTextureOffset = new Vector2(offset, 0);
                _lightMaterial.SetTextureOffset("_EmissiveColorMap", new Vector2(offset, 0));
                
                if (offset < -0.4f)
                    _col.isTrigger = true;
            });
        }
    }
}
