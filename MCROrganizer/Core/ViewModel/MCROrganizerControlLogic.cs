using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MCROrganizer.Core.Commands;
using MCROrganizer.Core.CustomControls;
using MCROrganizer.Core.Utils;
using MCROrganizer.Core.View;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MCROrganizer.Core.ViewModel
{
    public enum RunState
    {
        Pending,
        InProgress,
        Finished
    }

    public class ControlLogic : UserControlDataContext
    {
        #region Customization properties
        private Int32 _minimumNumberOfRuns = 2;

        // Width of the main control.
        private Double _controlWidth = 400.0;
        public Double ControlWidth
        {
            get => _controlWidth;
            set => _controlWidth = value;
        }

        // Height of the main control.
        private Double _controlHeight = 800.0;
        public Double ControlHeight
        {
            get => _controlHeight;
            set => _controlHeight = value;
        }

        // Margins for the ItemsControl
        private Thickness _itemsControlMargins = new Thickness(20.0, 20.0, 20.0, 10.0);
        public Thickness ItemsControlMargins => _itemsControlMargins;

        // Width of the ItemsControl
        private Double _itemsControlWidth = 0.0;
        public Double ItemsControlWidth => _itemsControlWidth;

        // Spacing
        private Double _spacingBetweenRuns = 20.0;
        #endregion

        #region Two-Way Helper DataBinding Properties
        // ItemsSource for the ItemsControl
        private ObservableCollection<DraggableButton> _runs = null;
        public ObservableCollection<DraggableButton> Runs
        {
            get => _runs;
            set => _runs = value;
        }

        // Main Hash Table that stores each run and their current abscissa.
        private Dictionary<DraggableButton, Double> _abscissaByRun = null; // Each abscissa entry will be relative to the ItemsControl (the parent of all DraggableButtons).
        public Dictionary<DraggableButton, Double> AbscissaByRun => _abscissaByRun;

        // Secondary Hash Table that stores each possible run number scenario next to their respective collection of abscissas.
        private Dictionary<Int32, List<Double>> _abscissaByNumberOfRunsCases = null;

        // Add Run Command.
        private static ImageSource _addRunImage = new BitmapImage(new Uri(PathUtils.ImagePath + "AddRun.png"));
        public ImageSource AddRunImage => _addRunImage;
        public ICommand AddRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => AddRun()));

        // Save Run Template Command.
        private static ImageSource _saveRunImage = new BitmapImage(new Uri(PathUtils.ImagePath + "SaveRun.png"));
        public ImageSource SaveRunImage => _saveRunImage;
        public ICommand SaveRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => _runTemplateManager.SaveAs()));

        // Save Run As Template Command.
        private static ImageSource _saveRunAsImage = new BitmapImage(new Uri(PathUtils.ImagePath + "SaveRunAs.png"));
        public ImageSource SaveRunAsImage => _saveRunAsImage;
        public ICommand SaveRunAsCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => _runTemplateManager.SaveAs()));

        // Save Run As Template Command.
        private static ImageSource _loadRunImage = new BitmapImage(new Uri(PathUtils.ImagePath + "LoadRun.png"));
        public ImageSource LoadRunImage => _loadRunImage;
        public ICommand LoadRunCommand => new MCROCommand(new Predicate<object>(obj => true), new Action<object>(obj => LoadRunTemplate()));

        // View object.
        private MainControl _userControl = null;
        public MainControl MainControl => _userControl;

        // Template manager.
        private RunTemplateManager _runTemplateManager = null;
        #endregion

        #region Initialization
        public ControlLogic(MainControl userControl)
        {
            _userControl = userControl;
            _runTemplateManager = new RunTemplateManager(this);
            InitializeDefaultRuns(ref _runs, ref _abscissaByRun);
            ComputeAbscissaCases(ref _abscissaByNumberOfRunsCases);
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

                // Swap the items in the observable collection as well. Due to the nature of this data structure, we cannot simply do a swap here.
                // The Move method actually removes the entry and then re-inserts it (a bit of overhead).
                _runs.Move(_runs.IndexOf(draggedItem), _runs.IndexOf(collidedItem));

                SetAllRunsAsPending();
            }
        }

        public void SetRunAsCurrent(DraggableButton selectedRun)
        {
            Int32 selectedRunIndex = _runs.IndexOf(selectedRun);
            selectedRun.DBDataContext.RunState = RunState.InProgress;

            for (Int32 runIndex = 0; runIndex < _runs.Count; ++runIndex)
            {
                // Skip the selected run (already handled).
                if (runIndex == selectedRunIndex)
                    continue;

                // Runs to the left of the current one will be considered finished and the ones to left are considered pending.
                _runs[runIndex].DBDataContext.RunState = runIndex < selectedRunIndex ? RunState.Finished : RunState.Pending;
            }
        }

        public void SetAllRunsAsPending()
        {
            // Firstly, find a run that is not pending.
            if (_runs.Any(x => x.DBDataContext.RunState != RunState.Pending))
            {
                // Secondly, if we found one, then go through all of them and set them as pending until the user decides on a current run.
                foreach (var run in _runs.Select(x => x.DBDataContext))
                {
                    run.RunState = RunState.Pending;
                }
            }
        }

        private void AddRun()
        {
            // Check if the new number of runs is okay.
            if (!_abscissaByNumberOfRunsCases.ContainsKey(_runs.Count + 1))
                return;

            // After adding a run we need to update the abscissas and the data structures.
            _runs.Add(new DraggableButton(this));
            UpdateAbscissasAndContainers();
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
        private void InitializeDefaultRuns(ref ObservableCollection<DraggableButton> runs, ref Dictionary<DraggableButton, Double> abscissaByRun, Boolean isDefaultTemplate = true)
        {
            if (isDefaultTemplate)
            {
                runs = new ObservableCollection<DraggableButton>()
                {
                    new DraggableButton(this),
                    new DraggableButton(this)
                };
            }

            abscissaByRun = new Dictionary<DraggableButton, Double>();

            // Compute the actual width of the ItemsControl (where the buttons will be placed).
            _itemsControlWidth = _controlWidth - _itemsControlMargins.Left - _itemsControlMargins.Right;
            NotifyPropertyChanged("ItemsControlWidth");

            Double runWidth = runs.FirstOrDefault().DBDataContext.Width;
            Double nextPivotPoint = _spacingBetweenRuns + runWidth;

            // Compute the start abscissa of the runs 
            Double startAbscissa = (_itemsControlWidth - (runWidth * runs.Count + _spacingBetweenRuns * (runs.Count - 1))) / 2.0;

            foreach (var run in runs)
            {
                abscissaByRun.Add(run, startAbscissa + nextPivotPoint * abscissaByRun.Count);
                TranslateItemHorizontally(run, startAbscissa + nextPivotPoint * (abscissaByRun.Count - 1));
            }
        }

        private void ComputeAbscissaCases(ref Dictionary<Int32, List<Double>> abscissaByNumberOfRunsCases)
        {
            abscissaByNumberOfRunsCases = new Dictionary<Int32, List<Double>>();
            Double runWidth = _runs.FirstOrDefault().DBDataContext.Width;
            Int32 maximumNumberOfRuns = (Int32)Math.Floor(_itemsControlWidth / (runWidth + _spacingBetweenRuns));

            for (Int32 iRunCase = _minimumNumberOfRuns; iRunCase <= maximumNumberOfRuns; ++iRunCase)
            {
                Double startAbscissa = (_itemsControlWidth - (runWidth * iRunCase + _spacingBetweenRuns * (iRunCase - 1))) / 2.0;
                var abscissas = new List<Double> { startAbscissa };

                for (Int32 iRun = 0; iRun < iRunCase; ++iRun)
                {
                    Double nextPivotPoint = (_spacingBetweenRuns + runWidth) * iRun;

                    if (!abscissaByNumberOfRunsCases.ContainsKey(iRunCase))
                        abscissaByNumberOfRunsCases.Add(iRunCase, abscissas);
                    else
                        abscissas.Add(startAbscissa + nextPivotPoint);
                }
            }
        }

        private void LoadRunTemplate()
        {
            _runs.Clear();
            ObservableCollection<DraggableButtonDataContext> deserializedRunsData = _runTemplateManager.LoadData();

            foreach (var runData in deserializedRunsData)
            {
                _runs.Add(new DraggableButton(this, runData));
            }

            InitializeDefaultRuns(ref _runs, ref _abscissaByRun, false);
            ComputeAbscissaCases(ref _abscissaByNumberOfRunsCases);
        }
        #endregion
    }
}
