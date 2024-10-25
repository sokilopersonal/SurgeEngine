using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.GameDocuments
{
    [Serializable]
    public class Document
    {
        public string name;
        
        public ParameterGroup[] groups;
        public ParameterGroup GetGroup(string name)
        {
            var group = groups.FirstOrDefault(x => x.name == name);
            if (group == null)
            {
                throw new NullReferenceException($"Can't find the group {name}. Please make sure the group name is correct.");
            }
            
            return group;
        }
    }

    [Serializable]
    public class ParameterGroup
    {
        public string name;
        
        public Parameter[] parameters;
        public T GetParameter<T>(string name)
        {
            return (T)parameters.FirstOrDefault(x => x.name == name).GetValue();
        }
    }
    
    [Serializable]
    public class Parameter
    {
        public string name;
        public ParameterType type;
        
        [ShowIf("type", ParameterType.Float), SerializeField, AllowNesting] private float floatValue;
        [ShowIf("type", ParameterType.Int), SerializeField, AllowNesting] private int intValue;
        [ShowIf("type", ParameterType.Bool), SerializeField, AllowNesting] private bool boolValue;
        [ShowIf("type", ParameterType.AnimationCurve), SerializeField, AllowNesting] private AnimationCurve curveValue;
        [ShowIf("type", ParameterType.LayerMask), SerializeField, AllowNesting] private LayerMask maskValue;
        
        public object GetValue()
        {
            switch (type)
            {
                case ParameterType.Float:
                    return floatValue;
                case ParameterType.Int:
                    return intValue;
                case ParameterType.Bool:
                    return boolValue;
                case ParameterType.AnimationCurve:
                    return curveValue;
                case ParameterType.LayerMask:
                    return maskValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public enum ParameterType
    {
        Float,
        Int,
        Bool,
        AnimationCurve,
        LayerMask
    }
}