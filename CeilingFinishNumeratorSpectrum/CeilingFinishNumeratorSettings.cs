using System;
using System.IO;
using System.Xml.Serialization;

namespace CeilingFinishNumerator
{
    public class CeilingFinishNumeratorSettings
    {
        public string CeilingFinishNumberingSelectedName { get; set; }
        public bool ProcessSelectedLevel { get; set; }
        public bool SeparatedBySections { get; set; }
        public string SelectedLevelName { get; set; }
        public string SelectedParameterName { get; set; }
        public bool FillRoomBookParameters { get; set; }

        private const string FileName = "CeilingFinishNumeratorSettings.xml";

        private static string SettingsDirectory =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Citrus BIM",
                "CeilingFinishNumerator");

        private static string SettingsFilePath => Path.Combine(SettingsDirectory, FileName);

        public static CeilingFinishNumeratorSettings GetSettings()
        {
            if (!File.Exists(SettingsFilePath))
                return new CeilingFinishNumeratorSettings();

            try
            {
                using (var fs = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var xSer = new XmlSerializer(typeof(CeilingFinishNumeratorSettings));
                    return xSer.Deserialize(fs) as CeilingFinishNumeratorSettings
                           ?? new CeilingFinishNumeratorSettings();
                }
            }
            catch
            {
                return new CeilingFinishNumeratorSettings();
            }
        }

        public void SaveSettings()
        {
            Directory.CreateDirectory(SettingsDirectory);

            var tmpPath = SettingsFilePath + ".tmp";

            if (File.Exists(tmpPath))
            {
                try { File.Delete(tmpPath); } catch { }
            }

            var xSer = new XmlSerializer(typeof(CeilingFinishNumeratorSettings));
            using (var fs = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xSer.Serialize(fs, this);
            }

            TryReplaceOrMove(tmpPath, SettingsFilePath);
        }

        private static void TryReplaceOrMove(string tmpPath, string targetPath)
        {
            try
            {
                if (File.Exists(targetPath))
                    File.Replace(tmpPath, targetPath, destinationBackupFileName: null);
                else
                    File.Move(tmpPath, targetPath);
            }
            catch
            {
                try
                {
                    if (File.Exists(targetPath))
                        File.Delete(targetPath);

                    File.Move(tmpPath, targetPath);
                }
                catch
                {
                }
            }
        }
    }
}
