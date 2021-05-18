using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SWE2_Tourplanner.ReusableComponents
{
    /// <summary>
    /// Interaktionslogik für TemperatureSliders.xaml
    /// </summary>
    public partial class TemperatureSliders : UserControl, INotifyPropertyChanged
    {
        private double kelvinTemperature;
        private double fahrenheitTemperature;
        public double Temperature
        {
            get 
            {
                return (double)GetValue(TemperatureProperty); 
            }
            set 
            { 
                SetValue(TemperatureProperty, value);
                KelvinTemperature = value;
                FahrenheitTemperature = value;
                OnPropertyChanged();
            }
        }

        // Using a DependencyProperty as the backing store for Temperature.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register("Temperature", typeof(double), typeof(TemperatureSliders), new PropertyMetadata(0.0));

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TemperatureSliders()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public double KelvinTemperature
        {
            get
            {
                return kelvinTemperature;
            }
            set
            {
                if (value + 273.15 != fahrenheitTemperature)
                {
                    kelvinTemperature = Temperature + 273.15;
                    OnPropertyChanged();
                }
            }
        }
        public double FahrenheitTemperature
        {
            get
            {
                return fahrenheitTemperature;
            }
            set
            {
                if((value*1.8+32)!= fahrenheitTemperature){
                    fahrenheitTemperature = (value * 1.8) + 32;
                    OnPropertyChanged();
                }
            }
        }
    }
}
