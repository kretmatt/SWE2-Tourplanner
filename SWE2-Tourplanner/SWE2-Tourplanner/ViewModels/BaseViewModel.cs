using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SWE2_Tourplanner.ViewModels
{
    /// <summary>
    /// BaseViewModel provides inheriting classes with PropertyChanged and OnPropertyChanged. It allows inheriting ViewModels to notify the UI about data changes.
    /// </summary>
    public abstract class BaseViewModel:INotifyPropertyChanged
    {
        /// <summary>
        /// EventHandler used for notifying the UI aspect of the application about data changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// OnPropertyChanged invokes PropertyChanged and notifies other parts of the application about changes
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
