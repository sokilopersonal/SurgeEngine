using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Rendering
{
    [ExecuteInEditMode]
    public class CollisionRender : MonoBehaviour
    {
        private List<MeshRenderer> _renderers;

        private void Update()
        {
            int count = transform.childCount;
            if (_renderers == null || _renderers.Count != count)
            {
                _renderers = new List<MeshRenderer>(count);
                for (int i = 0; i < count; i++)
                {
                    _renderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
                }
            }
        }

        [Button, UsedImplicitly]
        private void ToggleDraw()
        {
            foreach (var colRenderer in _renderers)
            {
                colRenderer.enabled = !colRenderer.enabled;
            }
        }
    }
}