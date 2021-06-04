using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BusinessLogicLayer.ExporterImporter;
using BusinessLogicLayer.Factories;
using BusinessLogicLayer.PDFCreation;
using Common.Config;
using Common.Entities;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using SWE2_Tourplanner.ViewModels;
using BusinessLogicLayer.Exceptions;
using System.IO;

namespace SWE2_Tourplanner
{
    public class MainViewModel:BaseViewModel
    {
        private TourDetailViewModel tourDetailViewModel;
        private ToursViewModel toursViewModel;
        private ITourFactory tourPlannerFactory;
        private readonly IDialogService dialogService;
        private ITourPlannerReportsGenerator pdfGenerator;
        private IExporterImporter<Tour> exporterImporter;
        public TourDetailViewModel TourDetailViewModel
        {
            get { return tourDetailViewModel; }
        }

        public ToursViewModel ToursViewModel
        {
            get { return toursViewModel; }
        }
        public ICommand SearchToursCommand { get; }
        public ICommand AddTourCommand { get; }
        public ICommand RemoveTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand GenerateJSONExportCommand { get; }
        public ICommand ConductImportCommand { get; }
        public ICommand LoadConfigCommand { get; }

        private void ShowException(Exception e)
        {
            ErrorViewModel evm = new ErrorViewModel(e.Message, e.GetType().ToString());
            dialogService.ShowDialog(evm);
        }

        public MainViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            tourPlannerFactory = new TourFactory();
            tourDetailViewModel = new TourDetailViewModel(dialogService);
            toursViewModel = new ToursViewModel(tourPlannerFactory, dialogService);
            pdfGenerator = new TourPlannerReportsGenerator();
            exporterImporter = new TourExporterImporter();

            LoadConfigCommand = new RelayCommand(
                (_) => {
                    try
                    {
                        ImportConfigViewModel importCVM = new ImportConfigViewModel();
                        bool? result = dialogService.ShowDialog(importCVM);
                        if (result ?? false)
                        {
                            TourPlannerConfig.GetTourPlannerConfig().LoadConfigFromFile(importCVM.ConfigPath);
                        }
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                        toursViewModel = new ToursViewModel(tourPlannerFactory, dialogService);
                        tourDetailViewModel = new TourDetailViewModel(dialogService);
                    }

                },
                (_) =>
                {
                    return true;
                }
            );

            GenerateTourReportCommand = new RelayCommand(
                async (_) =>
                {
                    try
                    {
                        await pdfGenerator.GenerateTourReport(TourDetailViewModel.SelectedTour);
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                    }
                } ,
                (_) =>
                {
                    return TourDetailViewModel.SelectedTour != null ? true : false;
                }
            );

            GenerateSummaryReportCommand = new RelayCommand(
                    async(object selectedItems) => 
                    {
                        try
                        {
                            System.Collections.IList items = (System.Collections.IList)selectedItems;
                            List<Tour> selectedTours = items.Cast<Tour>().ToList();
                            await pdfGenerator.GenerateSummaryReport(selectedTours);
                        }
                        catch (Exception e)
                        {
                            ShowException(e);
                        }
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
                        try
                        {
                            await exporterImporter.Export(selectedTours);
                        }
                        catch (Exception e)
                        {
                            ShowException(e);
                        }
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
                                    ToursViewModel.Tours.Add(t);
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                    }         
                },
                (_) => { return true; }
            );
            AddTourCommand = new RelayCommand(
                async (_) => {
                    try
                    {
                        Tour addTour = new Tour();
                        CreateUpdateTourViewModel createUpdateTourViewModel = new CreateUpdateTourViewModel(addTour, dialogService);
                        bool? result = dialogService.ShowDialog(createUpdateTourViewModel);
                        if (result ?? false)
                        {
                            if (createUpdateTourViewModel.ManualTour == false)
                            {
                                if (!(addTour.Distance < 0 && addTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(addTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.CreateMapQuestTour(addTour);
                                    ToursViewModel.Tours.Add(addTour);
                                }
                            }
                            else
                            {
                                addTour.Maneuvers = createUpdateTourViewModel.Maneuvers.Where(m => !string.IsNullOrEmpty(m.Narrative) && m.Distance >= 0).ToList();

                                if (!(addTour.Distance <= 0 && addTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(addTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.CreateTour(addTour);
                                    ToursViewModel.Tours.Add(addTour);
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
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
                        ToursViewModel.Tours.Remove(TourDetailViewModel.SelectedTour);
                        TourDetailViewModel.SelectedTour = null;
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
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
                        CreateUpdateTourViewModel createUpdateTourViewModel = new CreateUpdateTourViewModel(editTour, dialogService);
                        bool? result = dialogService.ShowDialog(createUpdateTourViewModel);
                        if (result ?? false)
                        {
                            if (createUpdateTourViewModel.ManualTour == false)
                            {
                                if (!(editTour.Distance < 0 && editTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(editTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.UpdateMapQuestTour(editTour);
                                    editTour.TourLogs = TourDetailViewModel.SelectedTour.TourLogs;
                                    ToursViewModel.Tours.Add(editTour);
                                    ToursViewModel.Tours.Remove(TourDetailViewModel.SelectedTour);
                                    TourDetailViewModel.SelectedTour = editTour;
                                }
                            }
                            else
                            {
                                editTour.Maneuvers = createUpdateTourViewModel.Maneuvers.Where(m => !string.IsNullOrEmpty(m.Narrative) && m.Distance >= 0).ToList();

                                if (!(editTour.Distance <= 0 && editTour.Maneuvers.Count == 0 && string.IsNullOrEmpty(editTour.RouteInfo)))
                                {
                                    await tourPlannerFactory.UpdateTour(editTour);
                                    editTour.TourLogs = TourDetailViewModel.SelectedTour.TourLogs;
                                    ToursViewModel.Tours.Add(editTour);
                                    ToursViewModel.Tours.Remove(TourDetailViewModel.SelectedTour);
                                    TourDetailViewModel.SelectedTour = editTour;
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        ShowException(e);
                    } 
                },
                (_) => {
                    return TourDetailViewModel.SelectedTour!=null;
            });
        }
    }
}