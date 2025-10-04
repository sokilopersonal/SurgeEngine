using JetBrains.Annotations;
using NaughtyAttributes;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public abstract class ModeCollision : StageObject
    {
        [SerializeField] protected SplineContainer path;
        [SerializeField] protected bool isChangeCamera;
        [SerializeField] protected bool isEnabledFromBack = true;
        [SerializeField] protected bool isEnabledFromFront = true;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            if (!CheckFacing(context.transform.forward))
                return;
            
            SetMode(context);
        }

        protected abstract void SetMode(CharacterBase ctx);

        protected bool CheckFacing(Vector3 dir)
        {
            if (isEnabledFromBack && isEnabledFromFront)
                return true;
            
            float dot = Vector3.Dot(transform.forward, dir);
            
            return isEnabledFromBack && dot > 0 || isEnabledFromFront && dot < 0;
        }
        
        [Button("Get Nearest Path"), UsedImplicitly]
        private void SetNearestPath()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Set Nearest Path");
#endif
            
            path = FindClosestContainer();
        }

        private SplineContainer FindClosestContainer()
        {
            var containers = FindObjectsByType<SplineContainer>(FindObjectsSortMode.None);
            SplineContainer closest = null;
            float minDistance = float.MaxValue;

            foreach (var container in containers)
            {
                SplineUtility.GetNearestPoint(container.Spline, container.transform.InverseTransformPoint(transform.position), 
                    out var nearestPoint, out _, 8, 4);

                var worldPoint = container.transform.TransformPoint(nearestPoint);
                float distance = Vector3.Distance(transform.position, worldPoint);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = container;
                }
            }

            return closest;
        }
    }

    public class ChangeModeData
    {
        public SplineData Spline { get; private set; }
        public bool IsCameraChange { get; private set; }
        
        public ChangeModeData(SplineData spline, bool isCameraChange)
        {
            Spline = spline;
            IsCameraChange = isCameraChange;
        }
    }
}