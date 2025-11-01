using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SplineContainer))]
    [ExecuteInEditMode]
    public class RailMeshGenerator : MonoBehaviour
    {
        [Header("Rail Mesh Settings")]
        [SerializeField, Range(0.1f, 5f)] private float railWidth = 0.5f;
        [SerializeField, Range(0.1f, 5f)] private float railHeight = 0.2f;
        [SerializeField, Range(32, 256)] private int splineResolution = 32;
        [SerializeField, Range(2, 6)] private int crossSectionSegments = 4;
        [SerializeField] private bool generateEndCaps = true;
        [SerializeField] private Material material;

        private SplineContainer _container;
        
        private Mesh _mesh;
        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector2[] _uvs;
        private int[] _triangles;
        
        private bool _initialized;
        private bool _meshGenerated;

        private void Awake()
        {
            _mesh = new Mesh
            {
                name = "<Generated Rail>Mesh"
            };

            if (CheckIfMeshGenerated())
            {
                InitializeComponents();
                GenerateRailMesh();
                _meshGenerated = true;
            }
        }

        private void OnEnable()
        {
            if (CheckIfMeshGenerated())
            {
                InitializeComponents();
                GenerateRailMesh();
                _meshGenerated = true;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                InitializeComponents();
                
                GenerateRailMesh();
                UpdateCollider();
            }
        }

        private void InitializeComponents()
        {
            if (_container == null)
            {
                _container = GetComponent<SplineContainer>();
            }

            if (_meshFilter == null)
            {
                _meshFilter = GetComponent<MeshFilter>();
            }

            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }
            
            if (_meshCollider == null)
            {
                _meshCollider = GetComponent<MeshCollider>();
            }

            _meshFilter.hideFlags = HideFlags.HideInInspector;
            _meshRenderer.hideFlags = HideFlags.HideInInspector;
            _meshCollider.hideFlags = HideFlags.HideInInspector;
            
            _meshRenderer.material = material;
        }

        private void UpdateCollider()
        {
            if (_meshCollider != null)
            {
                _meshCollider.sharedMesh = _mesh;
            }
        }

        private void GenerateRailMesh()
        {
            if (_container == null || _container.Spline.Count == 0)
                return;

            Spline spline = _container.Spline;
            float splineLength = spline.GetLength();

            int verticesPerSection = (crossSectionSegments * 4);
            int railVertices = verticesPerSection * (splineResolution + 1);

            int endCapVertices = generateEndCaps ? (verticesPerSection * 2 + 2) : 0;
            int totalVertices = railVertices + endCapVertices;
            
            int railTriangles = 6 * crossSectionSegments * 4 * splineResolution;
            int endCapTriangles = generateEndCaps ? (verticesPerSection * 2 * 3) : 0;
            int totalTriangles = railTriangles + endCapTriangles;

            _vertices = new Vector3[totalVertices];
            _normals = new Vector3[totalVertices];
            _uvs = new Vector2[totalVertices];
            _triangles = new int[totalTriangles];

            float halfWidth = railWidth / 2f;
            float halfHeight = railHeight / 2f;

            Vector3[] boxShape = new Vector3[crossSectionSegments * 4];

            for (int i = 0; i < crossSectionSegments; i++)
            {
                float t = i / (float)(crossSectionSegments - 1);
                boxShape[i] = new Vector3(Mathf.Lerp(-halfWidth, halfWidth, t), halfHeight, 0);
            }

            for (int i = 0; i < crossSectionSegments; i++)
            {
                float t = i / (float)(crossSectionSegments - 1);
                boxShape[crossSectionSegments + i] = new Vector3(halfWidth, Mathf.Lerp(halfHeight, -halfHeight, t), 0);
            }

            for (int i = 0; i < crossSectionSegments; i++)
            {
                float t = i / (float)(crossSectionSegments - 1);
                boxShape[2 * crossSectionSegments + i] = new Vector3(Mathf.Lerp(halfWidth, -halfWidth, t), -halfHeight, 0);
            }

            for (int i = 0; i < crossSectionSegments; i++)
            {
                float t = i / (float)(crossSectionSegments - 1);
                boxShape[3 * crossSectionSegments + i] = new Vector3(-halfWidth, Mathf.Lerp(-halfHeight, halfHeight, t), 0);
            }

            for (int i = 0; i <= splineResolution; i++)
            {
                float t = i / (float)splineResolution;

                Vector3 position = spline.EvaluatePosition(t);
                Vector3 tangent = spline.EvaluateTangent(t);
                Vector3 up = spline.EvaluateUpVector(t);

                Quaternion rotation = Quaternion.LookRotation(tangent, up);

                for (int j = 0; j < boxShape.Length; j++)
                {
                    int vertexIndex = i * verticesPerSection + j;

                    Vector3 offset = rotation * boxShape[j];
                    _vertices[vertexIndex] = position + offset;

                    _normals[vertexIndex] = offset.normalized;

                    _uvs[vertexIndex] = new Vector2(t * splineLength, j / (float)boxShape.Length);
                }
            }

            if (generateEndCaps)
            {
                GenerateEndCaps(spline, boxShape, railVertices);
            }

            int triangleIndex = 0;
            for (int i = 0; i < splineResolution; i++)
            {
                for (int j = 0; j < verticesPerSection; j++)
                {
                    int currentVertex = i * verticesPerSection + j;
                    int nextVertex = i * verticesPerSection + (j + 1) % verticesPerSection;
                    int currentVertexNextRow = (i + 1) * verticesPerSection + j;
                    int nextVertexNextRow = (i + 1) * verticesPerSection + (j + 1) % verticesPerSection;

                    _triangles[triangleIndex++] = currentVertex;
                    _triangles[triangleIndex++] = nextVertexNextRow;
                    _triangles[triangleIndex++] = nextVertex;

                    _triangles[triangleIndex++] = currentVertex;
                    _triangles[triangleIndex++] = currentVertexNextRow;
                    _triangles[triangleIndex++] = nextVertexNextRow;
                }
            }

            if (generateEndCaps)
            {
                GenerateEndCapTriangles(triangleIndex, railVertices, crossSectionSegments);
            }

            _mesh.Clear();
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.normals = _normals;
            _mesh.uv = _uvs;

            _mesh.RecalculateBounds();
            _meshFilter.mesh = _mesh;

            UpdateCollider();
        }

        private void GenerateEndCaps(Spline spline, Vector3[] boxShape, int railVertexOffset)
        {
            int verticesPerSection = boxShape.Length;

            Vector3 startPos = spline.EvaluatePosition(0f);
            Vector3 startTangent = spline.EvaluateTangent(0f);
            Vector3 startUp = spline.EvaluateUpVector(0f);
            Quaternion startRotation = Quaternion.LookRotation(startTangent, startUp);

            int startCenterIndex = railVertexOffset;
            _vertices[startCenterIndex] = startPos;
            _normals[startCenterIndex] = -startTangent;
            _uvs[startCenterIndex] = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < verticesPerSection; i++)
            {
                int vertexIndex = railVertexOffset + 1 + i;
                Vector3 offset = startRotation * boxShape[i];
                _vertices[vertexIndex] = startPos + offset;
                _normals[vertexIndex] = -startTangent;

                Vector2 localPos = new Vector2(boxShape[i].x, boxShape[i].y);
                _uvs[vertexIndex] = new Vector2(
                    (localPos.y / (railHeight / 2f) + 1f) * 0.5f,
                    (localPos.x / (railWidth / 2f) + 1f) * 0.5f
                );
            }

            Vector3 endPos = spline.EvaluatePosition(1f);
            Vector3 endTangent = spline.EvaluateTangent(1f);
            Vector3 endUp = spline.EvaluateUpVector(1f);
            Quaternion endRotation = Quaternion.LookRotation(endTangent, endUp);

            int endCenterIndex = railVertexOffset + 1 + verticesPerSection;
            _vertices[endCenterIndex] = endPos;
            _normals[endCenterIndex] = endTangent;
            _uvs[endCenterIndex] = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < verticesPerSection; i++)
            {
                int vertexIndex = railVertexOffset + 2 + verticesPerSection + i;
                Vector3 offset = endRotation * boxShape[i];
                _vertices[vertexIndex] = endPos + offset;
                _normals[vertexIndex] = endTangent;

                Vector2 localPos = new Vector2(boxShape[i].x, boxShape[i].y);
                _uvs[vertexIndex] = new Vector2(
                    (-localPos.y / (railHeight / 2f) + 1f) * 0.5f,
                    (localPos.x / (railWidth / 2f) + 1f) * 0.5f
                );
            }
        }

        private void GenerateEndCapTriangles(int startTriangleIndex, int railVertexOffset, int crossSectionSegments)
        {
            int verticesPerSection = crossSectionSegments * 4;
            int triangleIndex = startTriangleIndex;

            int startCenterIndex = railVertexOffset;
            for (int i = 0; i < verticesPerSection; i++)
            {
                int current = railVertexOffset + 1 + i;
                int next = railVertexOffset + 1 + (i + 1) % verticesPerSection;

                _triangles[triangleIndex++] = startCenterIndex;
                _triangles[triangleIndex++] = current;
                _triangles[triangleIndex++] = next;
            }

            int endCenterIndex = railVertexOffset + 1 + verticesPerSection;
            for (int i = 0; i < verticesPerSection; i++)
            {
                int current = railVertexOffset + 2 + verticesPerSection + i;
                int next = railVertexOffset + 2 + verticesPerSection + (i + 1) % verticesPerSection;

                _triangles[triangleIndex++] = endCenterIndex;
                _triangles[triangleIndex++] = next;
                _triangles[triangleIndex++] = current;
            }
        }
        
        private bool CheckIfMeshGenerated() => Application.isPlaying && !_meshGenerated;
    }
}