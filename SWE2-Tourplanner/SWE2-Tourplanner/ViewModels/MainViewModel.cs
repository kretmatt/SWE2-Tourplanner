using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLogicLayer.ExporterImporter;
using BusinessLogicLayer.Factories;
using BusinessLogicLayer.PDFCreation;
using Common.Config;
using Common.Entities;
using Common.Logging;
using log4net;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using SWE2_Tourplanner.ViewModels;
using Common;
namespace SWE2_Tourplanner
{
    public class MainViewModel:BaseViewModel
    {
        private TourPlannerFactory tourPlannerFactory;
        private readonly IDialogService dialogService;
        private ObservableCollection<Tour> _tours = new ObservableCollection<Tour>();
        private Tour _selectedTour;
        private TourLog selectedTourLog;
        private ITourPlannerReportsGenerator pdfGenerator;
        private IExporterImporter<Tour> exporterImporter;
        private string searchString;
        private ObservableCollection<Tour> filteredTours;
        private ObservableCollection<TourLog> currentTourLogs;
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
        public ObservableCollection<Tour> FilteredTours
        {
            get { return filteredTours; }
            set
            {
                if (value != filteredTours)
                {
                    filteredTours = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SearchString
        {
            get { return searchString; }
            set
            {
                if (value != searchString)
                {
                    searchString = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Tour> Tours
        {
            get
            {
                return _tours;
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
        public ICommand SearchToursCommand { get; }
        public ICommand AddTourCommand { get; }
        public ICommand RemoveTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand AddLogCommand { get; }
        public ICommand RemoveLogCommand { get; }
        public ICommand EditLogCommand { get; }

        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand GenerateJSONExportCommand { get; }
        public ICommand ConductImportCommand { get; }


        public MainViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            tourPlannerFactory = new TourPlannerFactory();
            pdfGenerator = new TourPlannerReportsGenerator();
            exporterImporter = new TourExporterImporter();
            filteredTours = new ObservableCollection<Tour>();
            tourPlannerFactory.GetTours().ForEach(t=>_tours.Add(t));
            FilteredTours = Tours;

            GenerateTourReportCommand = new RelayCommand(
                async (_) => await pdfGenerator.GenerateTourReport(SelectedTour),
                (_) =>
                {
                    return SelectedTour != null ? true : false;
                }
            );

            GenerateSummaryReportCommand = new RelayCommand(
                    async(object selectedItems) => 
                    {
                        System.Collections.IList items = (System.Collections.IList)selectedItems;
                        List<Tour> selectedTours = items.Cast<Tour>().ToList();
                        await pdfGenerator.GenerateSummaryReport(selectedTours);
                    },
                    (object selectedItems) =>
                    {
                        System.Collections.IList items = (System.Collections.IList)selectedItems;
                        List<Tour>selectedTours = items.Cast<Tour>().ToList();
                        return (selectedTours).Count>1 ? true : false;
                    }
            );

            GenerateJSONExportCommand = new RelayCommand(
                    async (object selectedItems) =>
                    {
                        _selectedTour = null;
                        System.Collections.IList items = (System.Collections.IList)selectedItems;
                        List<Tour> selectedTours = items.Cast<Tour>().ToList();
                        await exporterImporter.Export(selectedTours);
                    },
                    (object selectedItems) =>
                    {
                        System.Collections.IList items = (System.Collections.IList)selectedItems;
                        List<Tour> selectedTours = items.Cast<Tour>().ToList();
                        return (selectedTours).Any() ? true : false;
                    }
            );

            ConductImportCommand = new RelayCommand(
                async (_)=> {
                    ImportViewModel importViewModel = new ImportViewModel(TourPlannerConfig.GetTourPlannerConfig());
                    bool? result = dialogService.ShowDialog(importViewModel);                    
                    if (result ?? false)
                    {
                        foreach(string path in importViewModel.JsonPaths)
                        {
                            foreach(Tour t in await exporterImporter.Import(path))
                            {
                                Tours.Add(t);
                            }
                        }
                    }
                },
                (_) => { return true; }
            );

            SearchToursCommand = new RelayCommand(
                (_) => {
                    if (!string.IsNullOrWhiteSpace(SearchString))
                    {
                        FilteredTours = new ObservableCollection<Tour>(Tours.Where(t =>
                        t.Name.CIContains(SearchString) || t.Description.CIContains(SearchString) ||
                        t.Maneuvers.Any(m => m.Narrative.CIContains(SearchString)) ||
                        t.TourLogs.Any(tl => tl.Report.CIContains(SearchString) || tl.Weather.ToString().CIContains(SearchString) || tl.TravelMethod.ToString().CIContains(SearchString))).ToList());
                    }
                    else
                    {
                        FilteredTours = Tours;
                    }
                },
                (_) => {
                    return true;
                });
            AddTourCommand = new RelayCommand(
                async (_) => {
                    Tour addTour = new Tour();
                    CreateUpdateTourViewModel createUpdateTourViewModel = new CreateUpdateTourViewModel(addTour);
                    bool? result = dialogService.ShowDialog(createUpdateTourViewModel);
                    if (result ?? false)
                    {
                        if (createUpdateTourViewModel.ManualTour == false)
                        {
                            if(!(addTour.Distance<0 && addTour.Maneuvers.Count==0 && string.IsNullOrEmpty(addTour.RouteInfo)))
                            {
                                await tourPlannerFactory.CreateMapQuestTour(addTour);
                                Tours.Add(addTour);
                            }
                        }
                        else
                        {
                            addTour.Maneuvers = createUpdateTourViewModel.Maneuvers.Where(m=>!string.IsNullOrEmpty(m.Narrative)&&m.Distance>=0).ToList();

                            if (!(addTour.Distance <= 0 && addTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(addTour.RouteInfo)))
                            {
                                await tourPlannerFactory.CreateTour(addTour);
                                Tours.Add(addTour);
                            }
                        }
                    }
                },
                (_) => {
                    return true;
            });
            RemoveTourCommand = new RelayCommand(
                async (_) => {
                    try
                    {
                        await tourPlannerFactory.DeleteTour(SelectedTour);
                        Tours.Remove(SelectedTour);
                    }
                    catch
                    {
                    }
                },
                (_) => {
                    return SelectedTour!=null ? true : false;
            });
            EditTourCommand = new RelayCommand(
                async(_) => {
                    List<Maneuver> copiedManeuvers = new List<Maneuver>();
                    SelectedTour.Maneuvers.ForEach(m => copiedManeuvers.Add(new Maneuver() { Id = m.Id, Distance = m.Distance, Narrative = m.Narrative, TourId = m.TourId }));
                    Tour editTour = new Tour() { Id=SelectedTour.Id, Description=SelectedTour.Description, Distance=SelectedTour.Distance, EndLocation=SelectedTour.EndLocation, StartLocation=SelectedTour.StartLocation, Name = SelectedTour.Name, RouteInfo=SelectedTour.RouteInfo, RouteType = SelectedTour.RouteType, Maneuvers=copiedManeuvers };
                    CreateUpdateTourViewModel createUpdateTourViewModel = new CreateUpdateTourViewModel(editTour);
                    bool? result = dialogService.ShowDialog(createUpdateTourViewModel);
                    if (result ?? false)
                    {
                        if (createUpdateTourViewModel.ManualTour == false)
                        {
                            if (!(editTour.Distance < 0 && editTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(editTour.RouteInfo)))
                            {
                                await tourPlannerFactory.UpdateMapQuestTour(editTour);
                                Tours.Add(editTour);
                                Tours.Remove(SelectedTour);
                            }
                        }
                        else
                        {
                            editTour.Maneuvers = createUpdateTourViewModel.Maneuvers.Where(m => !string.IsNullOrEmpty(m.Narrative) && m.Distance >= 0).ToList();

                            if (!(editTour.Distance <= 0 && editTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(editTour.RouteInfo)))
                            {
                                await tourPlannerFactory.UpdateTour(editTour);
                                Tours.Add(editTour);
                                Tours.Remove(SelectedTour);
                                SelectedTour = editTour;
                            }
                        }
                    }
                },
                (_) => {
                    return SelectedTour!=null;
            });
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
                    return SelectedTour!=null?true:false;
            });
            RemoveLogCommand = new RelayCommand(
                async (_) => {
                    await tourPlannerFactory.DeleteTourLog(SelectedTourLog);
                    SelectedTour.TourLogs.Remove(SelectedTourLog);
                    CurrentTourLogs.Remove(SelectedTourLog);
                },
                (_) => {
                    return SelectedTourLog!= null ? true : false;
            });
            EditLogCommand = new RelayCommand(
                async (_) => {
                    TourLog editTourLog = new TourLog() { Id=SelectedTourLog.Id, TourId=SelectedTourLog.TourId, StartDate=SelectedTourLog.StartDate, EndDate=SelectedTourLog.EndDate, Distance=SelectedTourLog.Distance, Temperature=SelectedTourLog.Temperature, Weather=SelectedTourLog.Weather, TravelMethod = SelectedTourLog.TravelMethod, Rating = SelectedTourLog.Rating, Report=SelectedTourLog.Report };
                    CreateUpdateTourLogViewModel createUpdateTourLogViewModel = new CreateUpdateTourLogViewModel(editTourLog);
                    bool? result = dialogService.ShowDialog(createUpdateTourLogViewModel);
                    if (result.HasValue)
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