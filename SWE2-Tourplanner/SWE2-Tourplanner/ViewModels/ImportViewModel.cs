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
    /// <summary>
    /// ImportViewModel is used for selecting JSON files for the import process
    /// </summary>
    public class ImportViewModel:BaseViewModel, IDialogRequestClose
    {
        /// <summary>
        /// All selected JSON files for the import process
        /// </summary>
        private ObservableCollection<string> jsonPaths;
        /// <summary>
        /// ITourPlannerConfig instance for starting the OpenFileDialog in an appropriate folder
        /// </summary>
        private ITourPlannerConfig config;
        /// <summary>
        /// Event used for closing the dialog
        /// </summary>
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        /// <value>
        /// All selected JSON files for the import process
        /// </value>
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
        /// <value>
        /// Command for selecting JSON files
        /// </value>
        public ICommand SelectPathsCommand { get; }
        /// <value>
        /// Command for closing the dialog
        /// </value>
        public ICommand ExitCommand { get; }
        /// <value>
        /// Command for starting the import process
        /// </value>
        public ICommand ConfirmImportCommand { get; }
        /// <summary>
        /// Default constructor for ImportViewModel
        /// </summary>
        /// <param name="config">Configuration instance used for OpenFileDialog</param>
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
