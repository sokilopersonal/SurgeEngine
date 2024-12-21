using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(SurgeEngine.Code.CommonObjects.ObjCameraPan))]
public class CameraPanEditor : Editor
{
    public VisualTreeAsset visualTree;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        visualTree.CloneTree(root);
        return root;
    }
}
