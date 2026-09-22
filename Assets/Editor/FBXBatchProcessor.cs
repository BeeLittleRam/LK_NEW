using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FBXEditor
{
    /// <summary>
    /// Tool for processing multiple FBX files at once
    /// </summary>
    public class FBXBatchProcessor : EditorWindow
    {
        private List<Object> fbxFiles = new List<Object>();
        private Vector3 rotationOffset = new Vector3(0, 0, 0);
        private bool bakeAxisConversion = true;
        private Vector2 scrollPosition;
        private bool showAdvanced = false;
        private float globalScale = 1f;
        private bool useFileScale = true;

        [MenuItem("Tools/FBX Editor/Batch Processor")]
        public static void ShowWindow()
        {
            var window = GetWindow<FBXBatchProcessor>("FBX Batch Processor");
            window.minSize = new Vector2(450, 500);
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            EditorGUILayout.Space(10);
            
            DrawFileSelection();
            EditorGUILayout.Space(10);
            
            DrawSettings();
            EditorGUILayout.Space(10);
            
            DrawAdvancedSettings();
            EditorGUILayout.Space(10);
            
            DrawActions();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField("📦 FBX Batch Processor", titleStyle, GUILayout.Height(30));
            
            EditorGUILayout.HelpBox(
                "Process multiple FBX files at once. " +
                "Drag FBX files to the list or use the button to add them.",
                MessageType.Info);
        }

        private void DrawFileSelection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("📁 FBX Files", EditorStyles.boldLabel);

            // Drag and drop area
            Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag FBX files here", EditorStyles.helpBox);

            // Detects drag and drop
            Event evt = Event.current;
            if (dropArea.Contains(evt.mousePosition))
            {
                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }
                else if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        if (path.ToLower().EndsWith(".fbx") && !fbxFiles.Contains(obj))
                        {
                            fbxFiles.Add(obj);
                        }
                    }
                    evt.Use();
                }
            }

            EditorGUILayout.Space(5);

            // Action buttons for the list
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("➕ Add FBX"))
            {
                string path = EditorUtility.OpenFilePanel("Select FBX", "Assets", "fbx");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        path = "Assets" + path.Substring(Application.dataPath.Length);
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                        if (obj != null && !fbxFiles.Contains(obj))
                        {
                            fbxFiles.Add(obj);
                        }
                    }
                }
            }

            if (GUILayout.Button("📂 Add Folder"))
            {
                string folder = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                if (!string.IsNullOrEmpty(folder) && folder.StartsWith(Application.dataPath))
                {
                    folder = "Assets" + folder.Substring(Application.dataPath.Length);
                    string[] guids = AssetDatabase.FindAssets("t:Model", new[] { folder });
                    
                    foreach (string guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        if (assetPath.ToLower().EndsWith(".fbx"))
                        {
                            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                            if (obj != null && !fbxFiles.Contains(obj))
                            {
                                fbxFiles.Add(obj);
                            }
                        }
                    }
                }
            }

            if (GUILayout.Button("🗑️ Clear List"))
            {
                fbxFiles.Clear();
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // File list
            EditorGUILayout.LabelField($"Files in list: {fbxFiles.Count}");
            
            if (fbxFiles.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                
                for (int i = fbxFiles.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(fbxFiles[i], typeof(Object), false);
                    EditorGUI.EndDisabledGroup();
                    
                    if (GUILayout.Button("✕", GUILayout.Width(25)))
                    {
                        fbxFiles.RemoveAt(i);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSettings()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("⚙️ Settings", EditorStyles.boldLabel);

            bakeAxisConversion = EditorGUILayout.Toggle("Bake Axis Conversion", bakeAxisConversion);
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Rotation Presets:", EditorStyles.miniLabel);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Blender\n(Bake Axis)", GUILayout.Height(35)))
            {
                bakeAxisConversion = true;
                rotationOffset = Vector3.zero;
            }
            
            if (GUILayout.Button("3DS Max\n(Bake Axis)", GUILayout.Height(35)))
            {
                bakeAxisConversion = true;
                rotationOffset = Vector3.zero;
            }
            
            if (GUILayout.Button("Custom\n(Manual)", GUILayout.Height(35)))
            {
                bakeAxisConversion = false;
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedSettings()
        {
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "🔧 Advanced Settings");
            
            if (showAdvanced)
            {
                EditorGUILayout.BeginVertical("box");
                
                globalScale = EditorGUILayout.FloatField("Global Scale", globalScale);
                useFileScale = EditorGUILayout.Toggle("Use File Scale", useFileScale);
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Scale: 0.01"))
                {
                    globalScale = 0.01f;
                }
                if (GUILayout.Button("Scale: 1"))
                {
                    globalScale = 1f;
                }
                if (GUILayout.Button("Scale: 100"))
                {
                    globalScale = 100f;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawActions()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("🎬 Actions", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(fbxFiles.Count == 0);

            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("✅ Process All FBX Files", GUILayout.Height(40)))
            {
                ProcessAllFBX();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            
            GUI.backgroundColor = new Color(0.9f, 0.7f, 0.3f);
            if (GUILayout.Button("🔄 Reimport All", GUILayout.Height(30)))
            {
                ReimportAllFBX();
            }
            GUI.backgroundColor = Color.white;

            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.9f);
            if (GUILayout.Button("📋 View Settings", GUILayout.Height(30)))
            {
                ShowCurrentSettings();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }

        private void ProcessAllFBX()
        {
            if (fbxFiles.Count == 0) return;

            int processed = 0;
            int failed = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                for (int i = 0; i < fbxFiles.Count; i++)
                {
                    string path = AssetDatabase.GetAssetPath(fbxFiles[i]);
                    
                    EditorUtility.DisplayProgressBar(
                        "Processing FBX",
                        $"Processing: {System.IO.Path.GetFileName(path)}",
                        (float)i / fbxFiles.Count);

                    ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                    
                    if (importer != null)
                    {
                        importer.bakeAxisConversion = bakeAxisConversion;
                        importer.globalScale = globalScale;
                        importer.useFileScale = useFileScale;
                        
                        EditorUtility.SetDirty(importer);
                        importer.SaveAndReimport();
                        
                        processed++;
                    }
                    else
                    {
                        failed++;
                        Debug.LogWarning($"[FBX Batch] Could not process: {path}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Processing Complete",
                $"✅ Processed: {processed}\n❌ Failed: {failed}",
                "OK");
        }

        private void ReimportAllFBX()
        {
            if (fbxFiles.Count == 0) return;

            try
            {
                for (int i = 0; i < fbxFiles.Count; i++)
                {
                    string path = AssetDatabase.GetAssetPath(fbxFiles[i]);
                    
                    EditorUtility.DisplayProgressBar(
                        "Reimporting FBX",
                        $"Reimporting: {System.IO.Path.GetFileName(path)}",
                        (float)i / fbxFiles.Count);

                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            EditorUtility.DisplayDialog("Complete", $"Reimported: {fbxFiles.Count} files", "OK");
        }

        private void ShowCurrentSettings()
        {
            string settings = $"Current Settings:\n\n" +
                             $"Bake Axis Conversion: {bakeAxisConversion}\n" +
                             $"Global Scale: {globalScale}\n" +
                             $"Use File Scale: {useFileScale}\n" +
                             $"Files in list: {fbxFiles.Count}";

            EditorUtility.DisplayDialog("Settings", settings, "OK");
        }
    }
}
