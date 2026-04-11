using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.CameraSystem;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public abstract class ObjCameraBase : StageObject
    {
        public abstract void SetPan(CharacterBase ctx, CameraEaseData? easeOverride = null);
        public abstract void RemovePan(CharacterBase context);
    }
    
    public abstract class ObjCameraBase<TState,TData> : ObjCameraBase, IHE1Importable
        where TState: CameraState, IPanState<TData>
        where TData : PanData
    {
        [SerializeField] protected TData data;

        public override void SetPan(CharacterBase ctx, CameraEaseData? easeOverride = null)
        {
            var stateMachine = ctx.Camera.StateMachine;
            var st = stateMachine.GetState<TState>();
            st?.SetData(data);
            
            stateMachine.EaseData = easeOverride ?? CameraEaseData.FromPan(data);
            stateMachine.IsExiting = false;
            stateMachine.Blending.Reset();

            stateMachine.SetState<TState>(allowSameState: true);
        }

        public override void RemovePan(CharacterBase ctx)
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

        public virtual void ImportSetData(string objectName, XElement elem)
        {
            data.easeTimeEnter = HE1Helper.GetFloat(elem, "Ease_Time_Enter");
            data.easeTimeLeave = HE1Helper.GetFloat(elem, "Ease_Time_Leave");
            data.fov = HE1Helper.GetFloat(elem, "Fovy");
            data.isCollision = HE1Helper.GetBool(elem, "IsCollision");
        }
    }
}