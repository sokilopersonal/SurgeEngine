using System.IO;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Editor
{
    public static class SceneViewScreenshot
    {
        [MenuItem("Tools/Take Scene View Screenshot", false, 0)]
        public static void Take()
        {
            var view = SceneView.lastActiveSceneView;
            
            int width = view.camera.pixelWidth;
            int height = view.camera.pixelHeight;
            
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            view.camera.Render();
            
            RenderTexture.active = view.camera.targetTexture;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            
            byte[] bytes = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            
            var path = EditorUtility.OpenFolderPanel("Save Screenshot To Directory", "", "Saved Screenshots");
            var pathFiles = Directory.GetFiles(path);
            if (path.Length != 0)
            {
                File.WriteAllBytes(path + $"/Screenshot_{pathFiles.Length}.png", bytes);
                Debug.Log("Screenshot Saved to: " + path);
            }
        }
    }
}