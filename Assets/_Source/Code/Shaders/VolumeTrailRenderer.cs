using System;
using System.Collections;
using System.Collections.Generic;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Shaders
{
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class VolumeTrailRenderer : MonoBehaviour 
	{
		[Serializable]
		public class TubeVertex 
		{
			public float timeCreated;
			public Vector3 point = Vector3.zero;
			public float radius = 1.0f;

			public TubeVertex() 
			{
			}

			public TubeVertex(Vector3 pt, float r, Color c) 
			{
				point = pt;
				radius = r;
			}
		}

		private bool _emit = true;
		private float _emitTime;
		private readonly bool _autoDestruct = false;
		[SerializeField] private Material material;
		[SerializeField] private Rigidbody target;
		[SerializeField] private Vector3 offset;
		[SerializeField] private int crossSegments = 18;
		[SerializeField] private AnimationCurve radius;
		[SerializeField] private Gradient colorOverLifeTime;
		[SerializeField] private float lifeTime;
		[SerializeField] private float fadeSpeed = 2f;

		private List<TubeVertex> _vertices;
		private Color[] _vertexColors;
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		private Vector3[] _crossPoints;
		private int _lastCrossSegments;
		private Renderer _renderer;
		private bool _fadeOut;
		private float _fadeBias;

		private void Start() 
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshRenderer.material = material;
        
			_meshFilter = GetComponent<MeshFilter>();
			_renderer = GetComponent<Renderer>();

			_vertices = new List<TubeVertex>();
			
			_meshFilter.hideFlags = HideFlags.HideInInspector;
			_meshRenderer.hideFlags = HideFlags.HideInInspector;

			_fadeOut = true;
			_fadeBias = 0f;
		}

		private void Update() 
		{
			if (_fadeOut) _fadeBias -= Time.deltaTime * fadeSpeed;
			_fadeBias = Mathf.Clamp(_fadeBias, 0, lifeTime);

			if (Mathf.Approximately(_fadeBias, 0f) && _emit)
			{
				_emit = false;
				_vertices.Clear();
			}
		}

		private void LateUpdate() 
		{
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			
			_renderer.enabled = _vertices.Count > 2;

			if (_emit && _emitTime > 0) 
			{
				_emitTime -= Time.deltaTime;
			}
			else if (_emit && _emitTime <= 0)
			{
				_fadeOut = true;
			}
			
			if (!_emit && _vertices.Count == 0 && _autoDestruct) 
			{
				Destroy(gameObject);
			}

			if (_emit) 
			{
				TubeVertex p = new TubeVertex
				{
					point = target.transform.position + target.transform.TransformDirection(offset),
					timeCreated = Time.time
				};
				_vertices.Add(p);
			}

			ArrayList remove = new ArrayList();
			foreach (TubeVertex p in _vertices) 
			{
				if (Time.time - p.timeCreated > lifeTime) 
				{
					remove.Add(p);
				}
			}

			foreach (TubeVertex p in remove) 
			{
				_vertices.Remove(p);
			}
		
			remove.Clear();

			if (_vertices.Count > 1) 
			{
				for (int k = 0; k < _vertices.Count; k++) 
				{
					_vertices[k].radius = radius.Evaluate((_vertices.Count - 1 - k) / (float)(_vertices.Count - 1));
				}

				if (crossSegments != _lastCrossSegments) 
				{
					_crossPoints = new Vector3[crossSegments];
					float theta = 2.0f * Mathf.PI / crossSegments;
					for (int c = 0; c < crossSegments; c++) 
					{
						_crossPoints[c] = new Vector3(Mathf.Cos(theta * c), Mathf.Sin(theta * c), 0);
					}
					_lastCrossSegments = crossSegments;
				}

				Vector3[] meshVertices = new Vector3[_vertices.Count * crossSegments];
				Vector2[] uvs = new Vector2[_vertices.Count * crossSegments];
				Color[] colors = new Color[_vertices.Count * crossSegments];
				int[] tris = new int[_vertices.Count * crossSegments * 6];
				int[] lastVertices = new int[crossSegments];
				int[] theseVertices = new int[crossSegments];
				Quaternion rotation = Quaternion.identity;

				for (int p = 0; p < _vertices.Count; p++) 
				{
					if (p < _vertices.Count - 1) 
					{
						rotation = Quaternion.FromToRotation(Vector3.forward, _vertices[p + 1].point - _vertices[p].point);
					}

					for (int c = 0; c < crossSegments; c++) 
					{
						int vertexIndex = p * crossSegments + c;
						meshVertices[vertexIndex] = _vertices[p].point + rotation * _crossPoints[c] * _vertices[p].radius;
						uvs[vertexIndex] = new Vector2((0f + c) / crossSegments, (1f + p) / _vertices.Count);

						float overlifetime = (_vertices.Count - 1 - p) / (float)(_vertices.Count - 1);

						Color color = colorOverLifeTime.Evaluate(overlifetime) * _fadeBias;
						colors[vertexIndex] = color;

						lastVertices[c] = theseVertices[c];
						theseVertices[c] = p * crossSegments + c;	
					}
				
					if (p > 0) 
					{
						for (int c = 0; c < crossSegments; c++) 
						{
							int start = (p * crossSegments + c) * 6;
							tris[start] = lastVertices[c];
							tris[start + 1] = lastVertices[(c + 1) % crossSegments];
							tris[start + 2] = theseVertices[c];
							tris[start + 3] = tris[start + 2];
							tris[start + 4] = tris[start + 1];
							tris[start + 5] = theseVertices[(c + 1) % crossSegments];
						}
					}
				}

				Mesh mesh = GetComponent<MeshFilter>().mesh;
				if (!mesh) 
				{
					mesh = new Mesh();
				}
				mesh.Clear();
				mesh.vertices = meshVertices;
				mesh.triangles = tris;
				mesh.colors = colors;

				mesh.RecalculateNormals();
				mesh.uv = uvs;
			}
		}

		public void Emit(float lifetime = 0.5f) 
		{
			_emit = true;
			lifeTime = lifetime;
			_fadeBias = lifetime;
			_fadeOut = false;
		}

		public void Clear(bool instant = false)
		{
			if (instant)
			{
				_emit = false;
				_vertices.Clear();
			}
			else
			{
				_fadeOut = true;
			}
		}
	}
}