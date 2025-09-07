using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class DestroyedPiece : MonoBehaviour
    {
        [SerializeField] private float destroyTime = 15f;
        
        [SerializeField] private Rigidbody[] rigidbodies;
        [SerializeField] private Renderer[] meshes;

        private void Awake()
        {
            foreach (Renderer mesh in meshes)
            {
                Material[] originalMaterials = mesh.sharedMaterials;
                List<Material> newMaterials = new List<Material>();
                
                foreach (Material mat in originalMaterials)
                {
                    newMaterials.Add(new Material(mat));
                }
                
                mesh.sharedMaterials = newMaterials.ToArray();

                foreach (Material mat in mesh.sharedMaterials)
                {
                    float initialAlpha = mat.GetFloat("_AlphaMult");
                    float t = 0f;
                    DOTween.To(() => t, x => t = x, 1, destroyTime * 0.25f).SetDelay(destroyTime * 0.75f).OnUpdate(() =>
                    {
                        mat.SetFloat("_AlphaMult", Mathf.Lerp(initialAlpha, 0f, t));
                    });
                }
            }

            foreach (var piece in rigidbodies)
            {
                piece.gameObject.layer = LayerMask.NameToLayer("BrokenPiece");
                piece.interpolation = RigidbodyInterpolation.Interpolate;
            }

            Destroy(gameObject, destroyTime + 0.25f);
        }

        public void ApplyExplosionForce(float force, Vector3 position, float radius)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].AddExplosionForce(force, position, radius);
            }
        }

        public void ApplyDirectionForce(Vector3 direction, float force)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].AddForce(direction * force, ForceMode.VelocityChange);
            }
        }
    }
}