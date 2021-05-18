using Common.Config;
using Microsoft.Win32;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    public class ImportViewModel:BaseViewModel, IDialogRequestClose
    {
        private ObservableCollection<string> jsonPaths;
        private ITourPlannerConfig config;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ObservableCollection<string> JsonPaths
        {
            get { return jsonPaths; }
            set
            {
                if(value!=jsonPaths)
                {
                    jsonPaths = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand SelectPathsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ConfirmImportCommand { get; }
        public ImportViewModel(ITourPlannerConfig config)
        {
            jsonPaths = new ObservableCollection<string>();
            this.config = config;
            SelectPathsCommand = new RelayCommand(
                (_) =>
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.InitialDirectory = config.ExportsDirectory;
                    dialog.Filter = "Json files (*.json)|*.json";
                    dialog.RestoreDirectory = true;
                    dialog.Multiselect = true;
                    if (dialog.ShowDialog() ?? false)
                    {
                        JsonPaths.Clear();
                        foreach(string path in dialog.FileNames)
                        {
                            JsonPaths.Add(path);
                        }
                    }
                },
                (_) => { return true; }
            );

            ExitCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                },
                (_) => { return true; }
            );

            ConfirmImportCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
                },
                (_) => { return JsonPaths.Any(); }

            );
        }
    }
}
