using System.IO;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMaker.Editor.Install
{
    public static class Installer
    {
        private const string InstallFolder = "Packages/com.hutonggames.playmaker/Install";
      
        public static void InstallPlayMaker()
        {
            var file = GetFileName(InstallFolder, "PlayMaker.unitypackage");
            AssetDatabase.ImportPackage(file, false);
        }

        private static string GetFileName(string folder, string name) => 
            Path.GetFullPath(Application.dataPath + $"/../{folder}/{name}");
    }
}