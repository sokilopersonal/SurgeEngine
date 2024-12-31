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

        Vector3 lastSize = Vector3.one;

        List<Transform> pipes = new List<Transform>();
        float[] pipeRotations = new float[] { -90, 180, -90, 180, 0, 90, 0, 90 };

        List<Transform> joints = new List<Transform>();
        Transform lightBlock;

        Material lightMaterial;

        BoxCollider col;

        bool hidden = false;
        float timer;
        void Start()
        {
            foreach (Transform pipe in transform.Find("pipes"))
                pipes.Add(pipe);

            foreach (Transform joint in transform.Find("joints"))
                joints.Add(joint);

            lightBlock = transform.Find("lightBlock");

            col = GetComponent<BoxCollider>();

            if (Application.isPlaying)
            {
                lightMaterial = new Material(lightBlock.GetComponent<MeshRenderer>().sharedMaterial);
                lightBlock.GetComponent<MeshRenderer>().sharedMaterial = lightMaterial;
            }

            hidden = startHidden;

            if (hidden)
                Hide();
            else
                Show();

            VisualUpdate();
        }
        void VisualUpdate()
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
            for (int i = 0; i < joints.Count; i++)
            {
                joints[i].localPosition = vertices[i];
                joints[i].localRotation = Quaternion.Euler(0, pipeRotations[i], 0);
            }

            // Assign positions, rotations, and scales to pipes
            for (int i = 0; i < pipes.Count; i++)
            {
                if (i < joints.Count)
                {
                    pipes[i].localPosition = joints[i].localPosition;
                    pipes[i].localRotation = Quaternion.Euler(0, pipeRotations[i], 0);
                    pipes[i].localScale = new Vector3(1, 1, pipeLengths[i]);
                }
                else
                {
                    pipes[i].localPosition = joints[i - 8].localPosition;
                    pipes[i].localRotation = Quaternion.Euler(90, pipeRotations[i - 8], 0);
                    pipes[i].localScale = new Vector3(1, 1, pipeLengths[i]);

                    if (i > 9)
                        pipes[i].localPosition = Vector3.Scale(pipes[i].localPosition, new Vector3(1, -1, -1));
                }
            }

            lightBlock.localScale = size;
            lastSize = size;

            float margin = 0.2f;
            col.size = size + new Vector3(margin, margin, margin);
        }
        void Update()
        {
            if (alternate && Application.isPlaying)
            {
                timer += Time.deltaTime;
                if (timer > alternateTimer)
                {
                    timer = 0f;
                    Toggle();
                }
            }

            if (lastSize == size)
                return;

            VisualUpdate();
        }

        public void Toggle()
        {
            hidden = !hidden;

            if (hidden)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            if (lightMaterial == null)
                return;

            col.isTrigger = false;
            
            float offset = -0.5f;
            DOTween.To(() => offset, x => offset = x, 1, 0.25f).SetEase(Ease.OutExpo).OnUpdate(() =>
            {
                lightMaterial.SetFloat("_Offset", offset);
            });

            if (Time.timeSinceLevelLoad > 0)
                RuntimeManager.PlayOneShot(appearSound, transform.position);
        }

        public void Hide()
        {
            if (lightMaterial == null)
                return;

            col.isTrigger = true;
            
            float offset = 1;
            DOTween.To(() => offset, x => offset = x, -0.5f, 0.25f).SetEase(Ease.OutExpo).OnUpdate(() =>
            {
                lightMaterial.SetFloat("_Offset", offset);
            });
        }
    }
}
