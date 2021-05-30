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
    public class ToursViewModel:BaseViewModel
    {
        private ITourPlannerFactory tourPlannerFactory;
        private string searchString;
        private ObservableCollection<Tour> filteredTours;
        private ObservableCollection<Tour> _tours;

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
            set
            {
                if (_tours != value)
                {
                    _tours = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SearchToursCommand { get; }


        public ToursViewModel(ITourPlannerFactory tourPlannerFactory)
        {
            this.tourPlannerFactory = tourPlannerFactory;
            _tours = new ObservableCollection<Tour>(tourPlannerFactory.GetTours());
            FilteredTours = Tours;

            SearchToursCommand = new RelayCommand(
                (_) => {
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
                },
                (_) => {
                    return true;
                });
        }
    }
}
