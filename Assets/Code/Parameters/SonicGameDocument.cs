using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.GameDocuments
{
    public class SonicGameDocument : GameDocument<SonicGameDocument>
    {
        public static string PhysicsGroup = "BasePhysics";
        public static string HomingGroup = "Homing";
        public static string CastGroup = "Cast";
        
        [Button("Generate Param File")]
        public void Generate()
        {
            var docs = documents;
    
            foreach (var document in docs)
            {
                var groups = document.groups;
                
                var directoryPath = "Assets/Documents";
                var filePath = Path.Combine(directoryPath, $"{document.name}GameDocumentParams.cs");
                Directory.CreateDirectory(directoryPath);
        
                string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
                var parameters = new List<string>();

                for (int i = 0; i < groups.Length; i++)
                {
                    var group = groups[i];
                    var groupParams = group.parameters;

                    for (int j = 0; j < groupParams.Length; j++)
                    {
                        var param = groupParams[j];
                        var fieldLine = $"        public static string {group.name}_{param.name} = \"{param.name}\";";
                        
                        if (!fileContent.Contains(fieldLine))
                        {
                            parameters.Add(fieldLine);
                        }
                    }
                }
                
                if (fileContent == "")
                {
                    fileContent = $"namespace SurgeEngine.Code.GameDocuments\n{{\n    public static class {document.name}GameDocumentParams\n    {{\n{string.Join("\n", parameters)}\n    }}\n}}";
                }
                else if (parameters.Count > 0)
                {
                    var insertIndex = fileContent.LastIndexOf("    }");
                    fileContent = fileContent.Insert(insertIndex, string.Join("\n", parameters) + "\n");
                }
                
                File.WriteAllText(filePath, fileContent);
            }
        }
    }
}