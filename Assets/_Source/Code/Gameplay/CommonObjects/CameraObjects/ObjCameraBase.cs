
#if UNITY_EDITOR
using UnityEditor;
#endif

using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public abstract class ObjCameraBase : MonoBehaviour
    {
        public abstract void SetPan(ActorBase ctx);
        public abstract void RemovePan(ActorBase context);
    }
    
    public abstract class ObjCameraBase<TState,TData> : ObjCameraBase
        where TState: CameraState, IPanState<TData>
        where TData : PanData
    {
        [SerializeField] protected TData data;

        public override void SetPan(ActorBase ctx)
        {
            var st = ctx.Camera.StateMachine.GetState<TState>();
            st?.SetData(data);
            ctx.Camera.StateMachine.SetState<TState>(allowSameState: true);
        }

        public override void RemovePan(ActorBase ctx)
        {
            ctx.Camera.StateMachine.SetState<NewModernState>();
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            string tName = name;
            float distance = Vector3.Distance(transform.position, Camera.current.transform.position);
            if (distance < 35f)
            {
                Handles.Label(transform.position + Vector3.up * 0.75f, tName, style);
            }
#endif
        }
    }
}