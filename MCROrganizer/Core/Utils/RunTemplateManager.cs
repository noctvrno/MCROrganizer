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
        private static String filterString = "MCRO Files (*" + PathUtils.Extension + ")|";
        private String currentTemplatePath = String.Empty;

        public RunTemplateManager(ControlLogic managedControl)
        {
            _managedControl = managedControl;
        }

        public void Save()
        {
            if (currentTemplatePath == String.Empty)
            {
                SaveAs();
                return;
            }

            CreateAndWriteJSONStringToFile();
        }

        private void CreateAndWriteJSONStringToFile()
        {
            // Create a JSON string.
            String jsonString = JsonConvert.SerializeObject
            (
                _managedControl.Runs.Select(x => x.DBDataContext),
                Formatting.Indented,
                new JsonConverter[] { new StringEnumConverter() }
            );

            // Write all of it into the current template path.
            File.WriteAllText(currentTemplatePath, jsonString);
        }

        public void SaveAs()
        {
            SaveFileDialog folderBrowserDialog = new SaveFileDialog()
            {
                RestoreDirectory = false,
                Filter = filterString,
                DefaultExt = PathUtils.Extension,
                AddExtension = true,
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            // Update the path of the current template.
            currentTemplatePath = Path.Combine(folderBrowserDialog.InitialDirectory, folderBrowserDialog.FileName);

            CreateAndWriteJSONStringToFile();
        }

        public ObservableCollection<DraggableButtonDataContext> LoadData(Boolean loadPreviousSessionTemplate = false)
        {
            OpenFileDialog fileBrowserDialog = new OpenFileDialog()
            {
                InitialDirectory = PathUtils.ImagePath,
                Filter = filterString,
                DefaultExt = PathUtils.Extension,
                AddExtension = true
            };

            if (fileBrowserDialog.ShowDialog() != DialogResult.OK)
                return new ObservableCollection<DraggableButtonDataContext>();

            _managedControl.Runs.Clear();

            // Update the path of the current template.
            currentTemplatePath = fileBrowserDialog.FileName;

            // Read the jsonString from the loaded file.
            String jsonString = File.ReadAllText(currentTemplatePath);

            // Actual deserialization.
            var runsData = new ObservableCollection<DraggableButtonDataContext>
            (
                JsonConvert.DeserializeObject<ObservableCollection<DraggableButtonDataContext>>
                (
                    jsonString,
                    new JsonConverter[] { new StringEnumConverter() }
                )
            );

            return runsData;
        }
    }
}
