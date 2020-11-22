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
        private static String _imagePath = Path.Combine(_projectPath, @"Tools\Images\");
        public static String BaseDirectoryPath => _baseDirectoryPath;
        public static String ProjectPath => _projectPath;
        public static String ImagePath => _imagePath;
    }
}
