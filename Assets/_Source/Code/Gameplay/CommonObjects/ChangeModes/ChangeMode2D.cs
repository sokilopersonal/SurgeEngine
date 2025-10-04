using JetBrains.Annotations;
using NaughtyAttributes;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private SplineContainer path;
        [SerializeField] private float pathEaseTime = 1f;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (!CheckFacing(context.transform.forward))
                return;
            
            var kinematics = context.Kinematics;
            kinematics.Set2DPath(kinematics.Path2D == null
                ? new ChangeMode2DData(new SplineData(path, context.transform.position), isChangeCamera, pathEaseTime)
                : null);
        }

        [Button("Get Nearest Path"), UsedImplicitly]
        private void SetNearestPath()
        {
            Undo.RecordObject(this, "Set Nearest Path");
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

    public class ChangeMode2DData : ChangeModeData
    {
        public float PathEaseTime { get; private set; }
        public float CurrentEaseTime { get; set; }

        public ChangeMode2DData(SplineData spline, bool isCameraChange, float pathEaseTime) : base(spline, isCameraChange)
        {
            PathEaseTime = pathEaseTime;
        }
    }
}