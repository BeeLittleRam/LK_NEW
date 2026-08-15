using System.IO;
using UnityEngine;


namespace HutongGames.PlayMaker.Editor.Install
{
    /// <summary>
    /// Helper to check if PlayMaker is installed and if it needs updating.
    /// </summary>
    public static class InstallInfo
    {
        /// <summary>
        /// Version file written by the installer.
        /// </summary>
        private const string InstallerVersionFile = "../ProjectSettings/PlayMaker/InstallerVersion.txt";
        
        /// <summary>
        /// Version file written after PlayMaker is installed.
        /// </summary>
        private const string InstalledVersionFile = "../ProjectSettings/PlayMaker/InstalledVersion.txt";
        
        /// <summary>
        /// The version this installer will install.
        /// </summary>
        public const string InstallerVersion = "2.0.0b81";
        
        public static bool UpToDate => LastInstallerVersion == InstallerVersion && IsPlayMakerInstalled();
        
        public static string InstalledVersion { get; private set; }

        public static bool InstallerBuildIsOlder => VersionParser.IsNewer(InstalledVersion, InstallerVersion);

        private static string LastInstallerVersion { get; set; }

        static InstallInfo() => UpdateInfo();

        public static bool IsPlayMakerInstalled() => HasImportedPlayMaker() && !string.IsNullOrEmpty(InstalledVersion);
        
        public static void RecordInstallerWasOpened()
        {
            LastInstallerVersion = InstallerVersion;
            WriteInstallerVersionFile();
        }
        
        public static void UpdateInfo()
        {
            InstalledVersion = string.Empty;
            LastInstallerVersion = string.Empty;

            var installedVersionFile = GetVersionFilePath(InstalledVersionFile);
            var installerVersionFile = GetVersionFilePath(InstallerVersionFile);

            LastInstallerVersion = ReadVersionFile(installerVersionFile);

            if (!HasImportedPlayMaker())
            {
                DeleteVersionFile(installedVersionFile);
                return;
            }

            InstalledVersion = ReadVersionFile(installedVersionFile);
        }
        
        private static void WriteInstallerVersionFile()
        {
            var installerVersionFile = GetVersionFilePath(InstallerVersionFile);
            Directory.CreateDirectory(Path.GetDirectoryName(installerVersionFile) ?? "");
            File.WriteAllText(installerVersionFile, InstallerVersion);
        }

        private static bool HasImportedPlayMaker() =>
            // We can't just check if the package exists here
            // because the importer is imported by importing the package!
            // Instead, we check if the Runtime directory exists,
            // which is only true after the installer has imported PlayMaker.
            PackageHelpers.DoesPackageDirectoryExist("com.hutonggames.playmaker", "Runtime");

        private static string GetVersionFilePath(string relativePath) =>
            Path.GetFullPath(Path.Combine(Application.dataPath, relativePath));

        private static string ReadVersionFile(string filePath) =>
            File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

        private static void DeleteVersionFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }
    }
}
