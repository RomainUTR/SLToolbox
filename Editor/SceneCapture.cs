using UnityEngine;
using UnityEditor;
using System.IO;

namespace RomainUTR.SLToolbox.Editor
{
    public static class ScreenshotTool
    {
        [MenuItem("Tools/SL Toolbox/Take High-Res Screenshot %#k")]
        public static void TakeScreenshot()
        {
            string folderPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Screenshots");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"HighRes_Screen_{timestamp}.png";
            string fullPath = Path.Combine(folderPath, fileName);

            ScreenCapture.CaptureScreenshot(fullPath, 2);

            Debug.Log($"<color=green>High-Res Screenshot Captured :</color> {fullPath}");

            EditorApplication.delayCall += () =>
            {
                if (File.Exists(fullPath))
                {
                    EditorUtility.RevealInFinder(fullPath);
                }
            };
        }
    }
}