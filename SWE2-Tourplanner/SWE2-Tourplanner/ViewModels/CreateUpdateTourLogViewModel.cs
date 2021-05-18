using Common.Entities;
using Common.Enums;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    public class CreateUpdateTourLogViewModel:BaseViewModel, IDialogRequestClose
    {
        private TourLog tourLog;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public DateTime StartDate
        {
            get { return tourLog.StartDate; }
            set
            {
                if (value != tourLog.StartDate)
                {
                    tourLog.StartDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime EndDate
        {
            get { return tourLog.EndDate; }
            set
            {
                if (value != tourLog.EndDate)
                {
                    tourLog.EndDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Distance
        {
            get { return tourLog.Distance; }
            set
            {
                if (value != tourLog.Distance)
                {
                    tourLog.Distance = value; 
                    OnPropertyChanged();
                }
            }
        }
        public double Temperature
        {
            get { return tourLog.Temperature; }
            set
            {
                if (value != tourLog.Temperature)
                {
                    tourLog.Temperature = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Rating
        {
            get { return tourLog.Rating; }
            set
            {
                if (value != tourLog.Rating)
                {
                    tourLog.Rating = value;
                    OnPropertyChanged();
                }
            }
        }
        public EWeather Weather
        {
            get { return tourLog.Weather; }
            set
            {
                if (value != tourLog.Weather)
                {
                    tourLog.Weather = value;
                    OnPropertyChanged();
                }
            }
        }
        public ETravelMethod TravelMethod
        {
            get { return tourLog.TravelMethod; }
            set
            {
                if (value != tourLog.TravelMethod)
                {
                    tourLog.TravelMethod = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Report
        {
            get { return tourLog.Report; }
            set
            {
                if (value != tourLog.Report)
                {
                    tourLog.Report = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ConfirmButtonMessage
        {
            get;
        }
        public string HeadingMessage
        {
            get;
        }
        public IEnumerable<EWeather> WeatherTypes
        {
            get
            {
                return Enum.GetValues(typeof(EWeather)).Cast<EWeather>();
            }
        }
        public IEnumerable<ETravelMethod> TravelMethods
        {
            get
            {
                return Enum.GetValues(typeof(ETravelMethod)).Cast<ETravelMethod>();
            }
        }
        public ICommand ExitCommand { get; }
        public ICommand ConductCreateUpdateCommand { get; }

        public CreateUpdateTourLogViewModel(TourLog tourLog)
        {
            this.tourLog = tourLog;
            if (tourLog.Id <= 0)
            {
                ConfirmButtonMessage = "Create";
                HeadingMessage = "TourLog Create";
            }
            else
            {
                ConfirmButtonMessage = "Update";
                HeadingMessage = "TourLog Update";
            }

            ExitCommand = new RelayCommand(
                (_) => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)),
                (_) => { return true; }
            );
            ConductCreateUpdateCommand = new RelayCommand(
                (_) => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)),
                (_) => { return (Rating>=0 && Rating<=10) && (Distance>=0) && (Temperature>=-100 && Temperature<=65) && (StartDate.CompareTo(EndDate)<0); }
            );
        }
    }
}
