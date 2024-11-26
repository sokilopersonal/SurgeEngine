using System;
using System.Linq;
using UnityEngine;

namespace SurgeEngine.Code.GameDocuments
{
    [DefaultExecutionOrder(-10000)]
    public abstract class GameDocument<T> : MonoBehaviour where T : GameDocument<T>
    {
        private static T _instance;
        
        public static T Instance => _instance;
        
        [SerializeField] protected Document[] documents;

        private void Awake()
        {
            _instance = (T)this;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static Document GetDocument(string name)
        {
            var doc = _instance.documents.FirstOrDefault(x => x.name == name);
            if (doc == null)
            {
                throw new NullReferenceException($"Can't find the document {name}. Please make sure the document name is correct.");
            }

            return doc;
        }
    }
}