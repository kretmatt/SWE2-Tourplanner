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
    /// Interaktionslogik für ExternalResourceControl.xaml
    /// </summary>
    public partial class ExternalResourceControl : UserControl, INotifyPropertyChanged
    {
        private static readonly string googleFaviconCrawler = @"https://www.google.com/s2/favicons?domain";
        private string faviconLocation;
        public string LinkText
        {
            get { return (string)GetValue(LinkTextProperty); }
            set { 
                SetValue(LinkTextProperty, value);
                OnPropertyChanged();
            }
        }

        // Using a DependencyProperty as the backing store for LinkText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinkTextProperty =
            DependencyProperty.Register("LinkText", typeof(string), typeof(ExternalResourceControl), new PropertyMetadata(""));
        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set { 
                SetValue(LinkProperty, value);
                OnPropertyChanged();
            }
        }

        // Using a DependencyProperty as the backing store for Link.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string), typeof(ExternalResourceControl), new PropertyMetadata("", new PropertyChangedCallback(OnLinkChanged)));

        public string Favicon
        {
            get { return faviconLocation; }
            set
            {
                if (value != faviconLocation)
                {
                    faviconLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand OpenLink { get; }

        public ExternalResourceControl()
        {
            InitializeComponent();
        }

        private void ExternalResource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", Link);
        }

        private static void OnLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExternalResourceControl eRControl = (ExternalResourceControl)d;
            eRControl.Favicon = $"{googleFaviconCrawler}={eRControl.Link}";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
