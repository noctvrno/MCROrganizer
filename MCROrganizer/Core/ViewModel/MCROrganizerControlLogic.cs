using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MCROrganizer.Core.Commands;
using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.Extensions;
using MCROrganizer.Core.Utils;
using MCROrganizer.Core.View;

namespace MCROrganizer.Core.ViewModel
{
    public class ControlLogic : UserControlDataContext
    {
        #region Customization properties
        private Int32 _minimumNumberOfRuns = 2;

        // Width of the main control. (must be deleted)
        private Double _controlWidth = 400.0;
        public Double ControlWidth
        {
            get => _controlWidth;
            set => _controlWidth = value;
        }

        // Specified width for each added run. This setting is controlled using a contextual menu in the main control.
        public Double _specifiedRunWidth = 0.0;
        public Double SpecifiedRunWidth
        {
            get => _specifiedRunWidth;
            set
            {
                _specifiedRunWidth = value;
                UpdateRuns(RunParameter.Width, value);
                NotifyPropertyChanged(nameof(SpecifiedRunWidth));
            }
        }

        // Specified height for each added run. This setting is controlled using a contextual menu in the main control.
        public Double _specifiedRunHeight = 0.0;
        public Double SpecifiedRunHeight
        {
            get => _specifiedRunHeight;
            set
            {
                _specifiedRunHeight = value;
                UpdateRuns(RunParameter.Height, value);
                NotifyPropertyChanged(nameof(SpecifiedRunHeight));
            }
        }

        // Spacing
        private Double _specifiedRunSpacing = 20.0;
        public Double SpecifiedRunSpacing
        {
            get => _specifiedRunSpacing;
            set
            {
                _specifiedRunSpacing = value;
                UpdateRuns(RunParameter.Spacing, value);
                NotifyPropertyChanged(nameof(SpecifiedRunSpacing));
            }
        }

        // Maximum width that one run can have. This is something constant for all runs.
        private Double _runWidthMax = Double.PositiveInfinity;
        public Double RunWidthMax
        {
            get => _runWidthMax;
            set
            {
                _runWidthMax = value;
                NotifyPropertyChanged(nameof(RunWidthMax));
            }
        }

        // Maximum height that one run can have. This is something constant for all runs.
        private Double _runHeightMax = Double.PositiveInfinity;
        public Double RunHeightMax
        {
            get => _runHeightMax;
            set
            {
                _runHeightMax = value;
                NotifyPropertyChanged(nameof(RunHeightMax));
            }
        }

        // Maximum spacing between runs. Must be limited in the future.
        private Double _runSpacingMax = Double.PositiveInfinity;
        public Double RunSpacingMax
        {
            get => _runSpacingMax;
            set
            {
                _runSpacingMax = value;
                NotifyPropertyChanged(nameof(RunSpacingMax));
            }
        }

        // General run properties.
        private Dictionary<RunState, RunProperties> _generalRunProperties = null;

        // Margins for the ItemsControl
        private Thickness _itemsControlMargins = new Thickness(0.0, 10.0, 0.0, 10.0);
        public Thickness ItemsControlMargins => _itemsControlMargins;

        // Width of the ItemsControl
        private Double _itemsControlWidth = 0.0;
        public Double ItemsControlWidth
        {
            get => _itemsControlWidth;
            set
            {
                _itemsControlWidth = value;
                UpdateMaximumRunWidth();
                ComputeAbscissaCases();
                PositionRunsOnScreen(false);
            }
        }

        // Template manager.
        private RunTemplateManager _runTemplateManager = null;
        #endregion

        #region Two-Way Helper DataBinding Properties
        // ItemsSource for the ItemsControl
        private ObservableCollection<DraggableButton> _runs = null;
        public ObservableCollection<DraggableButton> Runs
        {
            get => _runs;
            set => _runs = value;
        }

        // Main HashTable that stores each run and their current abscissa.
        private Dictionary<DraggableButton, Double> _abscissaByRun = null; // Each abscissa entry will be relative to the ItemsControl (the parent of all DraggableButtons).
        public Dictionary<DraggableButton, Double> AbscissaByRun => _abscissaByRun;

        // Secondary HashTable that stores each possible run number scenario next to their respective collection of abscissas.
        private Dictionary<Int32, List<Double>> _abscissaByNumberOfRunsCases = null;

        // Add Run Command.
        private static ImageSource _addRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "AddRun.png")));
        public ImageSource AddRunImage => _addRunImage;
        public ICommand AddRunCommand => new MCROCommand(_ => AddRun());

        // Save Run Template Command.
        private static ImageSource _saveRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "SaveRun.png")));
        public ImageSource SaveRunImage => _saveRunImage;
        public ICommand SaveRunCommand => new MCROCommand(_ => _runTemplateManager.Save());

        // Save Run As Template Command.
        private static ImageSource _saveRunAsImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "SaveRunAs.png")));
        public ImageSource SaveRunAsImage => _saveRunAsImage;
        public ICommand SaveRunAsCommand => new MCROCommand(_ => _runTemplateManager.SaveAs());

        // Load Run Template Command.
        private static ImageSource _loadRunImage = new BitmapImage(new Uri(Path.Combine(PathUtils.ImagePath, "LoadRun.png")));
        public ImageSource LoadRunImage => _loadRunImage;
        public ICommand LoadRunCommand => new MCROCommand(_ => LoadRunTemplate());

        // View object.
        private MainControl _userControl = null;
        public MainControl MainControl => _userControl;

        // Current run.
        private DraggableButtonDataContext _runInProgress = null;
        public DraggableButtonDataContext RunInProgress
        {
            get => _runInProgress;
            set
            {
                _runInProgress = value;
                NotifyPropertyChanged(nameof(RunInProgress));
                NotifyPropertyChanged(nameof(IsCurrentRunLogoSet));
            }
        }

        public Boolean IsCurrentRunLogoSet => _runInProgress?.RunLogo != null;
        #endregion

        #region Initialization
        public ControlLogic(MainControl userControl)
        {
            _userControl = userControl;
            _runTemplateManager = new RunTemplateManager(this);
            _runs = new ObservableCollection<DraggableButton>();
            _generalRunProperties = new Dictionary<RunState, RunProperties>();
            if (RunTemplateManager.CurrentTemplatePath != String.Empty)
                LoadRunTemplate(false);
            else
            {
                InitializeRuns();
                ComputeAbscissaCases();
            }

            SpecifiedRunWidth = _runInProgress.Width;
            SpecifiedRunHeight = _runInProgress.Height;
            SpecifiedRunSpacing = _runInProgress.Spacing;
        }
        #endregion

        #region Functionality
        // This method translates the control horizontally.
        public void TranslateItemHorizontally(DraggableButton itemToTranslate, Double abscissaValue)
        {
            Canvas.SetLeft(itemToTranslate, abscissaValue);
        }

        // This method moves the collidedItem to the draggedItem position and updates the dictionary according to the swap (also swaps the entries in the ItemsSource).
        public void SwapDraggedItemOnCollision(DraggableButton draggedItem, DraggableButton collidedItem)
        {
            // Update the dictionary so that the draggedItem has the collidedItem's position.
            if (AbscissaByRun.TryGetValue(draggedItem, out Double draggedItemAbscissaValue) && AbscissaByRun.TryGetValue(collidedItem, out Double collidedItemAbscissaValue))
            {
                // Physically move the collideditem to the draggedItem standard position.
                TranslateItemHorizontally(collidedItem, draggedItemAbscissaValue);

                // Swap the values of the draggedItem and the collidedItem.
                AbscissaByRun[draggedItem] = collidedItemAbscissaValue;
                AbscissaByRun[collidedItem] = draggedItemAbscissaValue;

                SetAllRunsAsPending();

                // Swap the items in the observable collection as well. Due to the nature of this data structure, we cannot simply do a swap here.
                // The Move method actually removes the entry and then re-inserts it (a bit of overhead).
                _runs.Move(_runs.IndexOf(draggedItem), _runs.IndexOf(collidedItem));
            }
        }

        public void SetRunAsCurrent(DraggableButtonDataContext selectedRun)
        {
            if (selectedRun == null)
                return;

            RunInProgress = selectedRun;
            Int32 selectedRunIndex = _runs.IndexOf(selectedRun.Control);
            selectedRun.Properties = _generalRunProperties[RunState.InProgress];

            for (Int32 runIndex = 0; runIndex < _runs.Count; ++runIndex)
            {
                // Skip the selected run (already handled).
                if (runIndex == selectedRunIndex)
                    continue;

                // Runs to the left of the current one will be considered finished and the ones to the right are considered pending.
                _runs[runIndex].DBDataContext.Properties = runIndex < selectedRunIndex ? _generalRunProperties[RunState.Finished] : _generalRunProperties[RunState.Pending];
            }
        }

        public void SetAllRunsAsPending()
        {
            // Something more optimized could be written here, obviously, but LINQ keeps it more compact and mroe readable.
            Int32 lastRunPendingIndex = _runs.IndexOf(_runs.LastOrDefault(x => x.DBDataContext.Properties.State != RunState.Pending));
            if (lastRunPendingIndex == -1)
                return;

            // Only go through runs we know for sure that are not pending.
            for (Int32 iRun = 0; iRun <= lastRunPendingIndex; ++iRun)
            {
                _runs[iRun].DBDataContext.Properties = _generalRunProperties[RunState.Pending];
            }
        }

        private void AddRun()
        {
            // Check if the new number of runs is okay.
            if (!_abscissaByNumberOfRunsCases.ContainsKey(_runs.Count + 1))
                return;

            // After adding a run we need to update the properties and the data structures.
            var newRun = new DraggableButton(this);
            newRun.DBDataContext.Width = _specifiedRunWidth;
            newRun.DBDataContext.Height = _specifiedRunHeight;
            newRun.DBDataContext.Properties = _generalRunProperties[RunState.Pending]; // A new run is always added as the last run and, therefore, will always be pending.
            _runs.Add(newRun);
            UpdateAbscissasAndContainers();
            UpdateMaximumRunWidth();
        }

        public void RemoveRun(DraggableButton deletedRun)
        {
            // Check if the new number of runs is okay.
            if (!_abscissaByNumberOfRunsCases.ContainsKey(_runs.Count - 1))
                return;

            // After removing a run we need to update the abscissas and the data structures.
            _runs.Remove(deletedRun);
            _abscissaByRun.Remove(deletedRun);
            UpdateAbscissasAndContainers();
            UpdateMaximumRunWidth();
        }

        /// <summary>
        /// This method searches for the current number of runs, if this number is registered in the dictionary, then it will retrieve
        /// the list of abscissas the new runs are supposed to contain and it will update each abscissa.
        /// </summary>
        public void UpdateAbscissasAndContainers()
        {
            if (_abscissaByNumberOfRunsCases.TryGetValue(_runs.Count, out var abscissas))
            {
                for (Int32 iRun = 0; iRun < _runs.Count; ++iRun)
                {
                    Double currentAbscissa = abscissas[iRun];
                    DraggableButton currentRun = _runs[iRun];

                    // Translate the item accordingly using the new abscissa value.
                    TranslateItemHorizontally(currentRun, currentAbscissa);

                    // Modify the abscissa of the run with the new value.
                    if (!_abscissaByRun.ContainsKey(currentRun))
                        _abscissaByRun.Add(currentRun, currentAbscissa);
                    else
                        _abscissaByRun[currentRun] = currentAbscissa;
                }
            }
        }

        /// <summary>
        /// Initializes a default number of runs (two) and aligns them to the center of the screen.
        /// This method should be called when the user creates a default template.
        /// </summary>
        private void InitializeRuns()
        {
            RunProperties pendingRunProperties = _generalRunProperties[RunState.Pending] = new RunProperties(RunState.Pending);
            RunProperties inProgressRunProperties = _generalRunProperties[RunState.InProgress] = new RunProperties(RunState.InProgress);
            _generalRunProperties[RunState.Finished] = new RunProperties(RunState.Finished);
            if (_runs.Count == 0)
            {
                // Initialize default runs.
                _runs = new ObservableCollection<DraggableButton>()
                {
                    new DraggableButton(this, properties: inProgressRunProperties), // Default in progress run.
                    new DraggableButton(this, properties: pendingRunProperties), // Default pending run.
                };
            }
            else
            {
                // Initialize general runProperties.
                // If we cannot find an associated run with a certain RunState, then skip it.
                foreach (var run in _runs)
                {
                    switch (run.DBDataContext.Properties.State)
                    {
                        case RunState.Pending:
                            _generalRunProperties[RunState.Pending] = run.DBDataContext.Properties;
                            break;
                        case RunState.InProgress:
                            _generalRunProperties[RunState.InProgress] = run.DBDataContext.Properties;
                            break;
                        case RunState.Finished:
                            _generalRunProperties[RunState.Finished] = run.DBDataContext.Properties;
                            break;
                    }
                }
            }

            SetRunAsCurrent(_runs?.FirstOrDefault(x => x.DBDataContext.Properties.State == RunState.InProgress)?.DBDataContext);
            _abscissaByRun = new Dictionary<DraggableButton, Double>();

            // Compute the actual width of the ItemsControl (where the buttons will be placed).
            ItemsControlWidth = _controlWidth - _itemsControlMargins.Left - _itemsControlMargins.Right;
            PositionRunsOnScreen();
        }

        private void PositionRunsOnScreen(Boolean isInitializationPhase = true)
        {
            Double runWidth = RunInProgress.Width;
            Double nextPivotPoint = _specifiedRunSpacing + runWidth;

            // Compute the start abscissa of the runs.
            Double startAbscissa = (_itemsControlWidth - (runWidth * _runs.Count + _specifiedRunSpacing * (_runs.Count - 1))) / 2.0;

            for (Int32 runIndex = 0; runIndex < _runs.Count; ++runIndex)
            {
                DraggableButton run = _runs[runIndex];
                Double abscissa = startAbscissa + nextPivotPoint * runIndex;
                if (isInitializationPhase)
                {
                    _abscissaByRun.Add(run, abscissa);
                    TranslateItemHorizontally(run, abscissa);
                    continue;
                }

                if (_abscissaByRun.ContainsKey(run))
                {
                    TranslateItemHorizontally(run, abscissa);
                    _abscissaByRun[run] = abscissa;
                }
            }
        }

        private void ComputeAbscissaCases()
        {
            _abscissaByNumberOfRunsCases = new Dictionary<Int32, List<Double>>();
            Double runWidth = RunInProgress.Width;
            Int32 maximumNumberOfRuns = (Int32)Math.Floor(_itemsControlWidth / (runWidth + _specifiedRunSpacing));

            for (Int32 iRunCase = _minimumNumberOfRuns; iRunCase <= maximumNumberOfRuns; ++iRunCase)
            {
                Double startAbscissa = (_itemsControlWidth - (runWidth * iRunCase + _specifiedRunSpacing * (iRunCase - 1))) / 2.0;
                var abscissas = new List<Double> { startAbscissa };

                for (Int32 iRun = 0; iRun < iRunCase; ++iRun)
                {
                    Double nextPivotPoint = (_specifiedRunSpacing + runWidth) * iRun;

                    if (!_abscissaByNumberOfRunsCases.ContainsKey(iRunCase))
                        _abscissaByNumberOfRunsCases.Add(iRunCase, abscissas);
                    else
                        abscissas.Add(startAbscissa + nextPivotPoint);
                }
            }
        }

        private void LoadRunTemplate(Boolean browseForFile = true)
        {
            var deserializedRunsData = new ObservableCollection<DraggableButtonDataContext>(_runTemplateManager.LoadData<DraggableButtonDataContext, DraggableButton>(_runs, browseForFile));

            foreach (var runData in deserializedRunsData)
            {
                _runs.Add(new DraggableButton(this, runData));
            }

            InitializeRuns();
            ComputeAbscissaCases();
        }

        public void DesignRun(RunState runState, CustomizableRunElements customizableRunElements)
        {
            RunProperties pickedGeneralRunProperties = _generalRunProperties[runState];
            Int32 customColor = 0;
            System.Drawing.Color selectedColor = new();

            SolidColorBrush choiceColorBrush = customizableRunElements switch
            {
                CustomizableRunElements.Border => pickedGeneralRunProperties.BorderColor,
                CustomizableRunElements.Background => pickedGeneralRunProperties.BackgroundColor,
                CustomizableRunElements.Font => pickedGeneralRunProperties.FontColor,
                CustomizableRunElements _ => throw new NotSupportedException()
            };

            customColor = BitConverter.ToInt32(new byte[] { choiceColorBrush.Color.R, choiceColorBrush.Color.G, choiceColorBrush.Color.B, 0 }, 0);
            selectedColor = System.Drawing.Color.FromArgb(choiceColorBrush.Color.A, choiceColorBrush.Color.R, choiceColorBrush.Color.G, choiceColorBrush.Color.B);

            ColorDialog colorDialog = new ColorDialog()
            {
                AllowFullOpen = true,
                FullOpen = true,
                Color = selectedColor,
                CustomColors = new Int32[] { customColor }
            };

            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;

            switch (customizableRunElements)
            {
                case CustomizableRunElements.Border:
                    pickedGeneralRunProperties.BorderColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    break;
                case CustomizableRunElements.Background:
                    pickedGeneralRunProperties.BackgroundColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    break;
                case CustomizableRunElements.Font:
                    pickedGeneralRunProperties.FontColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    break;
            }

            foreach (var matchedRun in _runs.Select(x => x.DBDataContext).Where(x => x.Properties.State == runState))
            {
                matchedRun.Properties = pickedGeneralRunProperties;
            }
        }

        private void UpdateRuns(RunParameter updatedRunParameter, Double value)
        {
            IEnumerable<DraggableButtonDataContext> runsData = _runs.Select(x => x.DBDataContext);
            switch (updatedRunParameter)
            {
                case RunParameter.Width:
                    foreach (var runData in runsData)
                    {
                        runData.Width = value;
                    }
                    PositionRunsOnScreen(false);
                    ComputeAbscissaCases();
                    break;
                case RunParameter.Height:
                    foreach (var runData in runsData)
                    {
                        runData.Height = value;
                    }
                    break;
                case RunParameter.Spacing:
                    foreach (var runData in runsData)
                    {
                        runData.Spacing = value;
                    }
                    PositionRunsOnScreen(false);
                    ComputeAbscissaCases();
                    break;
            }
        }

        private void UpdateMaximumRunWidth()
        {
            DraggableButtonDataContext firstRunData = _runs.FirstOrDefault().DBDataContext;
            DraggableButtonDataContext lastRunData = _runs.LastOrDefault().DBDataContext;
            if (AbscissaByRun.TryGetValue(firstRunData.Control, out Double firstRunAbscissa) &&
                AbscissaByRun.TryGetValue(lastRunData.Control, out Double lastRunAbscissa))
            {
                // Compute the remaining available space to the left and to the right of the runs.
                Double remainingSpace = firstRunAbscissa + (_itemsControlWidth - lastRunAbscissa);

                // Compute the max possible run width by adding the remaining space to any run width and distribute it to each run.
                RunWidthMax = (firstRunData.Width + remainingSpace) / _runs.Count;

                if (!ItemsControlWidth.IsInRange(RunWidthMax * _runs.Count + _specifiedRunSpacing))
                    System.Diagnostics.Debug.WriteLine("Something went wrong with the max run width calculation");
            }
        }
        #endregion
    }
}
