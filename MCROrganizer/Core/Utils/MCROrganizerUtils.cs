using System;
using System.IO;

namespace MCROrganizer.Core.Utils
{
    public static class PathUtils
    {
        private static String _baseDirectoryPath = Environment.CurrentDirectory;
        private static String _projectPath = Directory.GetParent(_baseDirectoryPath).Parent.FullName;
        private static String _dataPath = Path.Combine(_projectPath, "Data");
        private static String _imagePath = $"pack://application:,,,/{System.Reflection.Assembly.GetExecutingAssembly().FullName};component/Data/Images";
        private static String _generalDataPath = Path.Combine(_dataPath, "General");
        private static String _extension = ".mcro";
        private static String _generalDataFilePath = _generalDataPath + "General" + _extension;
        private static String _MCROFilter = $"MCRO Files|*{_extension}";
        private static String _caption = "MCRO" /* To be replaced with the name of the application. */;

        public static String BaseDirectoryPath => _baseDirectoryPath;
        public static String ProjectPath => _projectPath;
        public static String ImagePath => _imagePath;
        public static String GeneralDataPath => _generalDataPath;
        public static String Extension => _extension;
        public static String GeneralDataFilePath => _generalDataFilePath;
        public static String MCROFilter => _MCROFilter;
        public static String Caption => _caption;
    }
}