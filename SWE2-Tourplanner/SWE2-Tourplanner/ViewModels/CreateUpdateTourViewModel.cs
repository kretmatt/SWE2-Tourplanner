using Common.Entities;
using Common.Enums;
using Microsoft.Win32;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    /// <summary>
    /// CreateUpdateTourViewModel is used for creating / updating tours with MapQuest or manually. To be precise, it can start the creation / update process of tours.
    /// </summary>
    public class CreateUpdateTourViewModel:BaseViewModel, IDialogRequestClose
    {
        /// <summary>
        /// Tour to be created / updated
        /// </summary>
        private Tour tour;
        /// <summary>
        /// Flag that determines if Tour will be created / updated manually or with MapQuest
        /// </summary>
        private bool manualTour = false;
        /// <summary>
        /// Collection of maneuvers of the tour
        /// </summary>
        private ObservableCollection<Maneuver> maneuvers;
        /// <summary>
        /// ViewModel for the managing maneuvers
        /// </summary>
        private ManeuversViewModel maneuversViewModel;
        /// <summary>
        /// Event for closing the dialog
        /// </summary>
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        /// <value>
        /// Name of the tour
        /// </value>
        public string Name
        {
            get { return tour.Name; }
            set
            {
                if (value != tour.Name)
                {
                    tour.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Start location of the tour
        /// </value>
        public string StartLocation
        {
            get { return tour.StartLocation; }
            set
            {
                if (value != tour.StartLocation)
                {
                    tour.StartLocation = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// End location of the tour
        /// </value>
        public string EndLocation
        {
            get { return tour.EndLocation; }
            set
            {
                if (value != tour.EndLocation)
                {
                    tour.EndLocation = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Description of the tour
        /// </value>
        public string Description
        {
            get { return tour.Description; }
            set
            {
                if (value != tour.Description)
                {
                    tour.Description = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Route type of the tour
        /// </value>
        public ERouteType RouteType
        {
            get { return tour.RouteType; }
            set
            {
                if (value != tour.RouteType)
                {
                    tour.RouteType = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Length (km) of the tour
        /// </value>
        public double Distance 
        {
            get 
            {
                return tour.Distance;
            }
            set
            {
                if (value != tour.Distance)
                {
                    tour.Distance = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Maneuvers of the tour
        /// </value>
        public ObservableCollection<Maneuver> Maneuvers
        {
            get
            {
                return maneuvers;
            }
            set
            {
                if (maneuvers != value)
                {
                    maneuvers = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Path to the map image of the tour
        /// </value>
        public string RouteInfo
        {
            get { return tour.RouteInfo; }
            set
            {
                if (value != tour.RouteInfo)
                {
                    tour.RouteInfo = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// All available route types
        /// </value>
        public IEnumerable<ERouteType> RouteTypes
        {
            get
            {
                return Enum.GetValues(typeof(ERouteType)).Cast<ERouteType>();
            }
        }
        /// <value>
        /// Text in the heading
        /// </value>
        public string HeadingMessage { get; }
        /// <value>
        /// Text on the confirm button
        /// </value>
        public string ConfirmButtonMessage { get; }
        /// <value>
        /// Flag that indicates whether the tour should be created manually or not
        /// </value>
        public bool ManualTour
        {
            get { return manualTour; }
            set
            {
                if (manualTour != value)
                {
                    manualTour = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Command to exit the dialog
        /// </value>
        public ICommand ExitCommand { get; }
        /// <value>
        /// Command to initiate the creation / update process
        /// </value>
        public ICommand CommitCommand { get; }
        /// <value>
        /// Command to open ManeuverManagement Dialog
        /// </value>
        public ICommand ManageManeuverCommand { get; }
        /// <value>
        /// Command to select path to map image
        /// </value>
        public ICommand SelectRouteInfoCommand { get; }
        /// <summary>
        /// Default constructor of CreateUpdateTourViewModel
        /// </summary>
        /// <param name="tour">Tour to be updated / created</param>
        /// <param name="dialogService">DialogService used for opening ManeuverManagementWindow</param>
        public CreateUpdateTourViewModel(Tour tour, IDialogService dialogService)
        {
            this.tour = tour;
            maneuvers = new ObservableCollection<Maneuver>(tour.Maneuvers);
            maneuversViewModel = new ManeuversViewModel(tour.Maneuvers);
            if (tour.Id <= 0)
            {
                HeadingMessage = "Tour create";
                ConfirmButtonMessage = "Create tour";
            }
            else
            {
                HeadingMessage = "Tour update";
                ConfirmButtonMessage = "Update tour";
            }

            ManageManeuverCommand = new RelayCommand(
                (_) =>
                {
                    maneuversViewModel = new ManeuversViewModel(new List<Maneuver>(Maneuvers.ToList()));
                    bool? result = dialogService.ShowDialog(maneuversViewModel);
                    if (result ?? false)
                    {
                        Maneuvers = maneuversViewModel.Maneuvers;
                    }
                },
                (_) =>
                {
                    return ManualTour;
                }
            );

            SelectRouteInfoCommand = new RelayCommand(
                (_) =>
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
                    dialog.RestoreDirectory = true;
                    if (dialog.ShowDialog() ?? false)
                    {
                        RouteInfo = dialog.FileName;
                    }

                },
                (_) => true

            );

            ExitCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                },
                (_) => { return true; }
            );

            CommitCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
                },
                (_) => {
                    bool standardConditions = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(StartLocation) && !string.IsNullOrEmpty(EndLocation);
                    if (ManualTour == false)
                        return standardConditions;
                    else
                        return standardConditions && Distance > 0 && File.Exists(RouteInfo) && Maneuvers.Count>0 && Maneuvers.All(m=>!string.IsNullOrEmpty(m.Narrative)&&m.Distance>=0) && Maneuvers.Sum(m=>m.Distance)==Distance;
                }
            );
            
        }
    }
}
