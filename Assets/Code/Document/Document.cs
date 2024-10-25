using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Document
{
    [Serializable]
    public class Document
    {
        public string name;
        
        public ParameterGroup[] groups;
        public ParameterGroup GetGroup(string name)
        {
            return groups.FirstOrDefault(x => x.name == name);
        }
    }

    [Serializable]
    public class ParameterGroup
    {
        public string name;
        
        public Parameter[] parameters;
        public Parameter GetParameter(string name)
        {
            return parameters.FirstOrDefault(x => x.name == name);
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
        AnimationCurve
    }
}