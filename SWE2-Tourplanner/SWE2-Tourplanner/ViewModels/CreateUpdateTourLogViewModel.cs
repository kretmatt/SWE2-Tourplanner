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
    /// <summary>
    /// CreateUpdateTourLogViewModel is used for creating and updating TourLog entities. To be precise, it is used for setting the values and initiating creation / update process in another class.
    /// </summary>
    public class CreateUpdateTourLogViewModel:BaseViewModel, IDialogRequestClose
    {
        /// <summary>
        /// The TourLog entity, whose values need to be set.
        /// </summary>
        private TourLog tourLog;

        /// <summary>
        /// Event for closing the Dialog
        /// </summary>
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        /// <value>
        /// DateTime value when the TourLog started.
        /// </value>
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
        /// <value>
        /// DateTime value when the TourLog ended.
        /// </value>
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
        /// <value>
        /// Distance is the amount of km travelled.
        /// </value>
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
        /// <value>
        /// Temperature in Celsius during the tour.
        /// </value>
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
        /// <value>
        /// Rating of the tour from 0 - 10
        /// </value>
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
        /// <value>
        /// Weather during the tour.
        /// </value>
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
        /// <value>
        /// Travel method used for the tour.
        /// </value>
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
        /// <value>
        /// Additional comment on the tour.
        /// </value>
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
        /// <value>
        /// Text on the Confirm Button
        /// </value>
        public string ConfirmButtonMessage
        {
            get;
        }
        /// <value>
        /// Text in the heading.
        /// </value>
        public string HeadingMessage
        {
            get;
        }
        /// <value>
        /// All available weather values.
        /// </value>
        public IEnumerable<EWeather> WeatherTypes
        {
            get
            {
                return Enum.GetValues(typeof(EWeather)).Cast<EWeather>();
            }
        }
        /// <value>
        /// All available travel methods.
        /// </value>
        public IEnumerable<ETravelMethod> TravelMethods
        {
            get
            {
                return Enum.GetValues(typeof(ETravelMethod)).Cast<ETravelMethod>();
            }
        }
        /// <value>
        /// Command to stop the create / update process.
        /// </value>
        public ICommand ExitCommand { get; }
        /// <value>
        /// Command to start create / update process.
        /// </value>
        public ICommand ConductCreateUpdateCommand { get; }
        /// <summary>
        /// Default constructor of CreateUpdateTourLogViewModel
        /// </summary>
        /// <param name="tourLog">TourLog to be created / updated</param>
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
                (_) => { return tourLog.Rating>=0 && tourLog.Rating<=10 && tourLog.Distance>=0 && tourLog.Temperature>=-100 && tourLog.Temperature<=65 &&tourLog.StartDate.CompareTo(tourLog.EndDate)<0; }
            );
        }
    }
}
