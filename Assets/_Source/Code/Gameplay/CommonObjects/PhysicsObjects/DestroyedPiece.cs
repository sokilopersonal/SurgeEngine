using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class DestroyedPiece : MonoBehaviour
    {
        private static readonly int AlphaMult = Shader.PropertyToID("_AlphaMult");
        
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
                    if (mat.HasProperty(AlphaMult))
                    {
                        float initialAlpha = mat.GetFloat(AlphaMult);
                        StartCoroutine(FadeOut(mat, initialAlpha));
                    }
                }
            }

            foreach (var piece in rigidbodies)
            {
                piece.gameObject.layer = LayerMask.NameToLayer("BrokenPiece");
                piece.interpolation = RigidbodyInterpolation.Interpolate;
                piece.linearDamping = 1;
            }

            Destroy(gameObject, destroyTime + 0.25f);
        }

        private IEnumerator FadeOut(Material mat, float initialAlpha)
        {
            yield return new WaitForSeconds(destroyTime * 0.75f);

            float time = 0f;
            float duration = destroyTime * 0.25f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                mat.SetFloat("_AlphaMult", Mathf.Lerp(initialAlpha, 0f, t));
                yield return null;
            }

            mat.SetFloat("_AlphaMult", 0f);
        }

        public void ApplyExplosionForce(float force, Vector3 position, float radius)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
                rigidbodies[i].AddExplosionForce(force, position, radius);
        }

        public void ApplyDirectionForce(Vector3 direction, float force)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
                rigidbodies[i].AddForce(direction * force, ForceMode.VelocityChange);
        }
    }
}
