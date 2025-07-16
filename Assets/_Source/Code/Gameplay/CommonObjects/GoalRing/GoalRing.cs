using FMODUnity;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Gameplay.UI;
using UnityEngine;
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

        public override void Contact(Collider msg, ActorBase context)
        {
            if (!_triggered)
            {
                _triggered = true;
                
                RuntimeManager.PlayOneShot(goalRingSound, transform.position);

                context.Input.playerInput.enabled = false;
                context.Flags.AddFlag(new AutorunFlag(default, true, 1f, 0f, 0.25f));

                var eventEmitter = GetComponent<StudioEventEmitter>();
                eventEmitter.Stop();
                
                CurrentGoalScreen = Instantiate(goalRingScreen);
                _container.InjectGameObject(CurrentGoalScreen.gameObject);
                
                var stage = Stage.Instance;
                
                var rank = stage.Data.GoalRank;
                CurrentGoalScreen.SetGoalRank(rank);
                
                CurrentGoalScreen.OnFlashEnd += () =>
                {
                    model.SetActive(false);

                    if (stage.BackgroundMusicEmitter != null)
                    {
                        stage.BackgroundMusicEmitter.AllowFadeout = true;
                        stage.BackgroundMusicEmitter.Stop();
                    }
                    
                    context.StateMachine.SetState<FStateGoal>().SetGoal(this, rank);

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