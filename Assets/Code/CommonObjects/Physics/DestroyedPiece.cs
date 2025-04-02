using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace SurgeEngine.Code.CommonObjects
{
    public class DestroyedPiece : MonoBehaviour
    {
        [SerializeField] private float destroyTime = 15f;
        
        [SerializeField] private Rigidbody[] rigidbodies;
        [SerializeField] private Renderer[] meshes;
        
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

            Destroy(gameObject, destroyTime + 0.25f);
        }
    }
}