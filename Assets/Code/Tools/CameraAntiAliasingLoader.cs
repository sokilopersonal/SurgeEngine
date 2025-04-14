using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine.Code.Tools
{
    public class CameraAntiAliasingLoader : MonoBehaviour
    {
        private HDAdditionalCameraData _data;
        [Inject] private UserGraphics _graphics;

        private void Awake()
        {
            _data = GetComponent<HDAdditionalCameraData>();
        }

        private void OnEnable()
        {
            _graphics.OnDataApplied += OnDataApplied;
            _graphics.OnDataLoaded += OnDataApplied;
        }

        private void OnDisable()
        {
            _graphics.OnDataApplied -= OnDataApplied;
            _graphics.OnDataLoaded -= OnDataApplied;
        }

        private void OnDataApplied(GraphicsData obj)
        {
            _data.TAAQuality = (HDAdditionalCameraData.TAAQualityLevel)obj.antiAliasingQuality;
        }
    }
}