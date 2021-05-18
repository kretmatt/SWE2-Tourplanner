using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SWE2_Tourplanner.Commands;

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
        public ICommand AddTourCommand { get; }
        public ICommand RemoveTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand AddLogCommand { get; }
        public ICommand RemoveLogCommand { get; }
        public ICommand EditLogCommand { get; }

        public MainViewModel()
        {
            SearchToursCommand = new RelayCommand(
                (_)=> FilteredTours = Tours.FindAll(t => t.Contains(SearchFilter) == true),
                (_)=> { return Tours.Any() && !string.IsNullOrWhiteSpace(SearchFilter);
            });
            AddTourCommand = new RelayCommand(
                (_) => MessageBox.Show("AddTourCommand stub "),
                (_) => {
                    return true;
            });
            RemoveTourCommand = new RelayCommand(
                (_) => MessageBox.Show("RemoveTourCommand stub "),
                (_) => {
                    return true;
            });
            EditTourCommand = new RelayCommand(
                (_) => MessageBox.Show("EditTourCommand stub "),
                (_) => {
                    return true;
            });
            AddLogCommand = new RelayCommand(
                (_) => MessageBox.Show("AddLogCommand stub "),
                (_) => {
                    return true;
            });
            RemoveLogCommand = new RelayCommand(
                (_) => MessageBox.Show("RemoveLogCommand stub "),
                (_) => {
                    return true;
            });
            EditLogCommand = new RelayCommand(
                (_) => MessageBox.Show("EditLogCommand stub "),
                (_) => {
                    return true;
            });

            Tours = new List<string> { "Tour 1", "Tour 2", "Tour 3", "Tour 34" };
            FilteredTours = Tours;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
