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
        private ITourPlannerFactory tourPlannerFactory;
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

        public TourDetailViewModel(IDialogService dialogService, ITourPlannerFactory tourPlannerFactory)
        {
            this.dialogService = dialogService;
            this.tourPlannerFactory = tourPlannerFactory;
            AddLogCommand = new RelayCommand(
                async (_) => {
                    TourLog addTourLog = new TourLog() { TourId = SelectedTour.Id };
                    CreateUpdateTourLogViewModel createUpdateTourLogViewModel = new CreateUpdateTourLogViewModel(addTourLog);
                    bool? result = dialogService.ShowDialog(createUpdateTourLogViewModel);
                    if (result ?? false)
                    {
                        addTourLog.TotalTime = (addTourLog.EndDate - addTourLog.StartDate).TotalHours;
                        addTourLog.AverageSpeed = addTourLog.Distance / addTourLog.TotalTime;
                        await tourPlannerFactory.CreateTourLog(addTourLog);
                        SelectedTour.TourLogs.Add(addTourLog);
                        CurrentTourLogs.Add(addTourLog);
                    }
                },
                (_) => {
                    return SelectedTour != null ? true : false;
                });
            RemoveLogCommand = new RelayCommand(
                async (_) => {
                    await tourPlannerFactory.DeleteTourLog(SelectedTourLog);
                    SelectedTour.TourLogs.Remove(SelectedTourLog);
                    CurrentTourLogs.Remove(SelectedTourLog);
                },
                (_) => {
                    return SelectedTourLog != null ? true : false;
                });
            EditLogCommand = new RelayCommand(
                async (_) => {
                    TourLog editTourLog = new TourLog() { Id = SelectedTourLog.Id, TourId = SelectedTourLog.TourId, StartDate = SelectedTourLog.StartDate, EndDate = SelectedTourLog.EndDate, Distance = SelectedTourLog.Distance, Temperature = SelectedTourLog.Temperature, Weather = SelectedTourLog.Weather, TravelMethod = SelectedTourLog.TravelMethod, Rating = SelectedTourLog.Rating, Report = SelectedTourLog.Report };
                    CreateUpdateTourLogViewModel createUpdateTourLogViewModel = new CreateUpdateTourLogViewModel(editTourLog);
                    bool? result = dialogService.ShowDialog(createUpdateTourLogViewModel);
                    if (result ?? false)
                    {
                        editTourLog.TotalTime = (editTourLog.EndDate - editTourLog.StartDate).TotalHours;
                        editTourLog.AverageSpeed = editTourLog.Distance / editTourLog.TotalTime;
                        await tourPlannerFactory.UpdateTourLog(editTourLog);
                        SelectedTour.TourLogs.Remove(SelectedTourLog);
                        SelectedTour.TourLogs.Add(editTourLog);
                        CurrentTourLogs.Remove(SelectedTourLog);
                        CurrentTourLogs.Add(editTourLog);
                        SelectedTourLog = editTourLog;
                    }
                },
                (_) => {
                    return (SelectedTourLog != null) ? true : false;
                });
        }
    }
}
