using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SWE2_Tourplanner.ReusableComponents
{
    /// <summary>
    /// ExternalResourceControl is UserControl used for linking external resources like websites
    /// </summary>
    public partial class ExternalResourceControl : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Base URL of the Google Favicon API / Crawler
        /// </summary>
        private static readonly string googleFaviconCrawler = @"https://www.google.com/s2/favicons?domain";
        /// <summary>
        /// Location of the favicon. Is a URL based on the Google Favicon API / Crawler URL
        /// </summary>
        private string faviconLocation;
        /// <value>
        /// Text that is displayed instead of the Link itself
        /// </value>
        public string LinkText
        {
            get { return (string)GetValue(LinkTextProperty); }
            set { 
                SetValue(LinkTextProperty, value);
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// DependencyProperty as the backing store for LinkText. Enables Binding
        /// </summary>
        public static readonly DependencyProperty LinkTextProperty =
            DependencyProperty.Register("LinkText", typeof(string), typeof(ExternalResourceControl), new PropertyMetadata(""));

        /// <value>
        /// Link is the URL of the external resource
        /// </value>
        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set { 
                SetValue(LinkProperty, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// DependencyProperty as the backing store of Link. Enables Binding
        /// </summary>
        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string), typeof(ExternalResourceControl), new PropertyMetadata("", new PropertyChangedCallback(OnLinkChanged)));

        /// <value>
        /// Location of the favicon. Is a URL based on the Google Favicon API / Crawler URL. Can return DependencyProperty.UnsetValue to avoid Binding Errors
        /// <value/>
        public object Favicon
        {
            get
            {
                if (string.IsNullOrEmpty(faviconLocation))
                    return DependencyProperty.UnsetValue;
                return faviconLocation; 
            }
            set
            {
                if ((string)value != faviconLocation)
                {
                    faviconLocation = (string)value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Default constructor of ExternalResourceControl. Initializes the component
        /// </summary>
        public ExternalResourceControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Click event of ExternalResourceControl. Opens Link in the default browser.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">MouseButtonEventArgs associated with the click</param>
        private void ExternalResource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", Link);
        }
        /// <summary>
        /// Callback method for the DependencyProperty LinkProperty. Updates the Favicon value according to the changes in the Binding
        /// </summary>
        /// <param name="d">DependcyObject that calls the callback method</param>
        /// <param name="e">DependencyPropertyChangedEventArgs associated with the call</param>
        private static void OnLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExternalResourceControl eRControl = (ExternalResourceControl)d;
            eRControl.Favicon = $"{googleFaviconCrawler}={eRControl.Link}";
        }
        /// <summary>
        /// EventHandler used for notifying the UI aspect of the UserControl about data changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// OnPropertyChanged notifies the UI aspect of the UserControl about changes of a specific property
        /// </summary>
        /// <param name="propertyName">Name of the changed property</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
