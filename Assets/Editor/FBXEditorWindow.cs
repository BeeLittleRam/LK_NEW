using UnityEngine;
using UnityEditor;
using System.IO;

namespace FBXEditor
{
    /// <summary>
    /// Editor Window to adjust rotation and scale of imported FBX models
    /// </summary>
    public class FBXEditorWindow : EditorWindow
    {
        private GameObject selectedFBX;
        private Vector3 rotationOffset = Vector3.zero;
        private Vector3 scaleMultiplier = Vector3.one;
        private bool applyToChildren = true;
        private bool createPrefab = true;
        private string outputFolder = "Assets/FixedModels";
        
        private Vector2 scrollPosition;
        private ModelImporter currentImporter;
        private string currentAssetPath;

        [MenuItem("Tools/FBX Editor/Rotation Fixer")]
        public static void ShowWindow()
        {
            var window = GetWindow<FBXEditorWindow>("FBX Editor");
            window.minSize = new Vector2(400, 500);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            UpdateSelection();
            Repaint();
        }

        private void UpdateSelection()
        {
            if (Selection.activeObject != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(path) && path.ToLower().EndsWith(".fbx"))
                {
                    selectedFBX = Selection.activeObject as GameObject;
                    currentAssetPath = path;
                    currentImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                }
            }
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawHeader();
            EditorGUILayout.Space(10);
            
            DrawSelectionSection();
            EditorGUILayout.Space(10);
            
            if (selectedFBX != null)
            {
                DrawCurrentInfoSection();
                EditorGUILayout.Space(10);
                
                DrawRotationSection();
                EditorGUILayout.Space(10);
                
                DrawScaleSection();
                EditorGUILayout.Space(10);
                
                DrawOutputSection();
                EditorGUILayout.Space(10);
                
                DrawActionButtons();
            }
            
            EditorGUILayout.Space(20);
            DrawQuickFixSection();
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField("🔧 FBX Editor - Rotation Fixer", titleStyle, GUILayout.Height(30));
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox(
                "Select an FBX file in the Project window to adjust its import rotation and scale.",
                MessageType.Info);
        }

        private void DrawSelectionSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("📁 Selected File", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            selectedFBX = (GameObject)EditorGUILayout.ObjectField(
                "FBX Model",
                selectedFBX,
                typeof(GameObject),
                false);
            
            if (EditorGUI.EndChangeCheck() && selectedFBX != null)
            {
                currentAssetPath = AssetDatabase.GetAssetPath(selectedFBX);
                currentImporter = AssetImporter.GetAtPath(currentAssetPath) as ModelImporter;
            }
            
            if (!string.IsNullOrEmpty(currentAssetPath))
            {
                EditorGUILayout.LabelField("Path:", currentAssetPath, EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawCurrentInfoSection()
        {
            if (currentImporter == null) return;
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("📊 Current Import Settings", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            
            // Shows current import settings
            EditorGUILayout.LabelField($"Scale Factor: {currentImporter.globalScale}");
            EditorGUILayout.LabelField($"Use File Scale: {currentImporter.useFileScale}");
            EditorGUILayout.LabelField($"Bake Axis Conversion: {currentImporter.bakeAxisConversion}");
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawRotationSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("🔄 Rotation Adjustment", EditorStyles.boldLabel);
            
            rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", rotationOffset);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reset", GUILayout.Width(60)))
            {
                rotationOffset = Vector3.zero;
            }
            
            if (GUILayout.Button("X: +90"))
            {
                rotationOffset.x += 90;
            }
            
            if (GUILayout.Button("X: -90"))
            {
                rotationOffset.x -= 90;
            }
            
            if (GUILayout.Button("Y: +90"))
            {
                rotationOffset.y += 90;
            }
            
            if (GUILayout.Button("Z: +90"))
            {
                rotationOffset.z += 90;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Common presets
            EditorGUILayout.LabelField("Common Presets:", EditorStyles.miniLabel);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Blender → Unity\n(X: -90)", GUILayout.Height(35)))
            {
                rotationOffset = new Vector3(-90, 0, 0);
            }
            
            if (GUILayout.Button("3DS Max → Unity\n(X: -90)", GUILayout.Height(35)))
            {
                rotationOffset = new Vector3(-90, 0, 0);
            }
            
            if (GUILayout.Button("Maya → Unity\n(X: 0)", GUILayout.Height(35)))
            {
                rotationOffset = Vector3.zero;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawScaleSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("📐 Scale Adjustment", EditorStyles.boldLabel);
            
            scaleMultiplier = EditorGUILayout.Vector3Field("Scale Multiplier", scaleMultiplier);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reset"))
            {
                scaleMultiplier = Vector3.one;
            }
            
            if (GUILayout.Button("0.01 (cm→m)"))
            {
                scaleMultiplier = Vector3.one * 0.01f;
            }
            
            if (GUILayout.Button("100 (m→cm)"))
            {
                scaleMultiplier = Vector3.one * 100f;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawOutputSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("💾 Output Options", EditorStyles.boldLabel);
            
            applyToChildren = EditorGUILayout.Toggle("Apply to Children", applyToChildren);
            createPrefab = EditorGUILayout.Toggle("Create Prefab", createPrefab);
            
            if (createPrefab)
            {
                EditorGUILayout.BeginHorizontal();
                outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
                
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string folder = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
                    if (!string.IsNullOrEmpty(folder))
                    {
                        if (folder.StartsWith(Application.dataPath))
                        {
                            outputFolder = "Assets" + folder.Substring(Application.dataPath.Length);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.BeginVertical("box");
            
            // Main apply button
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("✅ Apply Rotation Fix", GUILayout.Height(40)))
            {
                ApplyRotationFix();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            // Modify Import Settings directly
            GUI.backgroundColor = new Color(0.4f, 0.6f, 0.9f);
            if (GUILayout.Button("🔧 Modify Import Settings", GUILayout.Height(30)))
            {
                ModifyImportSettings();
            }
            GUI.backgroundColor = Color.white;
            
            // Reimport
            GUI.backgroundColor = new Color(0.9f, 0.7f, 0.3f);
            if (GUILayout.Button("🔄 Reimport FBX", GUILayout.Height(30)))
            {
                ReimportFBX();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawQuickFixSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("⚡ Quick Fix - Fix X: -90", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "If your model has X rotation at -90° (common with Blender models), " +
                "use the button below to fix it automatically.",
                MessageType.Warning);
            
            GUI.backgroundColor = new Color(1f, 0.5f, 0.3f);
            if (GUILayout.Button("🚀 QUICK FIX: Fix Rotation X: -90 → 0", GUILayout.Height(35)))
            {
                if (selectedFBX != null)
                {
                    rotationOffset = new Vector3(90, 0, 0); // Adds +90 to compensate for -90
                    ApplyRotationFix();
                }
                else
                {
                    EditorUtility.DisplayDialog("FBX Editor", "Please select an FBX file first!", "OK");
                }
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndVertical();
        }

        private void ApplyRotationFix()
        {
            if (selectedFBX == null)
            {
                EditorUtility.DisplayDialog("Error", "No FBX selected!", "OK");
                return;
            }

            // Creates an instance of the model
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedFBX);
            
            if (instance == null)
            {
                instance = Instantiate(selectedFBX);
            }
            
            instance.name = selectedFBX.name + "_Fixed";

            // Creates a parent to apply the rotation
            GameObject wrapper = new GameObject(instance.name + "_Wrapper");
            instance.transform.SetParent(wrapper.transform);
            
            // Applies the rotation offset
            instance.transform.localRotation = Quaternion.Euler(rotationOffset);
            instance.transform.localScale = scaleMultiplier;

            if (createPrefab)
            {
                SaveAsPrefab(wrapper);
            }

            Selection.activeGameObject = wrapper;
            
            EditorUtility.DisplayDialog("Success", 
                $"Rotation applied!\n\nOffset: {rotationOffset}\nScale: {scaleMultiplier}", "OK");
        }

        private void ModifyImportSettings()
        {
            if (currentImporter == null)
            {
                EditorUtility.DisplayDialog("Error", "No ModelImporter found!", "OK");
                return;
            }

            Undo.RecordObject(currentImporter, "Modify FBX Import Settings");
            
            // Enables Bake Axis Conversion to bake the rotation into the model
            currentImporter.bakeAxisConversion = true;
            
            // Applies the changes
            EditorUtility.SetDirty(currentImporter);
            currentImporter.SaveAndReimport();
            
            EditorUtility.DisplayDialog("Success", 
                "Import Settings modified!\n\n'Bake Axis Conversion' has been enabled.\n" +
                "The model will be reimported with the rotation fixed.", "OK");
        }

        private void ReimportFBX()
        {
            if (string.IsNullOrEmpty(currentAssetPath))
            {
                EditorUtility.DisplayDialog("Error", "No asset path found!", "OK");
                return;
            }

            AssetDatabase.ImportAsset(currentAssetPath, ImportAssetOptions.ForceUpdate);
            
            // Updates references
            currentImporter = AssetImporter.GetAtPath(currentAssetPath) as ModelImporter;
            
            EditorUtility.DisplayDialog("Success", "FBX reimported!", "OK");
        }

        private void SaveAsPrefab(GameObject obj)
        {
            // Ensures the folder exists
            if (!AssetDatabase.IsValidFolder(outputFolder))
            {
                string[] folders = outputFolder.Split('/');
                string currentPath = folders[0];
                
                for (int i = 1; i < folders.Length; i++)
                {
                    string newPath = currentPath + "/" + folders[i];
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, folders[i]);
                    }
                    currentPath = newPath;
                }
            }

            string prefabPath = $"{outputFolder}/{obj.name}.prefab";
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
            
            PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
            
            Debug.Log($"Prefab saved at: {prefabPath}");
        }
    }
}
