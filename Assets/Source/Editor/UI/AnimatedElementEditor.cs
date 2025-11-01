using System;
using System.Collections.Generic;
using SurgeEngine.Source.Code.UI;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.UI
{
    [CustomPropertyDrawer(typeof(ElementAnimation), true)]
    public class ElementAnimationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var animation = property.managedReferenceValue as ElementAnimation;

            string displayName = animation?.GetDisplayName() ?? "Null Animation";

            var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, displayName, true);
            
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var iterator = property.Copy();
                var end = property.GetEndProperty();
                
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (SerializedProperty.EqualContents(iterator, end))
                            break;
                            
                        var fieldRect = new Rect(position.x, currentY, position.width, EditorGUI.GetPropertyHeight(iterator));
                        EditorGUI.PropertyField(fieldRect, iterator, true);
                        currentY += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                    }
                    while (iterator.NextVisible(false));
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;
            
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            var iterator = property.Copy();
            var end = property.GetEndProperty();
            
            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        break;
                        
                    height += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                }
                while (iterator.NextVisible(false));
            }
            
            return height;
        }
    }

    [CustomEditor(typeof(AnimatedElement))]
    public class AnimatedElementEditor : UnityEditor.Editor
    {
        private SerializedProperty animationsProperty;
        
        private void OnEnable()
        {
            animationsProperty = serializedObject.FindProperty("animations");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "animations");
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);

            for (int i = 0; i < animationsProperty.arraySize; i++)
            {
                var animationProperty = animationsProperty.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(animationProperty);
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    animationsProperty.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Animation"))
            {
                ShowAddAnimationMenu();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void ShowAddAnimationMenu()
        {
            var menu = new GenericMenu();

            var animationTypes = GetAllAnimationTypes();
            
            foreach (var type in animationTypes)
            {
                var instance = Activator.CreateInstance(type) as ElementAnimation;
                var displayName = instance?.GetDisplayName() ?? type.Name;
                
                menu.AddItem(new GUIContent(displayName), false, () => AddAnimation(type));
            }
            
            menu.ShowAsContext();
        }
        
        private void AddAnimation(Type animationType)
        {
            var newAnimation = Activator.CreateInstance(animationType) as ElementAnimation;
            newAnimation.SetData(target as AnimatedElement);
            
            animationsProperty.arraySize++;
            var newElement = animationsProperty.GetArrayElementAtIndex(animationsProperty.arraySize - 1);
            newElement.managedReferenceValue = newAnimation;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private Type[] GetAllAnimationTypes()
        {
            var baseType = typeof(ElementAnimation);
            var types = new List<Type>();
            
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                        {
                            types.Add(type);
                        }
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException)
                { }
            }
            
            return types.ToArray();
        }
    }
}