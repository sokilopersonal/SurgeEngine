using DG.Tweening;
using NUnit.Framework;
using SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine
{
    public struct AfterImageTrail
    {
        public GameObject gameObject;
        public Transform transform;
        public MeshRenderer renderer;
        public MeshFilter filter;
    }
    public class MeshTrail : MonoBehaviour
    {
        [SerializeField] private float delay = 0.5f;
        [SerializeField] private float startAlpha = 0.5f;
        [SerializeField] private float fadeTime = 0.25f;
        [SerializeField] private int amount = 5;
        [SerializeField] private Sonic sonic;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private List<Material> materials = new List<Material>();

        private List<AfterImageTrail> _trails = new List<AfterImageTrail>();

        private int _index = -1;
        private float _timer;

        void Start()
        {
            _timer = delay;
            
            for (int i = 0; i < amount; i++)
            {
                AfterImageTrail afterImage = new AfterImageTrail();
                afterImage.gameObject = new GameObject();
                afterImage.transform = afterImage.gameObject.transform;
                afterImage.renderer = afterImage.gameObject.AddComponent<MeshRenderer>();
                afterImage.filter = afterImage.gameObject.AddComponent<MeshFilter>();

                afterImage.renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                List<Material> mats = new List<Material>();

                for (int v = 0; v < materials.Count; v++)
                {
                    mats.Add(new Material(materials[v]));
                }

                afterImage.renderer.SetSharedMaterials(mats);

                afterImage.gameObject.SetActive(false);

                _trails.Add(afterImage);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (sonic.Kinematics.Speed > 5.0f)
            {
                _timer -= Time.deltaTime;
            }

            if (_timer <= 0.0f)
            {
                _timer = delay;
                _index = _index + 1 >= amount ? 0 : _index + 1;

                AfterImageTrail trail = _trails[_index];

                meshRenderer.BakeMesh(trail.filter.mesh);

                trail.transform.SetPositionAndRotation(meshRenderer.transform.position, meshRenderer.transform.rotation);

                AnimateTrail(trail);
            }
        }

        void AnimateTrail(AfterImageTrail trail)
        {
            trail.gameObject.SetActive(true);
            foreach (Material mat in trail.renderer.sharedMaterials)
            {
                mat.DOKill();
                mat.SetFloat("_Alpha", startAlpha);
                mat.DOFloat(0f, "_Alpha", fadeTime).OnComplete(() => trail.gameObject.SetActive(false));
            }
        }
    }
}
