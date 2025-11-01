using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SurgeEngine.Source.Code.Rendering
{
    [System.Serializable]
    public class GrassData
    {
        [System.Serializable]
        public struct SerializableGrassInstance
        {
            public float posX;
            public float posY;
            public float posZ;
            public float rotation;
            public float height;
            public float width;
            
            public SerializableGrassInstance(GrassRenderer.GrassInstance instance)
            {
                posX = instance.position.x;
                posY = instance.position.y;
                posZ = instance.position.z;
                rotation = instance.rotation;
                height = instance.height;
                width = instance.width;
            }
            
            public GrassRenderer.GrassInstance ToGrassInstance()
            {
                return new GrassRenderer.GrassInstance
                {
                    position = new Vector3(posX, posY, posZ),
                    rotation = rotation,
                    height = height,
                    width = width
                };
            }
        }
        
        public List<SerializableGrassInstance> instances = new List<SerializableGrassInstance>();
        
        public GrassData(List<GrassRenderer.GrassInstance> grassInstances)
        {
            foreach (var instance in grassInstances)
            {
                instances.Add(new SerializableGrassInstance(instance));
            }
        }
        
        public List<GrassRenderer.GrassInstance> ToGrassInstances()
        {
            List<GrassRenderer.GrassInstance> result = new List<GrassRenderer.GrassInstance>();
            
            foreach (var serializableInstance in instances)
            {
                result.Add(serializableInstance.ToGrassInstance());
            }
            
            return result;
        }
        
        public void SaveToFile(string filename)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            BinaryFormatter formatter = new BinaryFormatter();
            
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(stream, this);
            }
            
            Debug.Log($"Grass data saved to {path}");
        }
        
        public static GrassData LoadFromFile(string filename)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    return formatter.Deserialize(stream) as GrassData;
                }
            }
            
            Debug.LogWarning($"Grass data file not found at {path}");
            return null;
        }
    }
}
