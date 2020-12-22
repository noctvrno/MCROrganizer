using MCROrganizer.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MCROrganizer.Core.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            RunTemplateManager.WriteToJSON(RunTemplateManager.CurrentTemplatePath, Path.Combine(PathUtils.GeneralDataPath, "General" + PathUtils.Extension));
            Application.Current.Shutdown();
        }
    }
}
