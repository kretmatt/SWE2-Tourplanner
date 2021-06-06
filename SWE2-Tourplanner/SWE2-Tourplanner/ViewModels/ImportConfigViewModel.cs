using Microsoft.Win32;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    /// <summary>
    /// ImportConfigViewModel is used for importing / selecting new configuration files during the execution
    /// </summary>
    public class ImportConfigViewModel:BaseViewModel, IDialogRequestClose
    {
        /// <summary>
        /// Path to the new configuration
        /// </summary>
        private string configPath;
        /// <summary>
        /// Even used for closing the dialog
        /// </summary>
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        /// <value>
        /// Path to the new configuration
        /// </value>
        public string ConfigPath
        {
            get { return configPath; }
            set
            {
                if (value != configPath)
                {
                    configPath = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <value>
        /// Command for selecting a new configuration
        /// </value>
        public ICommand SelectConfigCommand { get; }
        /// <value>
        /// Command for closing the dialog
        /// </value>
        public ICommand ExitCommand { get; }
        /// <summary>
        /// Command for initiating the load config process
        /// </summary>
        public ICommand ConfirmCommand { get; }
        /// <summary>
        /// Default constructor of ImportConfigViewModel
        /// </summary>
        public ImportConfigViewModel()
        {
            ExitCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                },
                (_) => { return true; }
            );

            ConfirmCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
                },
                (_) => { return !string.IsNullOrWhiteSpace(ConfigPath) && File.Exists(ConfigPath); }
            );

            SelectConfigCommand = new RelayCommand(
                (_) =>
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "Json files (*.json)|*.json";
                    dialog.RestoreDirectory = true;
                    if (dialog.ShowDialog() ?? false)
                    {
                        ConfigPath = dialog.FileName;
                    }
                },
                (_) => { return true; }
            );
        }
    }
}
