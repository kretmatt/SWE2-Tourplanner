using BusinessLogicLayer.Factories;
using Common.Entities;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    /// <summary>
    /// TourDetailViewModel provides a Tour and commands for managing its TourLogs
    /// </summary>
    public class TourDetailViewModel:BaseViewModel
    {
        /// <summary>
        /// Currently selected Tour
        /// </summary>
        private Tour _selectedTour;
        /// <summary>
        /// Currently selected TourLog
        /// </summary>
        private TourLog selectedTourLog;
        /// <summary>
        /// TourLogs of the currently selected tour
        /// </summary>
        private ObservableCollection<TourLog> currentTourLogs;
        /// <summary>
        /// Object used for creating, updating and deleting tourlogs
        /// </summary>
        private ITourLogFactory tourLogFactory;
        /// <summary>
        /// Service used for opening TourLog related Dialogs
        /// </summary>
        private readonly IDialogService dialogService;
        /// <value>
        /// TourLogs of the currently selected tour
        /// </value>
        public ObservableCollection<TourLog> CurrentTourLogs
        {
            get { return currentTourLogs; }
            set
            {
                if (value != currentTourLogs)
                {
                    currentTourLogs = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Currently selected tour
        /// </value>
        public Tour SelectedTour
        {
            get { return _selectedTour; }
            set
            {
                if (value != _selectedTour)
                {
                    _selectedTour = value;
                    OnPropertyChanged();

                    CurrentTourLogs = _selectedTour != null ? new ObservableCollection<TourLog>(_selectedTour.TourLogs) : new ObservableCollection<TourLog>();
                }
            }
        }
        /// <value>
        /// Currently selected tourlog
        /// </value>
        public TourLog SelectedTourLog
        {
            get { return selectedTourLog; }
            set
            {
                if (value != selectedTourLog)
                {
                    selectedTourLog = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Command for adding new TourLogs to the currently selected tour
        /// </value>
        public ICommand AddLogCommand { get; }
        /// <value>
        /// Command for deleting TourLogs from the currently selected tour
        /// </value>
        public ICommand RemoveLogCommand { get; }
        /// <value>
        /// Command for updating currently selected tourlog
        /// </value>
        public ICommand EditLogCommand { get; }
        /// <summary>
        /// Helper method for displayin exceptions / errors
        /// </summary>
        /// <param name="e">Thrown exception</param>
        private void ShowException(Exception e)
        {
            ErrorViewModel evm = new ErrorViewModel(e.Message, e.GetType().ToString());
            dialogService.ShowDialog(evm);
        }
        /// <summary>
        /// Default constructor of TourDetailViewModel
        /// </summary>
        /// <param name="dialogService">Service used for opening TourLog related Dialogs</param>
        public TourDetailViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.tourLogFactory = new TourLogFactory();
            AddLogCommand = new RelayCommand(
                async (_) => {
                    try
                    {
                        TourLog addTourLog = new TourLog() { TourId = SelectedTour.Id };
                        CreateUpdateTourLogViewModel createUpdateTourLogViewModel = new CreateUpdateTourLogViewModel(addTourLog);
                        bool? result = dialogService.ShowDialog(createUpdateTourLogViewModel);
                        if (result ?? false)
                        {
                            addTourLog.TotalTime = (addTourLog.EndDate - addTourLog.StartDate).TotalHours;
                            addTourLog.AverageSpeed = addTourLog.Distance / addTourLog.TotalTime;
                            await tourLogFactory.CreateTourLog(addTourLog);
                            SelectedTour.TourLogs.Add(addTourLog);
                            CurrentTourLogs.Add(addTourLog);
                        }
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                    }
                    
                },
                (_) => {
                    return SelectedTour != null ? true : false;
                });
            RemoveLogCommand = new RelayCommand(
                async (_) => {
                    try
                    {
                        await tourLogFactory.DeleteTourLog(SelectedTourLog);
                        SelectedTour.TourLogs.Remove(SelectedTourLog);
                        CurrentTourLogs.Remove(SelectedTourLog);
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                    }  
                },
                (_) => {
                    return SelectedTourLog != null ? true : false;
                });
            EditLogCommand = new RelayCommand(
                async (_) => {
                    try
                    {
                        TourLog editTourLog = new TourLog() { Id = SelectedTourLog.Id, TourId = SelectedTourLog.TourId, StartDate = SelectedTourLog.StartDate, EndDate = SelectedTourLog.EndDate, Distance = SelectedTourLog.Distance, Temperature = SelectedTourLog.Temperature, Weather = SelectedTourLog.Weather, TravelMethod = SelectedTourLog.TravelMethod, Rating = SelectedTourLog.Rating, Report = SelectedTourLog.Report };
                        CreateUpdateTourLogViewModel createUpdateTourLogViewModel = new CreateUpdateTourLogViewModel(editTourLog);
                        bool? result = dialogService.ShowDialog(createUpdateTourLogViewModel);
                        if (result ?? false)
                        {
                            editTourLog.TotalTime = (editTourLog.EndDate - editTourLog.StartDate).TotalHours;
                            editTourLog.AverageSpeed = editTourLog.Distance / editTourLog.TotalTime;
                            await tourLogFactory.UpdateTourLog(editTourLog);
                            SelectedTour.TourLogs.Remove(SelectedTourLog);
                            SelectedTour.TourLogs.Add(editTourLog);
                            CurrentTourLogs.Remove(SelectedTourLog);
                            CurrentTourLogs.Add(editTourLog);
                            SelectedTourLog = editTourLog;
                        }
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                    }
                },
                (_) => {
                    return (SelectedTourLog != null) ? true : false;
                });
        }
    }
}
