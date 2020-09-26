﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MCROrganizer.Core.CustomControls;

namespace MCROrganizer.Core.View
{
    /// <summary>
    /// Interaction logic for MCROrganizerControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            DataContext = new MCROrganizer.Core.ViewModel.ControlLogic(this);
            InitializeComponent();
        }
    }
}
