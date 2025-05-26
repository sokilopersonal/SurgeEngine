using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    [ExecuteInEditMode]
    public class ActorModelRenderer : MonoBehaviour
    {
        [Space(10)]
        [InfoBox("Assign the mesh and materials per sub mesh. Project contains an editor preset with Sonic mesh and materials configured.")]
        [Space(10)]
        [SerializeField] private Mesh meshToRender;
        [SerializeField] private Material[] m_Materials; // Copying doesn't work :(
        
        private void Update()
        {
            if (Application.isPlaying || !Application.isEditor)
                return;
            
            if (meshToRender == null || m_Materials == null || m_Materials.Length == 0)
            {
                return;
            }

            int submeshCount = meshToRender.subMeshCount;
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position + Vector3.down, transform.rotation, transform.lossyScale);

            for (int subMeshIndex = 0; subMeshIndex < submeshCount; subMeshIndex++)
            {
                if (subMeshIndex < m_Materials.Length && m_Materials[subMeshIndex] != null)
                {
                    m_Materials[subMeshIndex].SetPass(0);
                    Graphics.DrawMesh(meshToRender, matrix, m_Materials[subMeshIndex], 0, null, subMeshIndex, null, 
                        true, 
                        true, 
                        true);
                }
            }
        }
    }
}