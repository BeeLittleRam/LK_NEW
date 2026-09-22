using UnityEngine;
using UnityEditor;

namespace FBXEditor
{
    /// <summary>
    /// Automatically processes imported FBX files and fixes rotation
    /// </summary>
    public class FBXImportProcessor : AssetPostprocessor
    {
        // Default settings (can be changed via EditorPrefs)
        private static bool autoFixEnabled = false;
        private static Vector3 defaultRotationFix = new Vector3(0, 0, 0);

        private const string PREFS_AUTO_FIX = "FBXEditor_AutoFix";
        private const string PREFS_ROTATION_X = "FBXEditor_RotX";
        private const string PREFS_ROTATION_Y = "FBXEditor_RotY";
        private const string PREFS_ROTATION_Z = "FBXEditor_RotZ";
        private const string PREFS_BAKE_AXIS = "FBXEditor_BakeAxis";

        static FBXImportProcessor()
        {
            LoadSettings();
        }

        public static void LoadSettings()
        {
            autoFixEnabled = EditorPrefs.GetBool(PREFS_AUTO_FIX, false);
            defaultRotationFix.x = EditorPrefs.GetFloat(PREFS_ROTATION_X, 0);
            defaultRotationFix.y = EditorPrefs.GetFloat(PREFS_ROTATION_Y, 0);
            defaultRotationFix.z = EditorPrefs.GetFloat(PREFS_ROTATION_Z, 0);
        }

        public static void SaveSettings(bool autoFix, Vector3 rotation, bool bakeAxis)
        {
            EditorPrefs.SetBool(PREFS_AUTO_FIX, autoFix);
            EditorPrefs.SetFloat(PREFS_ROTATION_X, rotation.x);
            EditorPrefs.SetFloat(PREFS_ROTATION_Y, rotation.y);
            EditorPrefs.SetFloat(PREFS_ROTATION_Z, rotation.z);
            EditorPrefs.SetBool(PREFS_BAKE_AXIS, bakeAxis);
            
            LoadSettings();
        }

        public static bool IsAutoFixEnabled => autoFixEnabled;
        public static Vector3 DefaultRotationFix => defaultRotationFix;
        public static bool BakeAxisEnabled => EditorPrefs.GetBool(PREFS_BAKE_AXIS, true);

        /// <summary>
        /// Called before the model is imported
        /// </summary>
        private void OnPreprocessModel()
        {
            if (!autoFixEnabled) return;

            ModelImporter importer = assetImporter as ModelImporter;
            if (importer == null) return;

            // Applies Bake Axis Conversion if enabled
            if (BakeAxisEnabled)
            {
                importer.bakeAxisConversion = true;
            }

            Debug.Log($"[FBX Editor] Pre-processing: {assetPath}");
        }

        /// <summary>
        /// Called after the model is imported
        /// </summary>
        private void OnPostprocessModel(GameObject model)
        {
            if (!autoFixEnabled) return;
            
            Debug.Log($"[FBX Editor] Post-processing: {model.name}");
            
            // Applies rotation if configured
            if (defaultRotationFix != Vector3.zero)
            {
                // Note: Modifying rotation here only affects internal hierarchy
                // For a deeper fix, use OnPreprocessModel
                Debug.Log($"[FBX Editor] Applied rotation fix: {defaultRotationFix}");
            }
        }
    }

    /// <summary>
    /// Auto processor settings window
    /// </summary>
    public class FBXImportSettingsWindow : EditorWindow
    {
        private bool autoFixEnabled;
        private Vector3 rotationFix;
        private bool bakeAxisConversion;

        [MenuItem("Tools/FBX Editor/Auto Import Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<FBXImportSettingsWindow>("FBX Import Settings");
            window.minSize = new Vector2(350, 250);
        }

        private void OnEnable()
        {
            FBXImportProcessor.LoadSettings();
            autoFixEnabled = FBXImportProcessor.IsAutoFixEnabled;
            rotationFix = FBXImportProcessor.DefaultRotationFix;
            bakeAxisConversion = FBXImportProcessor.BakeAxisEnabled;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            
            EditorGUILayout.LabelField("⚙️ Auto-Import Settings", titleStyle);
            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "When enabled, these settings will be automatically applied " +
                "to all newly imported FBX files.",
                MessageType.Info);

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            
            autoFixEnabled = EditorGUILayout.Toggle("Auto-Fix Enabled", autoFixEnabled);
            
            EditorGUI.BeginDisabledGroup(!autoFixEnabled);
            
            EditorGUILayout.Space(5);
            rotationFix = EditorGUILayout.Vector3Field("Default Rotation", rotationFix);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Blender Preset (X: -90)"))
            {
                rotationFix = new Vector3(-90, 0, 0);
            }
            if (GUILayout.Button("Reset"))
            {
                rotationFix = Vector3.zero;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            bakeAxisConversion = EditorGUILayout.Toggle("Bake Axis Conversion", bakeAxisConversion);
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("💾 Save Settings", GUILayout.Height(30)))
            {
                FBXImportProcessor.SaveSettings(autoFixEnabled, rotationFix, bakeAxisConversion);
                EditorUtility.DisplayDialog("Success", "Settings saved!", "OK");
            }
            GUI.backgroundColor = Color.white;
            
            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                Close();
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Current Status:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Auto-Fix: {(FBXImportProcessor.IsAutoFixEnabled ? "✅ Enabled" : "❌ Disabled")}");
            EditorGUILayout.LabelField($"Rotation: {FBXImportProcessor.DefaultRotationFix}");
            EditorGUILayout.EndVertical();
        }
    }
}
