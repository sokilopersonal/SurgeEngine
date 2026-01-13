using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Player;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Debug = UnityEngine.Debug;

namespace SurgeEngine.Source.Editor.ObjectCustomGizmos
{
    public static class StageObjectGizmos
    {
        private static void DrawColliderGizmo(Component type, Color color)
        {
            var collider = type.GetComponent<BoxCollider>();
            if (collider == null)
                return;
                
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.color = color;
            Gizmos.DrawCube(collider.center, collider.size);
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(ChangeCameraVolume type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);

            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(0.15f, 1f, 0f, 0.1f));
            }
        }
        
        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(ModeCollision type, GizmoType gizmoType)
        {
            var matrix = Matrix4x4.TRS(type.transform.position, type.transform.rotation, Vector3.one * 0.75f);
            Gizmos.matrix = matrix;
            Gizmos.color = Color.clear;

            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/EditorMeshes/Arrow.fbx");
            if (mesh == null)
                return;
            
            Gizmos.DrawCube(mesh.bounds.center, mesh.bounds.size);
            
            GetEditorMaterial();
            Graphics.DrawMeshNow(mesh, matrix);

            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(0f, 0.35f, 1f, 0.1f));
            }
        }
        
        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(AutorunCollision type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);

            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(1f, 0.94f, 0.13f, 0.1f));
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(ObjCameraBase type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.color = Color.clear;
            
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/EditorMeshes/CameraMesh.fbx");
            if (mesh == null)
                return;
            
            Gizmos.DrawCube(mesh.bounds.center, mesh.bounds.size);
            
            GetEditorMaterial();
            Graphics.DrawMeshNow(mesh, type.transform.localToWorldMatrix);
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(JumpCollision type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(1f, 0f, 0f, 0.1f));
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(StumbleCollision type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(0f, 1f, 0f, 0.1f));
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(SlowdownCollision type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(0f, 0f, 1f, 0.1f));
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(SetRigidBody type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(1f, 1f, 0f, 0.1f));
            }
        }
        
        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(EventCollision type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(0.15f, 1f, 0f, 0.1f));
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(ParaloopVolume type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            
            if ((gizmoType & GizmoType.Selected) != 0)
            {
                DrawColliderGizmo(type, new Color(0f, 1f, 1f, 0.1f));
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(ReactionPlate type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            if (type.Type == ReactionPlateType.End)
            {
                Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.75f);
            }
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(SplineContainer type, GizmoType gizmoType)
        {
            Gizmos.matrix  = type.transform.localToWorldMatrix;
            Gizmos.color = Color.darkBlue;
            Gizmos.DrawSphere(Vector3.zero, 0.3f);
        }

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(Ring type, GizmoType gizmoType)
        {
            Gizmos.matrix = type.transform.localToWorldMatrix;
            Gizmos.color = Color.clear;
            Gizmos.DrawSphere(Vector3.zero, 0.4f);
        }

        // Call this method to use the material
        private static void GetEditorMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/EditorMeshes/EditorMeshMaterial.mat");
            if (mat == null)
                Debug.LogError("EditorMeshMaterial.mat not found");
            
            mat.SetPass(0);
        }
    }
}