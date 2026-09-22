using UnityEngine;
using UnityEditor;

namespace FBXEditor
{
    /// <summary>
    /// Context menu for quick FBX fixes in the Project Window
    /// </summary>
    public static class FBXContextMenu
    {
        [MenuItem("Assets/FBX Editor/Quick Fix - Bake Axis Conversion", false, 100)]
        private static void QuickFixBakeAxis()
        {
            ProcessSelectedFBX(importer =>
            {
                importer.bakeAxisConversion = true;
            }, "Bake Axis Conversion applied!");
        }

        [MenuItem("Assets/FBX Editor/Quick Fix - Bake Axis Conversion", true)]
        private static bool QuickFixBakeAxisValidate()
        {
            return IsValidFBXSelection();
        }

        [MenuItem("Assets/FBX Editor/Fix Rotation X: -90 → 0", false, 101)]
        private static void FixRotationX90()
        {
            ProcessSelectedFBX(importer =>
            {
                importer.bakeAxisConversion = true;
            }, "Rotation fixed (Bake Axis enabled)!");
        }

        [MenuItem("Assets/FBX Editor/Fix Rotation X: -90 → 0", true)]
        private static bool FixRotationX90Validate()
        {
            return IsValidFBXSelection();
        }

        [MenuItem("Assets/FBX Editor/Reset Import Settings", false, 200)]
        private static void ResetImportSettings()
        {
            ProcessSelectedFBX(importer =>
            {
                importer.bakeAxisConversion = false;
                importer.globalScale = 1f;
                importer.useFileScale = true;
            }, "Import Settings reset!");
        }

        [MenuItem("Assets/FBX Editor/Reset Import Settings", true)]
        private static bool ResetImportSettingsValidate()
        {
            return IsValidFBXSelection();
        }

        [MenuItem("Assets/FBX Editor/Show Import Info", false, 300)]
        private static void ShowImportInfo()
        {
            Object selected = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(selected);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null)
            {
                string info = $"📊 Import Information\n\n" +
                             $"File: {System.IO.Path.GetFileName(path)}\n" +
                             $"Path: {path}\n\n" +
                             $"🔄 Rotation:\n" +
                             $"  Bake Axis Conversion: {importer.bakeAxisConversion}\n\n" +
                             $"📐 Scale:\n" +
                             $"  Global Scale: {importer.globalScale}\n" +
                             $"  Use File Scale: {importer.useFileScale}\n" +
                             $"  File Scale: {importer.fileScale}\n\n" +
                             $"⚙️ Other:\n" +
                             $"  Import Visibility: {importer.importVisibility}\n" +
                             $"  Import Cameras: {importer.importCameras}\n" +
                             $"  Import Lights: {importer.importLights}";

                EditorUtility.DisplayDialog("FBX Import Info", info, "OK");
            }
        }

        [MenuItem("Assets/FBX Editor/Show Import Info", true)]
        private static bool ShowImportInfoValidate()
        {
            return IsValidFBXSelection();
        }

        [MenuItem("Assets/FBX Editor/Open in FBX Editor Window", false, 400)]
        private static void OpenInEditorWindow()
        {
            FBXEditorWindow.ShowWindow();
        }

        [MenuItem("Assets/FBX Editor/Open in FBX Editor Window", true)]
        private static bool OpenInEditorWindowValidate()
        {
            return IsValidFBXSelection();
        }

        // === Helpers ===

        private static bool IsValidFBXSelection()
        {
            if (Selection.activeObject == null) return false;
            
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return !string.IsNullOrEmpty(path) && path.ToLower().EndsWith(".fbx");
        }

        private static void ProcessSelectedFBX(System.Action<ModelImporter> processAction, string successMessage)
        {
            Object[] selectedObjects = Selection.objects;
            int processed = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (Object obj in selectedObjects)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (string.IsNullOrEmpty(path) || !path.ToLower().EndsWith(".fbx"))
                        continue;

                    ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                    if (importer != null)
                    {
                        processAction(importer);
                        EditorUtility.SetDirty(importer);
                        importer.SaveAndReimport();
                        processed++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();

            if (processed > 0)
            {
                EditorUtility.DisplayDialog("FBX Editor", 
                    $"{successMessage}\n\nFiles processed: {processed}", "OK");
            }
        }
    }
}
