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
    public class TourDetailViewModel:BaseViewModel
    {
        private Tour _selectedTour;
        private TourLog selectedTourLog;
        private ObservableCollection<TourLog> currentTourLogs;
        private ITourLogFactory tourLogFactory;
        private readonly IDialogService dialogService;
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
        public ICommand AddLogCommand { get; }
        public ICommand RemoveLogCommand { get; }
        public ICommand EditLogCommand { get; }

        private void ShowException(Exception e)
        {
            ErrorViewModel evm = new ErrorViewModel(e.Message, e.GetType().ToString());
            dialogService.ShowDialog(evm);
        }

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
