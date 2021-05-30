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
    public class ManeuversViewModel:BaseViewModel, IDialogRequestClose
    {
        private ObservableCollection<Maneuver> maneuvers;
        private Maneuver currentManeuver;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

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

        public ICommand AddManeuverCommand { get; }
        public ICommand RemoveManeuverCommand { get; }

        public ICommand ConfirmCommand { get; }
        public ICommand ExitCommand { get; }

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
