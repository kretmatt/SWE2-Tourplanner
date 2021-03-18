using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner
{
    public class MainViewModel:INotifyPropertyChanged
    {
        private string _searchFilter;

        private List<string> _tours;
        private List<string> _filteredTours;
        public List<string> Tours
        {
            get
            {
                return _tours;
            }
            set
            {
                if (value != _tours)
                {
                    _tours = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<string> FilteredTours
        {
            get
            {
                return _filteredTours;
            }

            set
            {
                if(value!=_filteredTours)
                {
                    _filteredTours = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchFilter
        {
            get
            {
                return _searchFilter;
            }

            set
            {
                if (value != _searchFilter)
                {
                    _searchFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SearchToursCommand { get; }

        public MainViewModel()
        {
            SearchToursCommand = new SearchToursCommand(this);
            Tours = new List<string> { "Tour 1", "Tour 2", "Tour 3", "Tour 34" };
            FilteredTours = Tours;

            Action<int> abc = (int i) =>
            {
                Console.WriteLine(i);
            };

            abc(123);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
