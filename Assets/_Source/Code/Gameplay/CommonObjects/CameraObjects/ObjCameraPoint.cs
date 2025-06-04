using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPoint : ObjCameraBase
    {
        [SerializeField] private PointPanData data;

        private void Update()
        {
            data.Forward = transform.forward;
        }

        public override void SetPan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.StateMachine.SetState<PointCameraPan>(allowSameState: true)?.SetData(data);
        }
        
        public override void RemovePan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.StateMachine.SetState<RestoreCameraPawn>();
        }
    }
}