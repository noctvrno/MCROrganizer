using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.Designer;
using MCROrganizer.Core.Utils;
using MCROrganizer.Core.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MCROrganizer.Core
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

        public static void WriteToJSON(Object runData, String filePath = null)
        {
            // Write all of it into the current template path.
            RunTemplate runTemplate = new()
            {
                ApplicationMode = ApplicationSettings.Mode,
                RunsData = new List<DraggableButtonDataContext>(runData as IEnumerable<DraggableButtonDataContext>)
            };

            String jsonString = GetJsonString(runTemplate);

            try
            {
                File.WriteAllText(filePath ?? _currentTemplatePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBoxUtils.ShowError(ex.InnerException?.Message ?? ex.Message);
                return;
            }
        }

        private static String GetJsonString(Object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                Converters = new JsonConverter[] { new StringEnumConverter() }
            });
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

        public Boolean TryLoadData(Boolean browseForFile = true)
        {
            OpenFileDialog fileBrowserDialog = null;
            if (browseForFile)
            {
                fileBrowserDialog = new OpenFileDialog()
                {
                    Filter = PathUtils.MCROFilter,
                    DefaultExt = PathUtils.Extension,
                    AddExtension = true,
                    RestoreDirectory = true,
                };

                if (fileBrowserDialog.ShowDialog() != DialogResult.OK)
                    return false;
            }

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
                return false;
            }

            // Deserialize the template.
            RunTemplate runTemplate = null;
            try
            {
                runTemplate = JsonConvert.DeserializeObject<RunTemplate>(jsonString, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                    Converters = new JsonConverter[] { new StringEnumConverter() }
                });
            }
            catch (Exception ex)
            {
                MessageBoxUtils.ShowError(ex.InnerException?.Message ?? ex.Message);
                return false;
            }

            // Convert the runsData object.
            var runsData = new ObservableCollection<DraggableButtonDataContext>(runTemplate.RunsData);
            if (runsData == null || runsData.Count == 0)
            {
                MessageBoxUtils.ShowError("Loading the runs data failed.");
                return false;
            }

            _managedControl.Runs.Clear();

            // Set the ApplicationMode.
            ApplicationSettings.Mode = runTemplate.ApplicationMode;

            foreach (var runData in runsData)
            {
                _managedControl.Runs.Add(new DraggableButton(_managedControl, runData));
            }

            return true;
        }
    }

    public class RunTemplate
    {
        public ApplicationMode ApplicationMode { get; set; }
        public List<DraggableButtonDataContext> RunsData { get; set; }
    }
}
