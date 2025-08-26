using System;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Gameplay.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine.Code.Gameplay.CommonObjects.GoalRing
{
    public class GoalRing : ContactBase
    {
        [SerializeField] private EventReference goalRingSound;
        [SerializeField] private GoalRingScreen goalRingScreen;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private GameObject model;
        public GoalRingScreen CurrentGoalScreen { get; private set; }

        private bool _triggered;

        [Inject] private DiContainer _container;

        public override void Contact(Collider msg, CharacterBase context)
        {
            if (!_triggered)
            {
                _triggered = true;
                
                RuntimeManager.PlayOneShot(goalRingSound, transform.position);

                context.Input.playerInput.enabled = false;
                context.Flags.AddFlag(new AutorunFlag(default, true, 1f, 0f, 0.5f));

                var eventEmitter = GetComponent<StudioEventEmitter>();
                eventEmitter.Stop();
                
                CurrentGoalScreen = Instantiate(goalRingScreen);
                _container.InjectGameObject(CurrentGoalScreen.gameObject);
                
                var rt = new RenderTexture(2048, 2048, GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.None);
                rt.useMipMap = true;
                rt.autoGenerateMips = true;
                
                var playerRenderCameraObject = new GameObject("PlayerRenderCamera");
                var playerRenderCamera = playerRenderCameraObject.AddComponent<Camera>();
                playerRenderCamera.targetTexture = rt;
                playerRenderCamera.fieldOfView = 45f;
                playerRenderCamera.cullingMask = 1 << LayerMask.NameToLayer("Character");

                var hdData = playerRenderCameraObject.AddComponent<HDAdditionalCameraData>();
                hdData.backgroundColorHDR = new Color(0, 0, 0, 0);
                hdData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
                hdData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                hdData.customRenderingSettings = true;
                hdData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, false);
                hdData.stopNaNs = true;
                
                playerRenderCameraObject.transform.SetParent(context.transform, false);
                playerRenderCameraObject.transform.localPosition = new Vector3(0, -0.5f, 2.5f);
                playerRenderCameraObject.transform.localRotation = Quaternion.Euler(0, -180, 0);
                
                CurrentGoalScreen.SetRenderTexture(rt);
                
                var stage = Stage.Instance;
                
                CurrentGoalScreen.OnFlashEnd += () =>
                {
                    model.SetActive(false);

                    if (stage.BackgroundMusicEmitter != null)
                    {
                        stage.BackgroundMusicEmitter.AllowFadeout = true;
                        stage.BackgroundMusicEmitter.Stop();
                    }
                    
                    context.StateMachine.SetState<FStateGoal>().SetGoal(this);

                    var fixedPanData = new FixPanData();
                    var target = cameraTarget;
                    fixedPanData.position = target.position;
                    fixedPanData.target = target.rotation;
                    fixedPanData.fov = 45;
                    
                    context.Camera.StateMachine.ClearVolumes();
                    
                    context.Camera.StateMachine.SetState<FixedCameraPan>().SetData(fixedPanData);
                };
            }
            
            base.Contact(msg, context);
        }
    }
}