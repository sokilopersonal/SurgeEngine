using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Gameplay.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.GoalRing
{
    public class GoalRing : StageObject
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
                context.Flags.AddFlag(new AutorunFlag(default, true, 1f, 0f, 0.2f));

                var eventEmitter = GetComponent<StudioEventEmitter>();
                eventEmitter.Stop();
                
                CurrentGoalScreen = Instantiate(goalRingScreen);
                _container.InjectGameObject(CurrentGoalScreen.gameObject);
                
                var stage = Stage.Instance;
                
                CurrentGoalScreen.OnFlashEnd += () =>
                {
                    context.Animation.enabled = false;

                    var rt = new RenderTexture(1512, 1512, GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.None);

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

                    context.Camera.GetCamera().enabled = false;
                    
                    model.SetActive(false);

                    if (stage.BackgroundMusicEmitter != null)
                    {
                        stage.BackgroundMusicEmitter.AllowFadeout = true;
                        stage.BackgroundMusicEmitter.Stop();
                    }
                    
                    context.StateMachine.SetState<FStateGoal>().SetGoal(this);
                };
            }
            
            base.Contact(msg, context);
        }
    }
}