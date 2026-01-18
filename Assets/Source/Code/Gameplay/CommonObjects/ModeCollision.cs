using System;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public abstract class ModeCollision : StageObject
    {
        [SerializeField] protected bool isChangeCamera;
        [SerializeField] protected bool isEnabledFromBack = true;
        [SerializeField] protected bool isEnabledFromFront = true;
        
        protected SplineContainer container;
        protected virtual SplineTag SplineTagFilter => SplineTag.All;

        private CharacterBase _character;

        private void Awake()
        {
            if (container == null)
            {
                var closePath = FindClosestContainer();
                if (closePath)
                {
                    container = closePath;
                    
                    Debug.Log($"Path set for {name} (ID: {SetID}). Spline {container.name} with {container.tag} tag.");
                }
                else
                {
                    Debug.LogWarning($"No path found for {name} (ID: {SetID})");
                }
            }
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            if (!CheckFacing(context.transform.forward) || !container)
                return;

            _character = context;
        }

        public override void OnExit(Collider msg, CharacterBase context)
        {
            base.OnExit(msg, context);

            _character = null;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_character)
            {
                float dot = Vector3.Dot(transform.forward, _character.transform.forward);
                if (dot > 0)
                {
                    SetMode(_character);
                }
                else
                {
                    RemoveMode(_character);
                }
            }
        }

        protected abstract void SetMode(CharacterBase ctx);
        protected abstract void RemoveMode(CharacterBase ctx);

        protected bool CheckFacing(Vector3 dir)
        {
            if (isEnabledFromBack && isEnabledFromFront)
                return true;
            
            float dot = Vector3.Dot(transform.forward, dir);
            
            return isEnabledFromBack && dot > 0 || isEnabledFromFront && dot < 0;
        }

        private SplineContainer FindClosestContainer()
        {
            var containers = FindObjectsByType<SplineContainer>(FindObjectsSortMode.None);
            SplineContainer closest = null;
            float minDistance = float.MaxValue;

            foreach (var container in containers)
            {
                if (!IsSplineInFilter(container))
                    continue;
                
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
        
        private bool IsSplineInFilter(SplineContainer container)
        {
            var splineTag = GetSplineTag(container);
            return (SplineTagFilter & splineTag) != 0;
        }
        
        private SplineTag GetSplineTag(SplineContainer container)
        {
            if (container.CompareTag("SideView"))
                return SplineTag.SideView;

            if (container.CompareTag("Quickstep"))
                return SplineTag.Quickstep;

            if (container.CompareTag("DashPath"))
                return SplineTag.DashPath;

            return SplineTag.Default;
        }
    }

    public class ChangeModeData
    {
        public SplineData Spline { get; private set; }
        public bool IsCameraChange { get; private set; }
        public SplineTag Tag { get; private set; }

        protected ChangeModeData(SplineData spline, bool isCameraChange)
        {
            Spline = spline;
            IsCameraChange = isCameraChange;
            Tag = DetermineSplineTag(spline);
        }

        public void SetSpline(SplineData spline)
        {
            Spline = spline;
            Tag = DetermineSplineTag(spline);
        }

        private SplineTag DetermineSplineTag(SplineData spline)
        {
            if (spline == null)
                return SplineTag.Default;

            var container = spline.Container;
            if (container == null)
                return SplineTag.Default;
            
            var gameObject = container.gameObject;
            
            if (gameObject.CompareTag("SideView"))
                return SplineTag.SideView;

            if (gameObject.CompareTag("Quickstep"))
                return SplineTag.Quickstep;

            if (gameObject.CompareTag("DashPath"))
                return SplineTag.DashPath;

            return SplineTag.Default;
        }
    }

    [Flags]
    public enum SplineTag
    {
        Default = 0,
        SideView = 1,
        Quickstep = 2,
        DashPath = 4,
        All = Default | SideView | Quickstep | DashPath
    }
}