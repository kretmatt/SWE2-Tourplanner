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
    public class CreateUpdateTourViewModel:BaseViewModel, IDialogRequestClose
    {
        private Tour tour;
        private bool manualTour = false;
        private ObservableCollection<Maneuver> maneuvers;
        private Maneuver currentManeuver;
        private ManeuversViewModel maneuversViewModel;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
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

        public Maneuver CurrentManeuver
        {
            get
            {
                return currentManeuver;
            }
            set
            {
                if (currentManeuver != value)
                {
                    currentManeuver = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public IEnumerable<ERouteType> RouteTypes
        {
            get
            {
                return Enum.GetValues(typeof(ERouteType)).Cast<ERouteType>();
            }
        }
        public string HeadingMessage { get; }
        public string ConfirmButtonMessage { get; }
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
        public ICommand ExitCommand { get; }
        public ICommand CommitCommand { get; }
        public ICommand ManageManeuverCommand { get; }
        public ICommand AddManeuverCommand { get; }
        public ICommand SelectRouteInfoCommand { get; }

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
                    maneuversViewModel = new ManeuversViewModel(Maneuvers.ToList());
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
