using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ObjVerticalCameraPan))]
public class CameraVerticalPanEditor : Editor
{
    public VisualTreeAsset visualTree;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        visualTree.CloneTree(root);
        return root;
    }
}
