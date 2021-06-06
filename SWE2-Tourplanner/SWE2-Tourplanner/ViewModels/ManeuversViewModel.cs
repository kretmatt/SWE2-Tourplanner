using Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SWE2_Tourplanner.Dialogs;
using SWE2_Tourplanner.Commands;

namespace SWE2_Tourplanner.ViewModels
{
    /// <summary>
    /// ManeuversViewModel is used for managing Maneuver entities of a Tour
    /// </summary>
    public class ManeuversViewModel:BaseViewModel, IDialogRequestClose
    {
        /// <summary>
        /// Maneuvers of the Tour
        /// </summary>
        private ObservableCollection<Maneuver> maneuvers;
        /// <summary>
        /// Currently selected Maneuver
        /// </summary>
        private Maneuver currentManeuver;
        /// <summary>
        /// Event used for closing the dialog
        /// </summary>
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        /// <value>
        /// Maneuvers of the Tour
        /// </value>
        public ObservableCollection<Maneuver> Maneuvers
        {
            get { return maneuvers; }
            set
            {
                if (value != maneuvers)
                {
                    maneuvers = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Currently selected Maneuver
        /// </value>
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
        /// <value>
        /// Command for adding new maneuvers
        /// </value>
        public ICommand AddManeuverCommand { get; }
        /// <value>
        /// Command for removing currently selected maneuver
        /// </value>
        public ICommand RemoveManeuverCommand { get; }
        /// <value>
        /// Command for confirming the maneuvers of the tour
        /// </value>
        public ICommand ConfirmCommand { get; }
        /// <value>
        /// Command for closing the dialog
        /// </value>
        public ICommand ExitCommand { get; }
        /// <summary>
        /// Default constructor of ManeuversViewModel
        /// </summary>
        /// <param name="maneuvers">Current maneuvers of the tour</param>
        public ManeuversViewModel(List<Maneuver> maneuvers)
        {
            this.maneuvers = new ObservableCollection<Maneuver>(maneuvers);

            ExitCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                },
                (_) => { return true; }
            );

            ConfirmCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
                },
                (_) => { return Maneuvers.All(m => !string.IsNullOrEmpty(m.Narrative) && m.Distance >= 0) && Maneuvers.Count>0; }
            );

            AddManeuverCommand = new RelayCommand(
                (_) =>
                {
                    Maneuver m = new Maneuver();
                    Maneuvers.Add(m);
                    CurrentManeuver = m;
                },
                (_) => { return true; }
             );

            RemoveManeuverCommand = new RelayCommand(
                (_) =>
                {
                    Maneuvers.Remove(CurrentManeuver);
                    CurrentManeuver = null;
                },
                (_) => { return CurrentManeuver!=null; }
             );
        }
    }
}
