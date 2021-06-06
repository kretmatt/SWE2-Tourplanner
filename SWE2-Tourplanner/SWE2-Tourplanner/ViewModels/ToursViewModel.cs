using BusinessLogicLayer.Factories;
using Common;
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
    /// TourViewModel provides tours and a way to search / filter tours
    /// </summary>
    public class ToursViewModel:BaseViewModel
    {
        /// <summary>
        /// Object used for retrieving tours
        /// </summary>
        private ITourFactory tourPlannerFactory;
        /// <summary>
        /// Service used for displaying exceptions / errors
        /// </summary>
        private IDialogService dialogService;
        /// <summary>
        /// Search term
        /// </summary>
        private string searchString;
        /// <summary>
        /// Tours filtered by search term
        /// </summary>
        private ObservableCollection<Tour> filteredTours;
        /// <summary>
        /// All tours found in datastore
        /// </summary>
        private ObservableCollection<Tour> _tours;
        /// <value>
        /// Tours filtered by search term
        /// </value>
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
        /// <value>
        /// Search term
        /// </value>
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
        /// <value>
        /// All tours from the datastore
        /// </value>
        public ObservableCollection<Tour> Tours
        {
            get
            {
                return _tours;
            }
            set
            {
                if (_tours != value)
                {
                    _tours = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Command for searching / filtering tours
        /// </value>
        public ICommand SearchToursCommand { get; }

        /// <summary>
        /// Default constructor of ToursViewModel
        /// </summary>
        /// <param name="tourPlannerFactory">Object used for retrieving tours from datastore</param>
        /// <param name="dialogService">Service used for opening dialogs and handling their results</param>
        public ToursViewModel(ITourFactory tourPlannerFactory, IDialogService dialogService)
        {
            this.tourPlannerFactory = tourPlannerFactory;
            _tours = new ObservableCollection<Tour>(tourPlannerFactory.GetTours());
            FilteredTours = Tours;
            this.dialogService = dialogService;
            SearchToursCommand = new RelayCommand(
                (_) => {
                    try
                    {
                        Tours = new ObservableCollection<Tour>(tourPlannerFactory.GetTours());
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
                    }
                    catch(Exception e)
                    {
                        ErrorViewModel evm = new ErrorViewModel(e.Message,e.GetType().ToString());
                        dialogService.ShowDialog(evm);
                    }
                    
                },
                (_) => {
                    return true;
                });
        }
    }
}
