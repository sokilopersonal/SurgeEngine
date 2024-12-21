using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(SurgeEngine.Code.CommonObjects.ObjVerticalCameraPan))]
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
