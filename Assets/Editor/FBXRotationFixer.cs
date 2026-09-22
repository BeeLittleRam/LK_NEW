using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Ferramenta para corrigir a rotação de FBX importados do Blender/3DS Max
/// Como o Transform do FBX é bloqueado, criamos um Prefab com a correção aplicada
/// </summary>
public class FBXRotationFixer : EditorWindow
{
    private Vector3 rotationCorrection = new Vector3(90f, 0f, 0f);
    private bool createPrefab = true;
    private string prefabSuffix = "_Fixed";
    private bool applyToAnimator = true;

    [MenuItem("Tools/FBX Rotation Fixer")]
    public static void ShowWindow()
    {
        var window = GetWindow<FBXRotationFixer>("FBX Rotation Fixer");
        window.minSize = new Vector2(350, 250);
    }

    private void OnGUI()
    {
        GUILayout.Label("🔧 FBX Rotation Fixer", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "FBX do Blender/3DS Max vêm com rotação -90 no X.\n" +
            "Esta ferramenta cria um Prefab com a correção aplicada.", 
            MessageType.Info);

        EditorGUILayout.Space(10);

        // Configurações
        GUILayout.Label("Configurações", EditorStyles.boldLabel);
        
        rotationCorrection = EditorGUILayout.Vector3Field("Correção de Rotação", rotationCorrection);
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Preset: Blender → Unity (90, 0, 0)"))
        {
            rotationCorrection = new Vector3(90f, 0f, 0f);
        }
        
        if (GUILayout.Button("Preset: Inverter atual (-90, 0, 0)"))
        {
            rotationCorrection = new Vector3(-rotationCorrection.x, -rotationCorrection.y, -rotationCorrection.z);
        }

        EditorGUILayout.Space(10);
        
        createPrefab = EditorGUILayout.Toggle("Criar Prefab", createPrefab);
        if (createPrefab)
        {
            prefabSuffix = EditorGUILayout.TextField("Sufixo do Prefab", prefabSuffix);
        }

        EditorGUILayout.Space(20);

        // Botão principal
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("✓ Aplicar aos FBX Selecionados", GUILayout.Height(35)))
        {
            ApplyToSelected();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);

        // Info de seleção
        int fbxCount = CountSelectedFBX();
        if (fbxCount > 0)
        {
            EditorGUILayout.HelpBox($"✓ {fbxCount} arquivo(s) FBX selecionado(s)", MessageType.None);
        }
        else
        {
            EditorGUILayout.HelpBox("Selecione arquivo(s) FBX na janela Project", MessageType.Warning);
        }
    }

    private int CountSelectedFBX()
    {
        int count = 0;
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.ToLower().EndsWith(".fbx"))
                count++;
        }
        return count;
    }

    private void ApplyToSelected()
    {
        int processed = 0;
        
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!path.ToLower().EndsWith(".fbx"))
                continue;

            GameObject fbxAsset = obj as GameObject;
            if (fbxAsset == null)
                fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (fbxAsset == null)
            {
                Debug.LogError($"Não foi possível carregar: {path}");
                continue;
            }

            if (createPrefab)
            {
                CreateCorrectedPrefab(fbxAsset, path);
            }
            else
            {
                CreateCorrectedInstance(fbxAsset);
            }

            processed++;
        }

        if (processed > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"✓ {processed} FBX(s) processado(s)!");
        }
        else
        {
            Debug.LogWarning("Nenhum FBX foi selecionado!");
        }
    }

    private void CreateCorrectedPrefab(GameObject fbxAsset, string fbxPath)
    {
        // Cria instância temporária
        GameObject instance = Instantiate(fbxAsset);
        instance.name = fbxAsset.name + prefabSuffix;

        // Cria um objeto pai para aplicar a correção
        GameObject wrapper = new GameObject(instance.name);
        instance.transform.SetParent(wrapper.transform);
        
        // Aplica a rotação no objeto filho (o FBX)
        instance.transform.localRotation = Quaternion.Euler(rotationCorrection);
        
        // Define o caminho do prefab (mesma pasta do FBX)
        string directory = Path.GetDirectoryName(fbxPath);
        string prefabPath = Path.Combine(directory, instance.name + ".prefab");
        prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

        // Salva como prefab
        PrefabUtility.SaveAsPrefabAsset(wrapper, prefabPath);
        
        // Limpa a instância temporária
        DestroyImmediate(wrapper);

        Debug.Log($"✓ Prefab criado: {prefabPath}");
    }

    private void CreateCorrectedInstance(GameObject fbxAsset)
    {
        // Apenas cria uma instância na cena com a correção
        GameObject wrapper = new GameObject(fbxAsset.name + "_Corrected");
        GameObject instance = Instantiate(fbxAsset, wrapper.transform);
        instance.name = fbxAsset.name;
        instance.transform.localRotation = Quaternion.Euler(rotationCorrection);

        Selection.activeGameObject = wrapper;
        Debug.Log($"✓ Instância criada na cena: {wrapper.name}");
    }
}

/// <summary>
/// Menu de contexto rápido para FBX
/// </summary>
public static class FBXQuickFix
{
    [MenuItem("Assets/Criar Prefab Corrigido (Rotação)", false, 50)]
    private static void CreateFixedPrefab()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!path.ToLower().EndsWith(".fbx"))
                continue;

            GameObject fbxAsset = obj as GameObject;
            if (fbxAsset == null)
                fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (fbxAsset == null)
                continue;

            // Cria instância com correção
            GameObject wrapper = new GameObject(fbxAsset.name + "_Fixed");
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset, wrapper.transform);
            instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // Salva como prefab
            string directory = Path.GetDirectoryName(path);
            string prefabPath = Path.Combine(directory, fbxAsset.name + "_Fixed.prefab");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

            PrefabUtility.SaveAsPrefabAsset(wrapper, prefabPath);
            Object.DestroyImmediate(wrapper);

            Debug.Log($"✓ Prefab criado: {prefabPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Criar Prefab Corrigido (Rotação)", true)]
    private static bool CreateFixedPrefabValidate()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.ToLower().EndsWith(".fbx"))
                return true;
        }
        return false;
    }
}
