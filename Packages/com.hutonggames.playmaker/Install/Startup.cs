
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMaker.Editor.Install
{
    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {
            // Delayed just to be safe
            EditorApplication.delayCall += OpenInstallWindow;

            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }
        
        private static void OpenInstallWindow()
        {
            // We only auto-open once per release build
            if (InstallInfo.UpToDate) return;

            // No graphics device in batch mode
            if (Application.isBatchMode) return;
            
            InstallWindow.Open();
            InstallInfo.RecordInstallerWasOpened();
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            if (!IsPlayMakerPackage(packageName)) return;

            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            EditorApplication.delayCall += CloseInstallWindow;
        }

        private static bool IsPlayMakerPackage(string packageName) =>
            string.Equals(Path.GetFileNameWithoutExtension(packageName), "PlayMaker", StringComparison.OrdinalIgnoreCase);

        private static void CloseInstallWindow()
        {
            var installWindows = Resources.FindObjectsOfTypeAll<InstallWindow>();
            foreach (var installWindow in installWindows)
            {
                installWindow.Close();
            }
        }
    }
}
