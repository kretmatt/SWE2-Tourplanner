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
        private TourDetailViewModel tourDetailViewModel;
        private ITourPlannerFactory tourPlannerFactory;
        private readonly IDialogService dialogService;

        private ObservableCollection<Tour> _tours = new ObservableCollection<Tour>();
        private Tour _selectedTour;
        private TourLog selectedTourLog;
        private ITourPlannerReportsGenerator pdfGenerator;
        private IExporterImporter<Tour> exporterImporter;
        private string searchString;
        private ObservableCollection<Tour> filteredTours;
        private ObservableCollection<TourLog> currentTourLogs;

        public TourDetailViewModel TourDetailViewModel
        {
            get { return tourDetailViewModel; }
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
        public ICommand SearchToursCommand { get; }
        public ICommand AddTourCommand { get; }
        public ICommand RemoveTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand GenerateJSONExportCommand { get; }
        public ICommand ConductImportCommand { get; }


        public MainViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            tourPlannerFactory = new TourPlannerFactory();
            tourDetailViewModel = new TourDetailViewModel(dialogService,tourPlannerFactory);
            pdfGenerator = new TourPlannerReportsGenerator();
            exporterImporter = new TourExporterImporter();
            filteredTours = new ObservableCollection<Tour>();
            tourPlannerFactory.GetTours().ForEach(t=>_tours.Add(t));
            FilteredTours = Tours;

            GenerateTourReportCommand = new RelayCommand(
                async (_) => await pdfGenerator.GenerateTourReport(TourDetailViewModel.SelectedTour),
                (_) =>
                {
                    return TourDetailViewModel.SelectedTour != null ? true : false;
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
                    try
                    {
                        ImportViewModel importViewModel = new ImportViewModel(TourPlannerConfig.GetTourPlannerConfig());
                        bool? result = dialogService.ShowDialog(importViewModel);
                        if (result ?? false)
                        {
                            foreach (string path in importViewModel.JsonPaths)
                            {
                                foreach (Tour t in await exporterImporter.Import(path))
                                {
                                    Tours.Add(t);
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        ErrorViewModel evm = new ErrorViewModel("An error occured during json import. Try again with other files!", e.GetType().ToString());
                        dialogService.ShowDialog(evm);
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
                    try
                    {
                        Tour addTour = new Tour();
                        CreateUpdateTourViewModel createUpdateTourViewModel = new CreateUpdateTourViewModel(addTour);
                        bool? result = dialogService.ShowDialog(createUpdateTourViewModel);
                        if (result ?? false)
                        {
                            if (createUpdateTourViewModel.ManualTour == false)
                            {
                                if (!(addTour.Distance < 0 && addTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(addTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.CreateMapQuestTour(addTour);
                                    Tours.Add(addTour);
                                }
                            }
                            else
                            {
                                addTour.Maneuvers = createUpdateTourViewModel.Maneuvers.Where(m => !string.IsNullOrEmpty(m.Narrative) && m.Distance >= 0).ToList();

                                if (!(addTour.Distance <= 0 && addTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(addTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.CreateTour(addTour);
                                    Tours.Add(addTour);
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show($"Exception handling stub: {e.Message}");
                    }
                },
                (_) => {
                    return true;
            });
            RemoveTourCommand = new RelayCommand(
                async (_) => {
                    try
                    {
                        await tourPlannerFactory.DeleteTour(TourDetailViewModel.SelectedTour);
                        Tours.Remove(TourDetailViewModel.SelectedTour);
                        TourDetailViewModel.SelectedTour = null;
                    }
                    catch
                    {
                    }
                },
                (_) => {
                    return TourDetailViewModel.SelectedTour!=null ? true : false;
            });
            EditTourCommand = new RelayCommand(
                async(_) => {
                    try
                    {
                        List<Maneuver> copiedManeuvers = new List<Maneuver>();
                        TourDetailViewModel.SelectedTour.Maneuvers.ForEach(m => copiedManeuvers.Add(new Maneuver() { Id = m.Id, Distance = m.Distance, Narrative = m.Narrative, TourId = m.TourId }));
                        Tour editTour = new Tour() { Id = TourDetailViewModel.SelectedTour.Id, Description = TourDetailViewModel.SelectedTour.Description, Distance = TourDetailViewModel.SelectedTour.Distance, EndLocation = TourDetailViewModel.SelectedTour.EndLocation, StartLocation = TourDetailViewModel.SelectedTour.StartLocation, Name = TourDetailViewModel.SelectedTour.Name, RouteInfo = TourDetailViewModel.SelectedTour.RouteInfo, RouteType = TourDetailViewModel.SelectedTour.RouteType, Maneuvers = copiedManeuvers };
                        CreateUpdateTourViewModel createUpdateTourViewModel = new CreateUpdateTourViewModel(editTour);
                        bool? result = dialogService.ShowDialog(createUpdateTourViewModel);
                        if (result ?? false)
                        {
                            if (createUpdateTourViewModel.ManualTour == false)
                            {
                                if (!(editTour.Distance < 0 && editTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(editTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.UpdateMapQuestTour(editTour);
                                    editTour.TourLogs = TourDetailViewModel.SelectedTour.TourLogs;
                                    Tours.Add(editTour);
                                    Tours.Remove(TourDetailViewModel.SelectedTour);
                                    TourDetailViewModel.SelectedTour = editTour;
                                }
                            }
                            else
                            {
                                editTour.Maneuvers = createUpdateTourViewModel.Maneuvers.Where(m => !string.IsNullOrEmpty(m.Narrative) && m.Distance >= 0).ToList();

                                if (!(editTour.Distance <= 0 && editTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(editTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.UpdateTour(editTour);
                                    Tours.Add(editTour);
                                    Tours.Remove(TourDetailViewModel.SelectedTour);
                                    TourDetailViewModel.SelectedTour = editTour;
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Exception Handling stub -> Tour could not be created. Try again with other values!");
                    } 
                },
                (_) => {
                    return TourDetailViewModel.SelectedTour!=null;
            });
        }
    }
}