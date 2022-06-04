using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MCROrganizer.Core.Utils
{
    public class RunTemplateManager
    {
        private ControlLogic _managedControl = null;
        private static String _currentTemplatePath = String.Empty;

        public static String CurrentTemplatePath => _currentTemplatePath;

        public RunTemplateManager(ControlLogic managedControl)
        {
            _managedControl = managedControl;
            _currentTemplatePath = ReadCurrentTemplatePath();
        }

        private String ReadCurrentTemplatePath()
        {
            // Read the jsonString from the general data file.
            String jsonString = String.Empty;
            try
            {
                jsonString = File.ReadAllText(PathUtils.GeneralDataFilePath);
            }
            catch (Exception)
            {
                return String.Empty;
            }

            return JsonConvert.DeserializeObject<String>(jsonString);
        }

        public void Save()
        {
            if (_currentTemplatePath == String.Empty)
            {
                SaveAs();
                return;
            }

            WriteToJSON(_managedControl.Runs.Select(x => x.DBDataContext));
        }

        public static void WriteToJSON(Object serializedData, String filePath = null)
        {
            // Create a JSON string.
            String jsonString = JsonConvert.SerializeObject
            (
                serializedData,
                Formatting.Indented,
                new JsonConverter[] { new StringEnumConverter() }
            );

            // Write all of it into the current template path.
            File.WriteAllText(filePath ?? _currentTemplatePath, jsonString);
        }

        public void SaveAs()
        {
            SaveFileDialog folderBrowserDialog = new SaveFileDialog()
            {
                RestoreDirectory = false,
                Filter = PathUtils.MCROFilter,
                DefaultExt = PathUtils.Extension,
                AddExtension = true,
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            // Update the path of the current template.
            _currentTemplatePath = Path.Combine(folderBrowserDialog.InitialDirectory, folderBrowserDialog.FileName);

            WriteToJSON(_managedControl.Runs.Select(x => x.DBDataContext));
        }

        public Collection<T> LoadData<T, U>(ICollection<U> entitiesToClearOnLoad, Boolean browseForFile = true)
            where T : UserControlDataContext
            where U : System.Windows.Controls.UserControl
        {
            OpenFileDialog fileBrowserDialog = null;
            if (browseForFile)
            {
                fileBrowserDialog = new OpenFileDialog()
                {
                    InitialDirectory = PathUtils.ImagePath,
                    Filter = PathUtils.MCROFilter,
                    DefaultExt = PathUtils.Extension,
                    AddExtension = true
                };

                if (fileBrowserDialog.ShowDialog() != DialogResult.OK)
                    return new Collection<T>();
            }

            entitiesToClearOnLoad.Clear();

            // Update the path of the current template.
            if (fileBrowserDialog?.FileName != null)
                _currentTemplatePath = fileBrowserDialog.FileName;

            // Read the jsonString from the loaded file.
            String jsonString = String.Empty;
            try
            {
                jsonString = File.ReadAllText(_currentTemplatePath);
            }
            catch (FileNotFoundException)
            {
                return new Collection<T>();
            }

            // Actual deserialization.
            var runsData = new ObservableCollection<T>
            (
                JsonConvert.DeserializeObject<ObservableCollection<T>>
                (
                    jsonString,
                    new JsonConverter[] { new StringEnumConverter() }
                )
            );

            return runsData;
        }
    }
}
