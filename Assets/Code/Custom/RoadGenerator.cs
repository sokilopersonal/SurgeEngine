using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Custom
{
    [ExecuteAlways]
    [RequireComponent(typeof(SplineContainer))]
    public class RoadGenerator : MonoBehaviour
    {
        [Header("Road Settings")]
        [SerializeField] private float roadWidth = 2f;
        [SerializeField] private float roadThickness = 0.2f;
        [SerializeField] private Material roadMaterial;

        [Header("Mesh Settings")]
        [SerializeField] private int subdivisions = 10;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private void OnEnable()
        {
            EnsureComponents();
            GenerateRoad();
        }

        private void Update()
        {
            GenerateRoad();
        }

        private void EnsureComponents()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            if (roadMaterial != null)
            {
                meshRenderer.sharedMaterial = roadMaterial;
            }
        
            if (meshCollider == null)
            {
                if (gameObject.TryGetComponent(out MeshCollider col))
                {
                    meshCollider = col;
                }
            }
        }

        private void GenerateRoad()
        {
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            if (splineContainer == null || splineContainer.Spline == null || splineContainer.Spline.Count < 2)
            {
                UnityEngine.Debug.LogWarning("Spline must have at least two points.");
                return;
            }

            Spline spline = splineContainer.Spline;

            Mesh roadMesh = new Mesh();

            Vector3[] vertices = new Vector3[(subdivisions + 1) * 8];
            int[] triangles = new int[subdivisions * 6 * 4];
            Vector2[] uvs = new Vector2[vertices.Length];

            int vertIndex = 0;
            int triIndex = 0;

            for (int i = 0; i <= subdivisions; i++)
            {
                float t = i / (float)subdivisions;
                Vector3 position = spline.EvaluatePosition(t);
                Vector3 tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;
                Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;

                Vector3 leftTop = position - normal * roadWidth / 2;
                Vector3 rightTop = position + normal * roadWidth / 2;
                Vector3 leftBottom = leftTop - Vector3.up * roadThickness;
                Vector3 rightBottom = rightTop - Vector3.up * roadThickness;

                // Top vertices
                vertices[vertIndex] = leftTop;
                vertices[vertIndex + 1] = rightTop;

                // Bottom vertices
                vertices[vertIndex + 2] = leftBottom;
                vertices[vertIndex + 3] = rightBottom;

                // Side vertices (left)
                vertices[vertIndex + 4] = leftTop;
                vertices[vertIndex + 5] = leftBottom;

                // Side vertices (right)
                vertices[vertIndex + 6] = rightTop;
                vertices[vertIndex + 7] = rightBottom;

                // UVs (adjust as needed for your material)
                uvs[vertIndex] = new Vector2(0, t);
                uvs[vertIndex + 1] = new Vector2(1, t);
                uvs[vertIndex + 2] = new Vector2(0, t);
                uvs[vertIndex + 3] = new Vector2(1, t);

                if (i < subdivisions)
                {
                    // Top surface
                    triangles[triIndex] = vertIndex;
                    triangles[triIndex + 1] = vertIndex + 1;
                    triangles[triIndex + 2] = vertIndex + 8;

                    triangles[triIndex + 3] = vertIndex + 8;
                    triangles[triIndex + 4] = vertIndex + 1;
                    triangles[triIndex + 5] = vertIndex + 9;

                    // Bottom surface
                    triangles[triIndex + 6] = vertIndex + 2;
                    triangles[triIndex + 7] = vertIndex + 10;
                    triangles[triIndex + 8] = vertIndex + 3;

                    triangles[triIndex + 9] = vertIndex + 3;
                    triangles[triIndex + 10] = vertIndex + 10;
                    triangles[triIndex + 11] = vertIndex + 11;

                    // Left side
                    triangles[triIndex + 12] = vertIndex + 4;
                    triangles[triIndex + 13] = vertIndex + 12;
                    triangles[triIndex + 14] = vertIndex + 5;

                    triangles[triIndex + 15] = vertIndex + 5;
                    triangles[triIndex + 16] = vertIndex + 12;
                    triangles[triIndex + 17] = vertIndex + 13;

                    // Right side
                    triangles[triIndex + 18] = vertIndex + 6;
                    triangles[triIndex + 19] = vertIndex + 7;
                    triangles[triIndex + 20] = vertIndex + 14;

                    triangles[triIndex + 21] = vertIndex + 14;
                    triangles[triIndex + 22] = vertIndex + 7;
                    triangles[triIndex + 23] = vertIndex + 15;

                    triIndex += 24;
                }

                vertIndex += 8;
            }

            roadMesh.vertices = vertices;
            roadMesh.triangles = triangles;
            roadMesh.uv = uvs;
            roadMesh.RecalculateNormals();

            meshFilter.sharedMesh = roadMesh;
            meshCollider.sharedMesh = roadMesh;
        }
    }
}
