using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner
{
    public class SearchToursCommand : ICommand
    {
        private readonly MainViewModel _mainViewModel;

        public SearchToursCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Tours" || args.PropertyName == "SearchFilter")
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            };

        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _mainViewModel.Tours.Any() && !string.IsNullOrWhiteSpace(_mainViewModel.SearchFilter);
        }

        public void Execute(object parameter)
        {
            _mainViewModel.FilteredTours = _mainViewModel.Tours.FindAll(t=>t.Contains(_mainViewModel.SearchFilter)==true);
        }
    }
}
