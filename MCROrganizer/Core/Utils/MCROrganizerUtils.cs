using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCROrganizer.Core.Utils
{
    public class PathUtils
    {
        private static String _baseDirectoryPath = Environment.CurrentDirectory;
        private static String _projectPath = Directory.GetParent(_baseDirectoryPath).Parent.FullName;
        private static String _dataPath = Path.Combine(_projectPath, "Data");
        private static String _imagePath = Path.Combine(_dataPath, "Images");
        private static String _generalDataPath = Path.Combine(_dataPath, "General");
        private static String _extension = ".mcro";
        private static String _generalDataFilePath = _generalDataPath + "General" + _extension;
        private static String _MCROFilterString = "MCRO Files (*" + _extension + ")|";

        public static String BaseDirectoryPath => _baseDirectoryPath;
        public static String ProjectPath => _projectPath;
        public static String ImagePath => _imagePath;
        public static String GeneralDataPath => _generalDataPath;
        public static String Extension => _extension;
        public static String GeneralDataFilePath => _generalDataFilePath;
        public static String MCROFilterString => _MCROFilterString;
    }
}
