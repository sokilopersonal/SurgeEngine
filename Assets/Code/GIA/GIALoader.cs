using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace SurgeEngine
{
    public class GIALoader : MonoBehaviour
    {
        public List<Texture2D> lightmapTextures = new List<Texture2D>();
        public List<Renderer> lightmapRenderers;
        [Button("Load GIA")]
        void LoadGIA()
        {
            LightmapData[] lightmaps = new LightmapData[lightmapTextures.Count];

            for (int i = 0; i < lightmaps.Length; i++)
            {
                lightmaps[i] = new LightmapData();
                lightmaps[i].lightmapColor = lightmapTextures[i];
                foreach (Renderer meshRenderer in lightmapRenderers)
                {
                    if (lightmapTextures[i].name.Contains(meshRenderer.name))
                    {
                        meshRenderer.lightmapIndex = i;
                    }
                }
            }

            LightmapSettings.lightmaps = lightmaps;
        }

        private void Start()
        {
            LoadGIA();
        }
    }
}
