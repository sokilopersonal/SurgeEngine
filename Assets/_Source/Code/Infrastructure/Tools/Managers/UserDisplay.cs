using System;
using SurgeEngine.Code.Infrastructure.Tools.Services;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers
{
    public class UserDisplay : JsonStorageService<UserDisplaySettings>, ILateDisposable
    {
        public UpscalerChanger UpscalerChanger { get; }
        
        private HDAdditionalCameraData _hdCameraData;

        public UserDisplay()
        {
            UpscalerChanger = new UpscalerChanger();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LateDispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg1 != LoadSceneMode.Additive)
            {
                SetupCamera();

                Apply();
            }
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera != null)
            {
                _hdCameraData = camera.GetComponent<HDAdditionalCameraData>();
                _hdCameraData.customRenderingSettings = true;
                
                UpscalerChanger.SetCameraAndData(camera, _hdCameraData);
            }
        }

        private void Apply()
        {
            
        }
    }

    [Serializable]
    public class UserDisplaySettings
    {
        public Upscaler Upscaler;
        public UpscalingMode UpscalerQuality;
    }
}