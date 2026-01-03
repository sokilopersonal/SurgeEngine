using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectSet
{
    [CreateAssetMenu(fileName = "AssetManagerData", menuName = "Surge Engine/Asset Manager Data")]
    public class AssetManagerData : ScriptableObject
    {
        [Serializable]
        public class PrefabData
        {
            public string guid;
            public string category;
        }

        public List<PrefabData> prefabs;
    }
}