using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Document
{
    public class GameDocument : MonoBehaviour
    {
        private static GameDocument _instance;
        
        public static GameDocument Instance => _instance;
        
        public Document[] documents;

        private void Awake()
        {
            _instance = this;
            
            var doc = GetDocument("Sonic");
            var group = doc.GetGroup("Physics");
            var param = group.GetParameter("TopSpeed");
            var value = param.GetValue();

            Debug.Log(value);
        }

        public static Document GetDocument(string name)
        {
            return _instance.documents.FirstOrDefault(x => x.name == name);
        }
    }
}