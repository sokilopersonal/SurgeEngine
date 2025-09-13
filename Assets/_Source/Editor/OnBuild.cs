using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SurgeEngine._Source.Editor
{
    public static class OnBuild
    {
        private const string ReadmeText = "Credits:"
                                          + "\nsokilo - Project lead" 
                                          + "\nloco - Shaders, programming help"
                                          + "\nSkyth (not related to the project) - Result screen music from Unleashed Recompiled Installer";

        [PostProcessBuild]
        private static void CreateReadmeFile(BuildTarget target, string pathToBuiltProject)
        {
            string directoryPath = Path.GetDirectoryName(pathToBuiltProject);
            string readmePath = Path.Combine(directoryPath, "README.txt");
            File.WriteAllText(readmePath, ReadmeText);
        }
    }
}